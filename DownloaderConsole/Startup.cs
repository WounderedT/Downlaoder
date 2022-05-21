﻿using DomainProcessors.Extensions;
using DomainProcessors.Extensions.UI.Extensions;
using DownloaderCli.Extensions;
using Handlers.Extensions;
using HttpDocumentHandler.Extensions;
using ImageHosts.Extensions;
using Infra.Storage.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationProvider.Extensions;
using ProgressHandler.Extensions;
using Serilog;

namespace DownloaderConsole
{
    internal class Startup
    {
        internal static IServiceProvider ServiceProvider;

        static Startup()
        {
            ServiceProvider = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            IConfigurationRoot? configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureLogger(serviceCollection, configuration);

            serviceCollection.ConfigureHtmlDocumentHandler();
            serviceCollection.ConfigureDomainProcessors(configuration.GetSection("DomainProcessors"));
            serviceCollection.ConfigureDownloadHadlers(configuration.GetSection("DownloadHandler"));
            serviceCollection.ConfigureDownloaderImageHosts();
            serviceCollection.ConfigureProgressHandler(configuration.GetSection("ProgressHandler"));
            serviceCollection.ConfigureStorageProvider();
            serviceCollection.ConfigureDownloaderCli(configuration.GetSection("DownloaderConsole"));
            serviceCollection.ConfigureNotificationProvider();
            //serviceCollection.AddSingleton<INotifications, Notifications>(builder => new Notifications(configuration.GetSection("Serilog:WriteTo:0:Args:path").Value));

            return serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureLogger(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(logging => logging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger(), true));
        }
    }
}
