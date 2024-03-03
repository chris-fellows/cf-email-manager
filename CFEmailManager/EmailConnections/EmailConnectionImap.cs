using EAGetMail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CFEmailManager.EmailConnections
{
    /// <summary>
    /// Email downloader via Imap
    /// </summary>
    internal class EmailConnectionImap : EmailConnectionBase, IEmailConnection
    {
        public void Download(string server, string username, string password, string downloadFolder,
                            bool downloadAttachments, IEmailRepository emailRepository,
                             CancellationToken cancellationToken,
                            Action<string> folderStartAction = null, Action<string> folderEndAction = null)
        {
            Directory.CreateDirectory(downloadFolder);

            // Set mail server connection settings
            MailServer mailServer = new MailServer(server,
                        username,
                        password,
                        ServerProtocol.Imap4);

            mailServer.SSLConnection = true;
            mailServer.Port = 993;

            MailClient mailClient = new MailClient("TryIt");
            mailClient.Connect(mailServer);

            var folders = mailClient.GetFolders();
            foreach (var folder in folders)
            {
                string localFolder = Path.Combine(downloadFolder, folder.Name);

                DownloadFolder(mailClient, folder, localFolder, downloadAttachments, null, emailRepository, cancellationToken, folderStartAction, folderEndAction);
            }

            // Quit and expunge emails marked as deleted from POP3 server.
            mailClient.Quit();
        }
    }
}
