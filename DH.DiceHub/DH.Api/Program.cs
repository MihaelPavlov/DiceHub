using DH.Adapter.Authentication;
using DH.Adapter.Data;
using DH.Adapter.Scheduling;
using DH.Api;
using DH.Api.Filters;
using DH.Domain;
using Microsoft.Extensions.Caching.Memory;
using DH.Adapter.ChatHub;
using DH.Adapter.QRManager;
using DH.Adapter.ChallengesOrchestrator;
using DH.Adapter.GameSession;
using DH.Application;
using DH.Adapter.PushNotifications;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using DH.Adapter.Reservations;
using Microsoft.OpenApi.Models;
using DH.Domain.Queue.Services;
using DH.Adapter.Email;
using DH.Domain.Adapters.Data;
using DH.Adapter.Statistics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
    options.Filters.Add<ValidationFilterAttribute>();
});
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
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
builder.Services.AddQRManagerAdapter();
builder.Services.AddSchedulingAdapter(builder.Configuration);
builder.Services.AddChallengesOrchestratorAdapter();
builder.Services.AddReservationAdapter();
builder.Services.AddGameSessionAdapter();
builder.Services.AddEmailAdapter(builder.Configuration);
builder.Services.AddStatisticsAdapter();
var test = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dicehub-8c63f-firebase-adminsdk-y31l3-6026a82c88.json")),
});

Console.WriteLine(test.Options.ProjectId);
builder.Services.AddFirebaseMessaging();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var queueDispatcher = scope.ServiceProvider.GetRequiredService<IQueueDispatcher>();
    queueDispatcher.Dispatch();

    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await dataSeeder.SeedAsync();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "[DH] Dice Hub API V1");
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("EnableCORS");

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoint =>
{
    endpoint.MapHub<ChatHubClient>("/chatHub");
    endpoint.MapControllers();
    endpoint.MapFallbackToFile("index.html");
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

// This should always be after UseEndpoints
app.UseSpaStaticFiles();

app.Run();