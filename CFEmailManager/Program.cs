using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CFEmailManager.Forms;
using CFEmailManager.Interfaces;
using CFEmailManager.Services;
using CFUtilities.Logging;
using CFUtilities.Interfaces;
using CFUtilities.Services;

namespace CFEmailManager
{
    static class Program
    {    
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }

        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Create a host builder to build the service provider
        /// </summary>
        /// <returns></returns>
        static IHostBuilder CreateHostBuilder()
        {                   
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {

                    // Register IEmailStorageService for each email account                    
                    var emailAccountService = new EmailAccountService(Path.Combine(System.Configuration.ConfigurationSettings.AppSettings.Get("DataFolder"), "EmailAccounts"));
                    foreach (var emailAccount in emailAccountService.GetAll())
                    {
                        services.AddTransient<IEmailStorageService>((scope) =>
                        {                            
                            return new FileEmailStorageService(emailAccount.EmailAddress, emailAccount.LocalFolder, scope.GetRequiredService<IFileEncryption>());                                                            
                        });
                    }

                    services.AddTransient<IPlaceholderService, PlaceholderService>();
                    services.AddTransient<ILogger>((scope) =>
                    {
                        var placeholderService = scope.GetRequiredService<IPlaceholderService>();

                        // Get log file
                        var logsFolder = System.Configuration.ConfigurationSettings.AppSettings.Get("LogsFolder").ToString();
                        logsFolder = placeholderService.GetWithPlaceholdersReplaced(logsFolder, new Dictionary<string, object>());
                        return new CSVLogger((Char)9, Path.Combine(logsFolder, "{date:MM-yyyy}"), placeholderService);
                    });
                    services.AddTransient<IFileEncryption, AESFileEncryption>();
                    services.AddTransient<IEmailDownloaderService, EmailDownloaderService>();
                    services.AddTransient<IEmailAccountService>((scope) =>
                    {
                        return new EmailAccountService(Path.Combine(System.Configuration.ConfigurationSettings.AppSettings.Get("DataFolder"), "EmailAccounts"));
                    });
                    services.RegisterAllTypes<IEmailConnection>(new[] { Assembly.GetExecutingAssembly() });                    
                    services.AddTransient<MainForm>();
                });
        }

        /// <summary>
        /// Registers all types implementing interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <param name="lifetime"></param>
        private static void RegisterAllTypes<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
            foreach (var type in typesFromAssemblies)
            {
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
            }
        }

        //private static void CreateEmailAccountList(IEmailAccountService emailAccountService)
        //{
        //    int count = 0;
        //    List<EmailAccount> emailAccounts = new List<EmailAccount>();
        //    do
        //    {
        //        count++;
        //        if (System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.EmailAddress", count)) != null)
        //        {
        //            string emailAddress = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.EmailAddress", count)).ToString();
        //            EmailAccount emailAccount = new EmailAccount()
        //            {
        //                ID = Guid.NewGuid().ToString(),
        //                EmailAddress = emailAddress,
        //                Password = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.Password", count)).ToString(),
        //                LocalFolder = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.LocalEmailFolder", count)).ToString(),
        //                Server = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.Server", count)).ToString(),
        //                ServerType = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.ServerType", count)).ToString(),
        //                TimeLastDownload = DateTimeOffset.UtcNow
        //            };
        //            emailAccounts.Add(emailAccount);

        //            emailAccountService.Insert(emailAccount);
        //        }
        //        else
        //        {
        //            count = -1;
        //        }
        //    } while (count > 0);
        //}
    }
}
