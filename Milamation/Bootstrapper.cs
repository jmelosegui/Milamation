using System;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Milamation.Extensions;
using Milamation.ViewModels;
using Milamation.ValidationRules;
using HarvestClient;
using System.Security.Principal;
using NLog.Extensions.Logging;

namespace Milamation
{
    public class Bootstrapper : BootstrapperBase
    {
        private IServiceProvider serviceProvider;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override object GetInstance(Type service, string key)
        {
            var resutl = serviceProvider.GetService(service);

            if (resutl == null)
            {
                resutl = base.GetInstance(service, key);
            }

            return resutl;
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override void Configure()
        {
            base.Configure();

            var settings = BuildConfiguration();

            serviceProvider = BuildServiceProvider(settings);
        }

        private static AppConfiguration BuildConfiguration()
        {
            // Reading configuration
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            var settings = new AppConfiguration();
            configuration.Bind(settings);
            return settings;
        }

        private static IServiceProvider BuildServiceProvider(AppConfiguration settings)
        {   
            var services = new ServiceCollection();

            services.AddSingleton(settings);
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton<IAppUpdater, WinAppUpdater>();
            services.AddTransient(typeof(ILogger<>), typeof(Logger<>));
            services.AddTransient<IHarvestRestClientFactory, HarvestRestClientFactory>();
            services.AddTransient<IHarvestRestClient, HarvestRestClient>();

            services.AddTransient<ShellViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<TimeEntriesReportViewModel>();
            services.RegisterAllTypes<Rule>(new[] { typeof(Bootstrapper).Assembly });

            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            services.AddSingleton<IPrincipal>(principal);

            //TODO: Add NLog
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();

                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageProperties = true,
                    CaptureMessageTemplates = true
                });
            });

            return services.BuildServiceProvider();
        }
    }
}
