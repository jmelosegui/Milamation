using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using System;

namespace HarvestReport.Utils
{
    internal static class Bootstrapper
    {
        public static IServiceProvider Run()
        {
            var settings = BuildConfiguration();

            return BuildServiceProvider(settings);
        }

        private static AppConfiguration BuildConfiguration()
        {
            // Reading configuration
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);

            var configuration = builder.Build();
            var settings = new AppConfiguration();
            configuration.Bind(settings);
            return settings;
        }

        private static IServiceProvider BuildServiceProvider(AppConfiguration settings)
        {
            // Configuring Services
            var services = new ServiceCollection();

            services.AddSingleton(settings);
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();

                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageProperties = true,
                    CaptureMessageTemplates = true
                });
            });

            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(Bootstrapper).Assembly);

            return services.BuildServiceProvider();
        }
    }
}
