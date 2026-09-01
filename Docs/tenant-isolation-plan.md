# Tenant Isolation Plan

This checklist tracks the DiceHub tenant-isolation refactoring. Keep the boxes updated as each item is verified in code, database, and integration tests.

## 0. Tenant contract

- [x] A user may access multiple tenants.
- [x] SuperAdmin may access all tenants.
- [x] Tenant identity is represented by `tenantId`.
- [x] Tenant setup is restricted to the approved application/setup flow.
- [ ] Every tenant-scoped request has one validated effective tenant.
- [ ] System/global requests are explicitly identified and cannot fall through to tenant data.

## 1. Reproduce and baseline the leak

- [x] Create two isolated test tenants: `dicehub-sofia` and `dragon-hub`.
- [ ] Record distinct games, challenges, rewards, events, space settings, and leaderboard activity for each tenant.
- [ ] Capture the browser request URL, tenant headers, JWT tenant claim, and API response for every affected feature.
- [x] Record row counts by tenant before applying fixes.

### Baseline recorded 2026-07-31

- Runtime database role is `postgres` with `rolsuper = true` and `rolbypassrls = true`.
- RLS is disabled on all 70 public tables; no public RLS policies exist.
- `dicehub-sofia` has 5 games, 20 challenges, 30 challenge rewards, 8 universal challenges, 12 events, and 3,327 visitor logs.
- `dragon-hub` has 8 games and currently has no challenge/event rows in the baseline query.
- Tenant settings are stored distinctly: `dicehub sofia` is linked to setting ID 2 and `Dragon Hub` to setting ID 3.

### Enforcement applied 2026-07-31

- A local `dicehub_user` role was created with `NOSUPERUSER`, `NOBYPASSRLS`, and CRUD/sequence permissions.
- RLS is now enabled and forced on all 46 tenant tables, with one `tenant_isolation_policy` per table.
- Each policy has both `USING` and `WITH CHECK` clauses keyed to `current_setting('app.tenant_id', true)`.
- Under `SET ROLE dicehub_user`, tenant context returned 5 Sofia games versus 8 Dragon Hub games; tenantless context returned zero tenant rows.

## 2. Resolve tenant context consistently

- [x] Define the effective-tenant precedence: validated route tenant, validated tenant header, then normal-user JWT tenant claim.
- [x] Allow SuperAdmin tenant selection only after validating the selected tenant.
- [x] Reject mismatched route/header/token tenant identities.
- [x] Ensure tenant-scoped frontend services send tenant context through `X-Tenant-Id`.
- [x] Ensure controllers without `api/{tenant}/...` routes still receive the effective tenant.
- [ ] Add request logging for effective tenant during development diagnostics.

## 3. Fix database connection isolation

- [x] Reset `app.tenant_id` whenever a pooled PostgreSQL connection opens.
- [x] Set `app.tenant_id` from the effective request tenant.
- [x] Never reuse a previous tenant value when the current request is tenantless.
- [x] Add a restricted runtime database role with `NOSUPERUSER` and `NOBYPASSRLS` in the local database.
- [x] Stop running the application with the `postgres` superuser for runtime connections; migrations use the separate `MigrationConnection`.

## 4. Verify and enforce PostgreSQL RLS

- [x] List every table containing `TenantId` (47 columns across 46 tenant tables plus `AspNetUsers`).
- [x] Confirm RLS is enabled on every tenant-scoped table (`AspNetUsers` is deliberately excluded).
- [x] Confirm each tenant table has a `USING` policy.
- [x] Confirm each tenant table has a `WITH CHECK` policy for inserts/updates.
- [x] Verify policies are active for the restricted runtime role using tenant A/B row-count checks.
- [x] Add a deliberate shared-data strategy for `UniversalChallenges` and `EmailTemplates` (excluded from tenant RLS and EF tenant filters).

Useful checks:

```sql
SELECT schemaname, tablename, policyname, qual, with_check
FROM pg_policies
WHERE schemaname = 'public';

SELECT rolname, rolsuper, rolbypassrls
FROM pg_roles
WHERE rolname IN ('postgres', 'dicehub_user', 'app_user');
```

## 5. Complete the entity model inventory

- [ ] Classify every entity as tenant-specific, system/global, or tenant-directory metadata.
- [x] Classify `UniversalChallenges` and `EmailTemplates` as shared information.
- [ ] Add direct `TenantId` to tenant-specific entities that currently rely on indirect relationships.
- [ ] Decide the final isolation model for `TenantSetting`.
- [ ] Ensure tenant-specific entities receive `TenantId` on creation.
- [ ] Prevent client input from assigning another tenant’s `TenantId`.

