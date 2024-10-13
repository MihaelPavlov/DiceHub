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
using DH.Application.Games.Commands.Games;

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
builder.Services.LoadDatabase(builder.Configuration);
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

builder.Services.ConfigureSignalR();
builder.Services.ConfigureQrCodeManager();
builder.Services.AuthenticationAdapter(builder.Configuration);
builder.Services.AddSchedulingConfiguration(builder.Configuration);
builder.Services.AddChallengesOrchestrator();
builder.Services.AddGameSessionAdapter();

builder.Services.AddScoped<IWebRootPathHelper, WebRootPathHelper>();
builder.Services.AddScoped<IContainerService, ContainerService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateGameCommand).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "[TRINT] Invest Track API V1");
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