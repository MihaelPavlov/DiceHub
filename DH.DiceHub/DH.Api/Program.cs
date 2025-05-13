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
using DH.Messaging.Publisher;
using DH.Domain.Adapters.Authentication;
using DH.Messaging.Publisher.Authentication;
using DH.Domain.Models.Common;
using DH.Domain.Services.Publisher;
using DH.Application.Services;
using Microsoft.OpenApi.Models;
using DH.Domain.Queue.Services;
using DH.Adapter.Email;
using DH.Domain.Adapters.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
    options.Filters.Add<ValidationFilterAttribute>();
});
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot"; // or "ClientApp/dist" depending on where Angular builds
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
builder.Services.AddScoped<IEventPublisherService, EventPublisherService>();

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

var rabbitMqConfig = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>()
    ?? throw new Exception("Failed to load RabbitMQ configuration. Ensure 'RabbitMq' section exists in appsettings.json.");
// Register RabbitMqOptions as a singleton
builder.Services.AddSingleton(rabbitMqConfig);

builder.Services.AddScoped<IRabbitMqUserContextFactory, RabbitMqUserContextFactory>();

builder.Services.AddScoped<IRabbitMqClient>(sp =>
{
    // Retrieve IUserContextFactory
    var userContextFactory = sp.GetRequiredService<IUserContextFactory>();
    var userContext = userContextFactory.GetUserContextForB2b();

    // Retrieve IRabbitMqUserContextFactory
    var rabbitMqUserContextFactory = sp.GetRequiredService<IRabbitMqUserContextFactory>();

    // Transfer values without direct reference to IUserContext
    rabbitMqUserContextFactory.SetDefaultUserContext(new RabbitMqUserContext
    {
        UserId = userContext.UserId,
        IsAuthenticated = userContext.IsAuthenticated,
        RoleKey = userContext.RoleKey,
        Token = userContext.Token
    });

    return new RabbitMqClient(rabbitMqConfig.EnableMessageQueue, rabbitMqConfig.HostName, rabbitMqConfig.ExchangeName, rabbitMqUserContextFactory);
});

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
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.MapWhen(context =>
    context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase)&&
    !context.Request.Path.StartsWithSegments("/swagger"),
    spaApp =>
    {
        spaApp.UseSpaStaticFiles();
        spaApp.UseSpa(spa =>
        {
            spa.Options.SourcePath = "wwwroot";
        });
    });

app.Run();