Known high-risk areas:

- [ ] `TenantSetting` is isolated by owning `Tenant` or receives a direct `TenantId`.
- [x] Tenant/user settings are isolated for employee background updates and tenant-scoped user settings.
- [ ] Statistics and leaderboard entities are isolated.
- [ ] Reservation, participant, and history entities are isolated.

## 6. Audit queries and caches

- [x] Add EF tenant query filters for `TenantEntity` records (including games, challenges, rewards, events, and universal challenges).
- [ ] Audit games and inventories.
- [ ] Audit challenges and universal challenges.
- [ ] Audit rewards and reward history.
- [ ] Audit events and event participants.
- [ ] Audit space tables and space settings.
- [ ] Audit statistics and leaderboards.
- [ ] Audit reservations and reservation history.
- [ ] Audit email templates and email history.
- [x] Remove the identified `TenantSettings Id = 1` lookup from custom-period settings updates; resolve settings through the active tenant.
- [ ] Review raw SQL and `IgnoreQueryFilters` usage.
- [ ] Make every cache key include tenant identity.

## 7. Seed and tenant-creation policy

- [ ] Decide whether a new tenant starts empty or receives default catalog data.
- [ ] Stop seeding DiceHub Sofia data into unrelated tenants.
- [ ] If defaults are required, seed them explicitly with the new tenant ID during tenant setup.
- [ ] Verify games/categories are created only for the new tenant.
- [ ] Verify challenges/rewards are created only according to the selected policy.
- [ ] Verify email templates are copied per tenant without cross-tenant reads.

## 8. Tenant settings and setup data

- [x] Tenant setup stores the entered club name in the created `TenantSetting`.
- [x] Tenant setup stores phone, capacity, working hours, days off, and reservation hours.
- [x] Settings retrieval was changed from always reading global settings row `Id = 1` to resolving the active tenant’s linked settings.
- [x] Verify the new tenant’s settings through the database.
- [ ] Verify SuperAdmin tenant preview loads the selected tenant’s settings.
- [ ] Verify club name, phone, capacity, hours, and days off do not show another tenant’s values.

## 9. Integration test matrix

Integration tests must be self-contained and must not depend on the existing
`dicehub-sofia` or `dragon-hub` records. Each test run must:

1. Create unique temporary Tenant A and Tenant B records.
2. Insert minimal tenant-owned fixtures with distinct values in every tested area.
3. Execute the API assertions using each tenant's request context.
4. Run cleanup in `finally`, deleting the temporary tenants and all dependent rows.
5. Fail if cleanup cannot remove all temporary rows.

- [x] Build a disposable two-tenant integration-test fixture.
- [x] Create and seed Tenant A/B inside the test run.
- [x] Clean Tenant A/B in `finally`, including dependent tenant rows.

- [x] Tenant A cannot read Tenant B games (restricted-role/RLS smoke test).
- [x] Tenant A cannot read Tenant B challenges or rewards.
- [x] Tenant A cannot read Tenant B events.
- [x] Tenant A cannot read Tenant B space settings or reservations.
- [x] Tenant A cannot read Tenant B leaderboard/statistics data (statistics/engagement smoke test).
- [x] Employee operations validate the active tenant when reading, updating, or deleting by ID.
- [ ] Tenant A cannot update Tenant B records by ID.
- [ ] SuperAdmin system view can intentionally aggregate tenants.
- [ ] SuperAdmin preview is restricted to the selected tenant.
- [ ] Tenant switching updates all feature data without a full logout.
- [ ] A tenantless request redirects or fails safely instead of returning tenant data.

The first smoke test is run with:

```bash
dotnet test DH.DiceHub.IntegrationTests/DH.DiceHub.IntegrationTests.csproj
```

Set `DICEHUB_TEST_CONNECTION` to point the fixture at a disposable database;
otherwise it uses the local development database connection and removes only
its uniquely generated `test-a-*`/`test-b-*` records.

## 10. Completion criteria

- [ ] Two-tenant integration tests pass.
- [ ] Database role and RLS checks pass in the local PostgreSQL instance.
- [ ] API request logs show the expected effective tenant for every tenant-scoped request.
- [ ] No DiceHub Sofia data appears while operating in Dragon Hub.
- [ ] No migration or seed process reintroduces cross-tenant rows.
- [ ] Backend and frontend builds pass.
