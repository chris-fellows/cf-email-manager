using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CFEmailManager.Forms;
using System.IO;
using System.Reflection;
using CFEmailManager.Interfaces;
using CFEmailManager.EmailConnections.MailKit;
using CFEmailManager.Services;

namespace CFEmailManager
{
    static class Program
    {
        ///// <summary>
        ///// The main entry point for the application.
        ///// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new MainForm());
        //}

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
            //new MKTest().Test();

            // Get path to executable
            string currentFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) => {                                                                              
                    // Register IEmailStorageService for each email account
                    var emailAccounts = new EmailAccountService().GetAll();
                    foreach (var emailAccount in emailAccounts)
                    {
                        services.AddTransient<IEmailStorageService>((scope) =>
                        {                            
                            return new FileEmailStorageService(emailAccount.EmailAddress, emailAccount.LocalFolder, scope.GetRequiredService<IFileEncryption>());                                                            
                        });
                    }
                    services.AddTransient<IFileEncryption, AESFileEncryption>();
                    services.AddTransient<IEmailDownloaderService, EmailDownloaderService>();
                    services.AddTransient<IEmailAccountService, EmailAccountService>();
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
    }
}
