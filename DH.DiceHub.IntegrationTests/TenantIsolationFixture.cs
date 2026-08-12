using Npgsql;

namespace DH.DiceHub.IntegrationTests;

public sealed class TenantIsolationFixture : IAsyncLifetime
{
    private readonly string connectionString = Environment.GetEnvironmentVariable("DICEHUB_TEST_CONNECTION")
        ?? "Host=localhost;Port=5432;Database=DH.DiceHub2;Username=postgres;Password=1qaz!QAZ";

    public string TenantA { get; } = $"test-a-{Guid.NewGuid():N}";
    public string TenantB { get; } = $"test-b-{Guid.NewGuid():N}";
    private int tenantSettingA;
    private int tenantSettingB;
    private int categoryA;
    private int categoryB;
    private int gameA;
    private int gameB;

    public async Task InitializeAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await ExecuteAsync(connection, "BEGIN;");
        try
        {
            tenantSettingA = await CreateTenantAsync(connection, TenantA, "Test Tenant A");
            tenantSettingB = await CreateTenantAsync(connection, TenantB, "Test Tenant B");
            categoryA = await CreateCategoryAsync(connection, TenantA, "Test Category A");
            categoryB = await CreateCategoryAsync(connection, TenantB, "Test Category B");
            gameA = await CreateGameAsync(connection, TenantA, categoryA, "Game A");
            gameB = await CreateGameAsync(connection, TenantB, categoryB, "Game B");
            await SeedTenantFixturesAsync(connection, TenantA, gameA, "A");
            await SeedTenantFixturesAsync(connection, TenantB, gameB, "B");
            await ExecuteAsync(connection, "COMMIT;");
        }
        catch
        {
            await ExecuteAsync(connection, "ROLLBACK;");
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await ExecuteAsync(connection, "BEGIN;");
        try
        {
            var escapedA = TenantA.Replace("'", "''");
            var escapedB = TenantB.Replace("'", "''");
            foreach (var table in new[] { "ChallengeStatistics", "Challenges", "Events", "GameReservations", "SpaceTableReservations", "SpaceTables", "ChallengeRewards" })
                await ExecuteAsync(connection, $"DELETE FROM \"{table}\" WHERE \"TenantId\" IN ('{escapedA}','{escapedB}');");
            await ExecuteAsync(connection, $"DO $$ DECLARE r RECORD; BEGIN FOR r IN SELECT table_name FROM information_schema.columns WHERE table_schema='public' AND column_name='TenantId' AND table_name NOT IN ('UniversalChallenges','EmailTemplates') GROUP BY table_name LOOP EXECUTE format('DELETE FROM %I WHERE \"TenantId\" IN (''{escapedA}'',''{escapedB}'')', r.table_name); END LOOP; END $$;");
            await ExecuteAsync(connection, $"DELETE FROM \"Games\" WHERE \"TenantId\" IN ('{escapedA}','{escapedB}');");
            await ExecuteAsync(connection, $"DELETE FROM \"GameCategories\" WHERE \"TenantId\" IN ('{escapedA}','{escapedB}');");
            await ExecuteAsync(connection, $"DELETE FROM \"Tenants\" WHERE \"Id\" IN ('{escapedA}','{escapedB}');");
            await ExecuteAsync(connection, $"DELETE FROM \"TenantSettings\" WHERE \"Id\" IN ({tenantSettingA},{tenantSettingB});");
            await ExecuteAsync(connection, "COMMIT;");
        }
        catch
        {
            await ExecuteAsync(connection, "ROLLBACK;");
            throw;
        }
    }

    public async Task<int> CountGamesAsTenantAsync(string tenantId)
        => await CountRowsAsTenantAsync(tenantId, "Games");

