using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CFEmailManager.Interfaces;
using CFEmailManager.Model;

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

        public Task DownloadEmailsAsync(EmailAccount emailAccount,
                        IEmailStorageService emailRepository,
                        bool downloadAttachments,
                        Action<string> actionFolderStart,
                        Action<string> actionFolderEnd,
                        Action downloadStart,
                        Action downloadEnd)
        {
            var task = Task.Factory.StartNew(() =>
            {
                // Set cancellation token
                _downloadTaskTokenSource = new CancellationTokenSource();

                downloadStart();

                // Get email connection                
                var emailConnection = _emailConnections.First(ec => ec.ServerType == emailAccount.ServerType);

                // Download                
                emailConnection.Download(emailAccount.Server, emailAccount.EmailAddress, emailAccount.Password,
                                emailAccount.LocalFolder, downloadAttachments, emailRepository, _downloadTaskTokenSource.Token,
                                (folder) => // Main thread
                                {
                                    actionFolderStart(folder);
                                },
                                (folder) => // Main thread
                                {
                                    actionFolderEnd(folder);
                                });

                downloadEnd();
            });
            return task;
        }

        public void Cancel()
        {
            _downloadTaskTokenSource.Cancel();
        }
    }
}
