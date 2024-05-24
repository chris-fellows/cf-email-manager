using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CFEmailManager.Interfaces;
using CFEmailManager.Model;

namespace CFEmailManager
{
    public class EmailDownloader : IEmailDownloader
    {
        private readonly IEnumerable<IEmailConnection> _emailConnections;
        //private readonly IEnumerable<IEmailRepository> _emailRepositories;

        private CancellationTokenSource _downloadTaskTokenSource;

        public EmailDownloader(IEnumerable<IEmailConnection> emailConnections)                                
        {
            _emailConnections = emailConnections;          
        }   

        public Task DownloadEmailsAsync(EmailAccount emailAccount,
                        IEmailRepository emailRepository,
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
                //var emailRepository = _emailRepositories.First(er => er.EmailAddress == emailAccount.EmailAddress);

                //emailConnection.DownloadViaImap("imap-mail.outlook.com", emailAccount.EmailAddress, emailAccount.Password, emailAccount.LocalFolder, true, emailRepository);
                emailConnection.Download(emailAccount.Server, emailAccount.EmailAddress, emailAccount.Password,
                                emailAccount.LocalFolder, true, emailRepository, _downloadTaskTokenSource.Token,
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