    public async Task<int> CountRowsAsTenantAsync(string tenantId, string table)
    {
        var allowedTables = new[] { "Games", "Challenges", "ChallengeRewards", "Events", "SpaceTables", "SpaceTableReservations", "GameReservations", "ChallengeStatistics", "UserStatistics", "GameEngagementLogs" };
        if (!allowedTables.Contains(table, StringComparer.Ordinal))
            throw new ArgumentOutOfRangeException(nameof(table));
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await ExecuteAsync(connection, "SET ROLE dicehub_user;");
        await ExecuteAsync(connection, $"SET app.tenant_id = '{tenantId.Replace("'", "''")}';");
        await using var command = new NpgsqlCommand($"SELECT COUNT(*) FROM \"{table}\";", connection);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private static async Task<int> CreateTenantAsync(NpgsqlConnection connection, string id, string name)
    {
        await using var command = new NpgsqlCommand("INSERT INTO \"TenantSettings\" (\"AverageMaxCapacity\",\"ChallengeRewardsCountForPeriod\",\"PeriodOfRewardReset\",\"ResetDayForRewards\",\"DaysOff\",\"StartWorkingHours\",\"EndWorkingHours\",\"ChallengeInitiationDelayHours\",\"ReservationHours\",\"BonusTimeAfterReservationExpiration\",\"PhoneNumber\",\"ClubName\",\"IsCustomPeriodOn\",\"IsCustomPeriodSetupComplete\") VALUES (1,5,'Weekly','Sunday','','08:00','20:00',6,'08:00, 20:00',10,'',@name,false,false) RETURNING \"Id\";", connection);
        command.Parameters.AddWithValue("name", name);
        var settingId = Convert.ToInt32(await command.ExecuteScalarAsync());
        await ExecuteAsync(connection, $"INSERT INTO \"Tenants\" (\"Id\",\"TenantName\",\"Town\",\"TenantStatus\",\"CreatedDate\",\"LogoFileName\",\"RegisterQrCode\",\"TenantSettingId\") VALUES ('{id}','{name}','','0',NOW(),'','','{settingId}');");
        return settingId;
    }

    private static async Task<int> CreateCategoryAsync(NpgsqlConnection connection, string tenantId, string name)
    {
        await using var command = new NpgsqlCommand("INSERT INTO \"GameCategories\" (\"Name\",\"TenantId\") VALUES (@name,@tenantId) RETURNING \"Id\";", connection);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("tenantId", tenantId);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private static async Task<int> CreateGameAsync(NpgsqlConnection connection, string tenantId, int categoryId, string name)
    {
        await using var command = new NpgsqlCommand("INSERT INTO \"Games\" (\"Name\",\"IsDeleted\",\"Description_EN\",\"Description_BG\",\"MinAge\",\"MinPlayers\",\"MaxPlayers\",\"AveragePlaytime\",\"CategoryId\",\"CreatedDate\",\"UpdatedDate\",\"ImageUrl\",\"TenantId\") VALUES (@name,false,'','',0,1,4,0,@category,NOW(),NOW(),' ',@tenant) RETURNING \"Id\";", connection);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("category", categoryId);
        command.Parameters.AddWithValue("tenant", tenantId);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private static async Task SeedTenantFixturesAsync(NpgsqlConnection connection, string tenantId, int gameId, string marker)
    {
        var escapedTenant = tenantId.Replace("'", "''");
        await ExecuteAsync(connection, $"INSERT INTO \"ChallengeRewards\" (\"Id\",\"Name_EN\",\"Name_BG\",\"CashEquivalent\",\"Description_EN\",\"Description_BG\",\"RequiredPoints\",\"Level\",\"CreatedBy\",\"CreatedDate\",\"UpdatedBy\",\"UpdatedDate\",\"IsDeleted\",\"ImageUrl\",\"TenantId\") VALUES ((SELECT COALESCE(MAX(\"Id\"),0)+1 FROM \"ChallengeRewards\"),'Reward {marker}','Награда {marker}',0,'','',1,1,'fixture',NOW(),'fixture',NOW(),false,'','{escapedTenant}');");
        await ExecuteAsync(connection, $"INSERT INTO \"Challenges\" (\"Id\",\"RewardPoints\",\"Attempts\",\"CreatedDate\",\"UpdatedDate\",\"CreatedBy\",\"UpdatedBy\",\"GameId\",\"TenantId\") VALUES ((SELECT COALESCE(MAX(\"Id\"),0)+1 FROM \"Challenges\"),1,1,NOW(),NOW(),'fixture','fixture',{gameId},'{escapedTenant}');");
        await ExecuteAsync(connection, $"INSERT INTO \"Events\" (\"Id\",\"Name\",\"Description_EN\",\"Description_BG\",\"StartDate\",\"MaxPeople\",\"GameId\",\"IsCustomImage\",\"IsDeleted\",\"IsJoinChallengeProcessed\",\"ImageUrl\",\"TenantId\") VALUES ((SELECT COALESCE(MAX(\"Id\"),0)+1 FROM \"Events\"),'Event {marker}','','',NOW(),10,{gameId},false,false,false,'','{escapedTenant}');");
        await ExecuteAsync(connection, $"INSERT INTO \"SpaceTables\" (\"Id\",\"CreatedBy\",\"Name\",\"MaxPeople\",\"IsLocked\",\"Password\",\"CreatedDate\",\"IsTableActive\",\"IsSoloModeActive\",\"GameId\",\"TenantId\") VALUES ((SELECT COALESCE(MAX(\"Id\"),0)+1 FROM \"SpaceTables\"),'fixture','Table {marker}',4,false,'',NOW(),true,false,{gameId},'{escapedTenant}');");
        await ExecuteAsync(connection, $"INSERT INTO \"GameReservations\" (\"Id\",\"UserId\",\"ReservationDate\",\"CreatedDate\",\"ReservedDurationMinutes\",\"NumberOfGuests\",\"IsPaymentSuccessful\",\"IsActive\",\"Status\",\"IsReservationSuccessful\",\"GameId\",\"InternalNote\",\"PublicNote\",\"TenantId\") VALUES ((SELECT COALESCE(MAX(\"Id\"),0)+1 FROM \"GameReservations\"),'fixture-user',NOW(),NOW(),60,1,false,true,0,false,{gameId},'','', '{escapedTenant}');");
        await ExecuteAsync(connection, $"INSERT INTO \"SpaceTableReservations\" (\"Id\",\"UserId\",\"NumberOfGuests\",\"ReservationDate\",\"CreatedDate\",\"IsActive\",\"Status\",\"InternalNote\",\"PublicNote\",\"IsReservationSuccessful\",\"TenantId\") VALUES ((SELECT COALESCE(MAX(\"Id\"),0)+1 FROM \"SpaceTableReservations\"),'fixture-user',1,NOW(),NOW(),true,0,'','',false,'{escapedTenant}');");
        await ExecuteAsync(connection, $"INSERT INTO \"UserStatistics\" (\"Id\",\"UserId\",\"TotalChallengesCompleted\",\"TenantId\") VALUES ((SELECT COALESCE(MAX(\"Id\"),0)+1 FROM \"UserStatistics\"),'fixture-user',1,'{escapedTenant}');");
        await ExecuteAsync(connection, $"INSERT INTO \"GameEngagementLogs\" (\"Id\",\"GameId\",\"UserId\",\"DetectedOn\",\"CreatedDate\",\"TenantId\") VALUES ((SELECT COALESCE(MAX(\"Id\"),0)+1 FROM \"GameEngagementLogs\"),{gameId},'fixture-user',NOW(),NOW(),'{escapedTenant}');");
    }

    private static async Task ExecuteAsync(NpgsqlConnection connection, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
