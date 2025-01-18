using DH.Adapter.Authentication;
using DH.Adapter.Data;
using DH.Adapter.Scheduling;
using DH.Api;
using DH.Api.Filters;
using DH.Domain;
using Microsoft.Extensions.Caching.Memory;
using DH.Adapter.ChatHub;
using DH.Adapter.QRManager;
using Microsoft.Extensions.FileProviders;
using DH.Adapter.ChallengesOrchestrator;
using DH.Adapter.GameSession;
using DH.Domain.Helpers;
using DH.Api.Helpers;
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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
    options.Filters.Add<ValidationFilterAttribute>();
});
builder.Services.AddSingleton<IMemoryCache>(service => new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromMinutes(1.0) }));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
builder.Services.AddScoped<IWebRootPathHelper, WebRootPathHelper>();
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

    return new RabbitMqClient(rabbitMqConfig.EnableMessageQueue,rabbitMqConfig.HostName, rabbitMqConfig.ExchangeName, rabbitMqUserContextFactory);
});

var test = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dicehub-8c63f-firebase-adminsdk-y31l3-6026a82c88.json")),
});

Console.WriteLine(test.Options.ProjectId);
builder.Services.AddFirebaseMessaging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "[DH] Dice Hub API V1");
    });
}
var fileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "images"));

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider,
    RequestPath = "/images"
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("EnableCORS");
app.UseEndpoints(endpoint =>
{
    endpoint.MapHub<ChatHubClient>("/chatHub");
});
app.MapControllers();
app.Run();