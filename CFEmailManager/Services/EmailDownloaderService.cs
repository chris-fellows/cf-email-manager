using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using CFUtilities.Encryption;

namespace CFEmailManager.Services
{
    public class EmailDownloaderService : IEmailDownloaderService
    {
        private readonly IEnumerable<IEmailConnection> _emailConnections;        
        private CancellationTokenSource _downloadTaskTokenSource;

        public EmailDownloaderService(IEnumerable<IEmailConnection> emailConnections)                                
        {
            _emailConnections = emailConnections;          
        }   

        public Task<EmailDownloadStatistics> DownloadEmailsAsync(EmailAccount emailAccount,
                        IEmailStorageService emailRepository,
                        bool downloadAttachments,
                        Action<string> actionFolderStart,
                        Action<string> actionFolderEnd,
                        Action downloadStart,
                        Action<EmailDownloadStatistics> downloadEnd)
        {
            var task = Task.Factory.StartNew(() =>
            {
                // Set cancellation token
                _downloadTaskTokenSource = new CancellationTokenSource();
                
                downloadStart();

                // Get email connection                
                var emailConnection = _emailConnections.First(ec => ec.ServerType == emailAccount.ServerType);

                // Get password
                var key = Convert.FromBase64String(System.Configuration.ConfigurationSettings.AppSettings.Get("Random1").ToString());
                var iv = Convert.FromBase64String(System.Configuration.ConfigurationSettings.AppSettings.Get("Random2").ToString());
                var password = Encoding.UTF8.GetString(AESEncryption.Decrypt(Convert.FromBase64String(emailAccount.Password), key, iv));

                // Download                
                var emailDownloadStatistics = emailConnection.Download(emailAccount.Server, emailAccount.EmailAddress, password,
                                emailAccount.LocalFolder, downloadAttachments, emailRepository, _downloadTaskTokenSource.Token,
                                (folder) => // Main thread
                                {
                                    actionFolderStart(folder);
                                },
                                (folder) => // Main thread
                                {
                                    actionFolderEnd(folder);
                                });

                downloadEnd(emailDownloadStatistics);

                return emailDownloadStatistics;
            });
            return task;
        }

        public void Cancel()
        {
            _downloadTaskTokenSource.Cancel();
        }
    }
}
