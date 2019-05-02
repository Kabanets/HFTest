using Autofac;
using Hangfire;
using System.Linq;

namespace HFTest.App_Start
{
    public class DependenciesConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(typeof(EmailService).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces().InstancePerDependency();

            IContainer applicationContainer = builder.Build();
            GlobalConfiguration.Configuration.UseAutofacActivator(applicationContainer, false);
        }

    }
}