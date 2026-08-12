namespace DH.DiceHub.IntegrationTests;

public sealed class TenantIsolationTests : IClassFixture<TenantIsolationFixture>
{
    private readonly TenantIsolationFixture fixture;

    public TenantIsolationTests(TenantIsolationFixture fixture) => this.fixture = fixture;

    [Fact]
    public async Task Restricted_role_only_sees_the_active_tenant_games()
    {
        Assert.Equal(1, await fixture.CountGamesAsTenantAsync(fixture.TenantA));
        Assert.Equal(1, await fixture.CountGamesAsTenantAsync(fixture.TenantB));
    }

    [Theory]
    [InlineData("Challenges")]
    [InlineData("ChallengeRewards")]
    [InlineData("Events")]
    [InlineData("SpaceTables")]
    [InlineData("SpaceTableReservations")]
    [InlineData("GameReservations")]
    [InlineData("UserStatistics")]
    [InlineData("GameEngagementLogs")]
    public async Task Restricted_role_only_sees_active_tenant_feature_rows(string table)
    {
        Assert.Equal(1, await fixture.CountRowsAsTenantAsync(fixture.TenantA, table));
        Assert.Equal(1, await fixture.CountRowsAsTenantAsync(fixture.TenantB, table));
    }
}
