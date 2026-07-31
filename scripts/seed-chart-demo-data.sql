DO $$
DECLARE
    tenant_id text := 'dicehub-sofia';
    start_day date := (date_trunc('month', current_date) - interval '5 months')::date;
    end_day date := current_date;
    current_day date;
    user_ids text[];
    game_ids integer[];
    event_ids integer[];
    reward_ids integer[];
    challenge_ids integer[];
    user_count integer;
    game_count integer;
    event_count integer;
    reward_count integer;
    challenge_count integer;
    rows_for_day integer;
    i integer;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "Tenants" WHERE "Id" = tenant_id) THEN
        RAISE EXCEPTION 'Tenant % was not found.', tenant_id;
    END IF;

    DELETE FROM "ClubVisitorLogs" WHERE "TenantId" = tenant_id AND "UserId" LIKE 'chart-demo-user-%';
    DELETE FROM "EventAttendanceLogs" WHERE "TenantId" = tenant_id AND "UserId" LIKE 'chart-demo-user-%';
    DELETE FROM "ReservationOutcomeLogs" WHERE "TenantId" = tenant_id AND "UserId" LIKE 'chart-demo-user-%';
    DELETE FROM "RewardHistoryLogs" WHERE "TenantId" = tenant_id AND "UserId" LIKE 'chart-demo-user-%';
    DELETE FROM "ChallengeHistoryLogs" WHERE "TenantId" = tenant_id AND "UserId" LIKE 'chart-demo-user-%';
    DELETE FROM "GameEngagementLogs" WHERE "TenantId" = tenant_id AND "UserId" LIKE 'chart-demo-user-%';
    DELETE FROM "EventAttendanceLogs"
    WHERE "TenantId" = tenant_id
      AND "EventId" IN (SELECT "Id" FROM "Events" WHERE "TenantId" = tenant_id AND "Name" LIKE '[Chart Demo]%');
    DELETE FROM "Events" WHERE "TenantId" = tenant_id AND "Name" LIKE '[Chart Demo]%';
    DELETE FROM "AspNetUsers" WHERE "TenantId" = tenant_id AND "Id" LIKE 'chart-demo-user-%';

    INSERT INTO "AspNetUsers" (
        "Id", "TenantId", "RefreshToken", "IsDeleted", "RefreshTokenExpiryTime", "TimeZone",
        "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
        "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed",
        "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount"
    )
    SELECT
        'chart-demo-user-' || lpad(s.user_number::text, 2, '0'),
        tenant_id,
        NULL,
        false,
        now(),
        'Europe/Sofia',
        'chart.user' || lpad(s.user_number::text, 2, '0') || '@dicehub.local',
        upper('chart.user' || lpad(s.user_number::text, 2, '0') || '@dicehub.local'),
        'chart.user' || lpad(s.user_number::text, 2, '0') || '@dicehub.local',
        upper('chart.user' || lpad(s.user_number::text, 2, '0') || '@dicehub.local'),
        true,
        NULL,
        md5(random()::text),
        md5(random()::text),
        NULL,
        false,
        false,
        NULL,
        true,
        0
    FROM generate_series(1, 18) AS s(user_number);

    SELECT array_agg("Id" ORDER BY "Id") INTO user_ids
    FROM "AspNetUsers"
    WHERE "TenantId" = tenant_id AND "Id" LIKE 'chart-demo-user-%';

    SELECT array_agg("Id" ORDER BY "Id") INTO game_ids
    FROM "Games"
    WHERE "TenantId" = tenant_id AND "IsDeleted" = false;

    SELECT array_agg("Id" ORDER BY "Id") INTO reward_ids
    FROM "ChallengeRewards"
    WHERE "TenantId" = tenant_id AND "IsDeleted" = false;

    SELECT array_agg("Id" ORDER BY "Id") INTO challenge_ids
    FROM "Challenges"
    WHERE "TenantId" = tenant_id;

    user_count := array_length(user_ids, 1);
    game_count := array_length(game_ids, 1);
    reward_count := array_length(reward_ids, 1);
    challenge_count := array_length(challenge_ids, 1);

    IF coalesce(game_count, 0) = 0 THEN
        RAISE EXCEPTION 'No active games found for tenant %.', tenant_id;
    END IF;
    IF coalesce(reward_count, 0) = 0 THEN
        RAISE EXCEPTION 'No rewards found for tenant %.', tenant_id;
    END IF;
    IF coalesce(challenge_count, 0) = 0 THEN
        RAISE EXCEPTION 'No challenges found for tenant %.', tenant_id;
    END IF;

    FOR i IN 1..10 LOOP
        INSERT INTO "Events" (
            "Name", "Description_EN", "Description_BG", "StartDate", "MaxPeople", "GameId",
            "IsCustomImage", "IsDeleted", "IsJoinChallengeProcessed", "ImageUrl", "TenantId"
        )
        VALUES (
            '[Chart Demo] Event ' || i,
            'Demo event for chart data',
            'Demo event for chart data',
            (start_day + (i * 14) + ((18 + (i % 4)) || ' hours')::interval)::timestamptz,
            18 + (i * 3),
            game_ids[((i - 1) % game_count) + 1],
            false,
            false,
            true,
            '',
            tenant_id
        );
    END LOOP;

    SELECT array_agg("Id" ORDER BY "StartDate", "Id") INTO event_ids
    FROM "Events"
    WHERE "TenantId" = tenant_id AND "IsDeleted" = false;

    event_count := array_length(event_ids, 1);

    FOR current_day IN
        SELECT generate_series(start_day, end_day, interval '1 day')::date
    LOOP
        rows_for_day := 10 + (extract(isodow from current_day)::integer % 5) * 3;

        INSERT INTO "ClubVisitorLogs" ("UserId", "LogDate", "CreatedDate", "TenantId")
        SELECT
            user_ids[((n + extract(doy from current_day)::integer) % user_count) + 1],
            (current_day + ((10 + (n % 12)) || ' hours')::interval + ((n * 7 % 60) || ' minutes')::interval)::timestamptz,
            now(),
            tenant_id
        FROM generate_series(1, rows_for_day + floor(random() * 7)::integer) AS n;

        INSERT INTO "GameEngagementLogs" ("GameId", "UserId", "DetectedOn", "CreatedDate", "TenantId")
        SELECT
            game_ids[((n + extract(day from current_day)::integer) % game_count) + 1],
            user_ids[((n * 2 + extract(doy from current_day)::integer) % user_count) + 1],
            (current_day + ((11 + (n % 10)) || ' hours')::interval + ((n * 11 % 60) || ' minutes')::interval)::timestamptz,
            now(),
            tenant_id
        FROM generate_series(1, rows_for_day + 8 + floor(random() * 10)::integer) AS n;

        INSERT INTO "ReservationOutcomeLogs" ("ReservationId", "UserId", "OutcomeDate", "CreatedDate", "Outcome", "Type", "TenantId")
        SELECT
            9000000 + extract(doy from current_day)::integer * 100 + n,
            user_ids[((n * 3 + extract(doy from current_day)::integer) % user_count) + 1],
            (current_day + ((12 + (n % 9)) || ' hours')::interval + ((n * 13 % 60) || ' minutes')::interval)::timestamptz,
            now(),
            CASE WHEN n % 5 = 0 THEN 1 ELSE 0 END,
            CASE WHEN n % 3 = 0 THEN 1 ELSE 0 END,
            tenant_id
        FROM generate_series(1, 6 + (extract(isodow from current_day)::integer % 4) + floor(random() * 5)::integer) AS n;

        INSERT INTO "RewardHistoryLogs" (
            "RewardId", "UserId", "IsExpired", "IsCollected", "CollectedDate", "ExpiredDate", "CreatedDate", "TenantId"
        )
        SELECT
            reward_ids[((n + extract(day from current_day)::integer) % reward_count) + 1],
            user_ids[((n * 5 + extract(doy from current_day)::integer) % user_count) + 1],
            n % 4 = 0,
            n % 4 <> 0,
            CASE WHEN n % 4 <> 0 THEN (current_day + ((15 + (n % 6)) || ' hours')::interval)::timestamptz ELSE NULL END,
            CASE WHEN n % 4 = 0 THEN (current_day + ((15 + (n % 6)) || ' hours')::interval)::timestamptz ELSE NULL END,
            now(),
            tenant_id
        FROM generate_series(1, 5 + (extract(isodow from current_day)::integer % 3) + floor(random() * 5)::integer) AS n;

        INSERT INTO "ChallengeHistoryLogs" ("UserId", "ChallengeId", "Outcome", "OutcomeDate", "CreatedDate", "TenantId")
        SELECT
            user_ids[((n + extract(doy from current_day)::integer) % user_count) + 1],
            challenge_ids[((n * 2 + extract(day from current_day)::integer) % challenge_count) + 1],
            CASE WHEN n % 6 = 0 THEN 1 ELSE 0 END,
            (current_day + ((13 + (n % 8)) || ' hours')::interval + ((n * 17 % 60) || ' minutes')::interval)::timestamptz,
            now(),
            tenant_id
        FROM generate_series(1, 11 + (extract(isodow from current_day)::integer % 5) + floor(random() * 9)::integer) AS n;
    END LOOP;

    INSERT INTO "EventAttendanceLogs" ("UserId", "EventId", "LogDate", "CreatedDate", "TenantId")
    SELECT
        user_ids[((n + e."Id") % user_count) + 1],
        e."Id",
        (e."StartDate" + ((n % 3) || ' hours')::interval + ((n * 9 % 60) || ' minutes')::interval)::timestamptz,
        now(),
        tenant_id
    FROM "Events" e
    CROSS JOIN LATERAL generate_series(1, LEAST(e."MaxPeople", 12 + (e."Id" % 18))) AS n
    WHERE e."TenantId" = tenant_id
      AND e."IsDeleted" = false
      AND e."StartDate" >= start_day::timestamptz
      AND e."StartDate" <= (end_day + 1)::timestamptz;

    RAISE NOTICE 'Seeded chart data for tenant %. Users: %, Games: %, Events: %, Rewards: %, Challenges: %',
        tenant_id, user_count, game_count, event_count, reward_count, challenge_count;
END $$;
