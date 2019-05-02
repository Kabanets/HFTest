using Hangfire;
using Hangfire.Common;
using Hangfire.MySql;
using Microsoft.Owin;
using Owin;
using System;
using System.Configuration;
using System.Transactions;
using HFTest.Services;

[assembly: OwinStartupAttribute(typeof(HFTest.Startup))]
namespace HFTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            string mySqlConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            GlobalConfiguration.Configuration.UseStorage(new MySqlStorage(
                mySqlConnectionString,
                new MySqlStorageOptions
                {
                    TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(1),
                    TablesPrefix = "Hangfire"
                }));
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete });
            app.UseHangfireDashboard("/Jobs", new DashboardOptions
            {
                //Authorization = new[] { new HangfireDashboardAuthorizationFilter(@"Administrators") }
            });

            var options = new BackgroundJobServerOptions
            {
                // This is the default value
                // WorkerCount = Environment.ProcessorCount * 8

            };

            app.UseHangfireServer(options);

            var manager = new RecurringJobManager();
            manager.AddOrUpdate("SomeServiceJob", Job.FromExpression((ISomeService ss) => ss.SomeMethod()), "1 * * * *", TimeZoneInfo.Local);

        }
    }
}
