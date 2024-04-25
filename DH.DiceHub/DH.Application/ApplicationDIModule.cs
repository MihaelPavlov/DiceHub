using Autofac;

namespace DH.Application;

public class ApplicationDIModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(this.ThisAssembly).AsImplementedInterfaces().InstancePerLifetimeScope();

        //builder.RegisterType<LoggingUtils>().As<ILoggingUtils>().InstancePerLifetimeScope();
    }
}
