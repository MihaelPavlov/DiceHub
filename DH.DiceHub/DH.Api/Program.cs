using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using DH.Adapter.Authentication;
using DH.Adapter.Data;
using DH.Api;
using DH.Application;
using DH.Domain;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.LoadDatabase(builder.Configuration);
builder.Services
                .AddAuthentication("cookie")
                .AddCookie("cookie");
builder.Services
.AddAuthorization();
builder.Services.AuthenticationAdapter();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    // Add modules for each class library in solution
    builder.RegisterAssemblyModules(typeof(DomainDIModule).Assembly);
    builder.RegisterAssemblyModules(typeof(ApplicationDIModule).Assembly);
    builder.RegisterAssemblyModules(typeof(AdapterDataDIModule).Assembly);

    builder.RegisterType<ContainerService>().As<IContainerService>();
});

var app = builder.Build();
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
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();
app.Run();