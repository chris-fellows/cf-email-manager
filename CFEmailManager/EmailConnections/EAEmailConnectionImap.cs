﻿using EAGetMail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CFEmailManager.Interfaces;

namespace CFEmailManager.EmailConnections
{
    /// <summary>
    /// Email downloader via Imap
    /// </summary>
    internal class EAEmailConnectionImap : EAEmailConnectionBase, IEmailConnection
    {
        public string ServerType => "IMAP";

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
                DownloadFolder(mailClient, folder, downloadAttachments, null, emailRepository, new List<string>(), cancellationToken, folderStartAction, folderEndAction);
            }

            // Quit and expunge emails marked as deleted from POP3 server.
            mailClient.Quit();
        }
    }
}