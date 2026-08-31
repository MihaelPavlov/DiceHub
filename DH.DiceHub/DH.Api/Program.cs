using DH.Adapater.Localization;
using DH.Adapter.Authentication;
using DH.Adapter.ChallengeHub;
using DH.Adapter.ChallengesOrchestrator;
using DH.Adapter.ChatHub;
using DH.Adapter.Data;
using DH.Adapter.Email;
using DH.Adapter.FileManager;
using DH.Adapter.GameSession;
using DH.Adapter.PushNotifications;
using DH.Adapter.QRManager;
using DH.Adapter.Reservations;
using DH.Adapter.Scheduling;
using DH.Adapter.Statistics;
using DH.Api;
using DH.Api.Filters;
using DH.Application;
using DH.Domain;
using DH.Domain.Adapters.Data;
using DH.Domain.Adapters.Scheduling;
using FirebaseAdmin;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl.Matchers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
    options.Filters.Add<ValidationFilterAttribute>();

});
builder.Services.AddSingleton<IMemoryCache>(service => new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromMinutes(1.0) }));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // Define the Bearer token security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT", // Optional
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and the JWT token."
    });

    // Apply the Bearer token to all requests
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
builder.Services.AddScoped<IContainerService, ContainerService>();

builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddDataAdapter(builder.Configuration);
builder.Services.AuthenticationAdapter(builder.Configuration);
builder.Services.AddChatHubAdapter();
builder.Services.AddChallengeHubAdapter();
builder.Services.AddQRManagerAdapter();
builder.Services.AddSchedulingAdapter(builder.Configuration);
builder.Services.AddChallengesOrchestratorAdapter();
builder.Services.AddReservationAdapter();
builder.Services.AddGameSessionAdapter();
builder.Services.AddEmailAdapter(builder.Configuration);
builder.Services.AddStatisticsAdapter();
builder.Services.AddFileManager(builder.Configuration);
var firebaseCredentialsJson = builder.Configuration["Firebase:CredentialsJson"];
GoogleCredential firebaseCredential;
if (!string.IsNullOrWhiteSpace(firebaseCredentialsJson))
{
    firebaseCredential = GoogleCredential.FromJson(firebaseCredentialsJson);
}
else
{
    var localCredentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-adminsdk.local.json");
    if (!File.Exists(localCredentialsPath))
    {
        throw new InvalidOperationException(
            "Firebase credentials not configured. Set Firebase:CredentialsJson (env var Firebase__CredentialsJson) " +
            "to the service account JSON content, or place a local firebase-adminsdk.local.json next to the app for development.");
    }
    firebaseCredential = GoogleCredential.FromFile(localCredentialsPath);
}

var firebaseApp = FirebaseApp.Create(new AppOptions() { Credential = firebaseCredential });
Console.WriteLine(firebaseApp.Options.ProjectId);

builder.Services.AddHostedService<MemoryMonitorService>();

builder.Services.AddFirebaseMessaging();
builder.Services.AddHealthChecks();
builder.Services.AddLocalizationAdapter();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var tenantDatabase = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    var appIdentityDatabase = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
    var migrationConnection = builder.Configuration.GetConnectionString("MigrationConnection");
    if (!string.IsNullOrWhiteSpace(migrationConnection))
    {
        tenantDatabase.Database.SetConnectionString(migrationConnection);
        appIdentityDatabase.Database.SetConnectionString(migrationConnection);
    }

    tenantDatabase.Database.Migrate();
    appIdentityDatabase.Database.Migrate();

    #region Testing Purposes
    var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
    var scheduler = await schedulerFactory.GetScheduler();

    var jobGroups = await scheduler.GetJobGroupNames();

    foreach (var group in jobGroups)
    {
        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));

        foreach (var jobKey in jobKeys)
        {
            var triggers = await scheduler.GetTriggersOfJob(jobKey);

            foreach (var trigger in triggers)
            {
                var nextFireTime = trigger.GetNextFireTimeUtc();
                var previousFireTime = trigger.GetPreviousFireTimeUtc();

                Console.WriteLine($"Job: {jobKey.Name}, Trigger: {trigger.Key.Name}");
                Console.WriteLine($"Next Fire Time: {nextFireTime}");
                Console.WriteLine($"Previous Fire Time: {previousFireTime}");
            }
        }
    }
    #endregion Testing Purposes
}

using (var scope = app.Services.CreateScope())
{
    // Back-fill the per-tenant daily job triggers (CloseActiveTablesJob,
    // UserChallengeValidationJob, UserChallengeTop3StreakTrackerJob) for any
    // active tenant that is missing one, and drop the obsolete global triggers
    // the last two used before they became per-tenant. Existing per-tenant
    // triggers are left untouched.
    var schedulerService = scope.ServiceProvider.GetRequiredService<ISchedulerService>();
    try
    {
        await schedulerService.ReconcileTenantDailyJobsAsync(CancellationToken.None);
    }
    catch (Exception ex)
    {
        app.Services.GetRequiredService<ILogger<Program>>()
            .LogError(ex, "Failed to reconcile per-tenant daily job triggers on startup.");
    }
}

using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await dataSeeder.SeedAsync();

    //TODO: Better way to seed eveyrthing maybe ???
    await ApplicationDbContextSeeder.SeedUsers(scope.ServiceProvider);
}
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "[DH] Dice Hub API V1");
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("EnableCORS");

app.UseAuthentication();
app.UseMiddleware<TenantRouteValidationMiddleware>();
app.UseAuthorization();

// Then map endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health");
    endpoints.MapHub<ChatHubClient>("/chatHub");
    endpoints.MapHub<ChallengeHubClient>("/challengeHub");
    endpoints.MapControllers();
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

// This should always be after UseEndpoints
app.Run();
