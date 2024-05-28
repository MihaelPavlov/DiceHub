using Autofac;
using Autofac.Extensions.DependencyInjection;
using DH.Adapter.Authentication;
using DH.Adapter.Data;
using DH.Api;
using DH.Api.Filters;
using DH.Application;
using DH.Domain;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
    options.Filters.Add<ValidationFilterAttribute>();
}); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.LoadDatabase(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
builder.Services.AuthenticationAdapter(builder.Configuration);
builder.Services.AddAutofac();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    // Add modules for each class library in solution
    builder.RegisterAssemblyModules(typeof(DomainDIModule).Assembly);
    builder.RegisterType<ContainerService>().As<IContainerService>().InstancePerLifetimeScope();
    builder.RegisterAssemblyModules(typeof(ApplicationDIModule).Assembly);
    builder.RegisterAssemblyModules(typeof(AdapterDataDIModule).Assembly);
    builder.RegisterAssemblyModules(typeof(AdapterAuthenticationDIModule).Assembly);
});

var app = builder.Build();

app.SeedUsersAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "[TRINT] Invest Track API V1");
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("EnableCORS");

app.MapControllers();
app.Run();