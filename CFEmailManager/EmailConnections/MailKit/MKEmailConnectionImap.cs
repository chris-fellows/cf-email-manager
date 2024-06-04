using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using MailKit.Net.Imap;
using MailKit.Security;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CFEmailManager.Utilities;
using System.Text;
using CFUtilities.Encryption;

namespace CFEmailManager.EmailConnections.MailKit
{
    /// <summary>
    /// IMAP email connection for MailKit
    /// </summary>
    public class MKEmailConnectionImap : IEmailConnection
    {
        public string ServerType => "IMAP";

        private DateTimeOffset _lastYield = DateTimeOffset.MinValue;

        public EmailDownloadStatistics Download(string server, string username, string password, string downloadFolder,
                            bool downloadAttachments, IEmailStorageService emailRepository,
                            CancellationToken cancellationToken,
                            Action<string> folderStartAction = null, Action<string> folderEndAction = null)
        {
            var emailDownloadStatistics = new EmailDownloadStatistics();           

            using (var client = new ImapClient(new ProtocolLogger("imap.log")))
            {
                //client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                client.Connect(server, 993, SecureSocketOptions.SslOnConnect);

                client.Authenticate(username, password);
                
                System.Diagnostics.Debug.WriteLine("Personal namespaces:");
                foreach (var ns in client.PersonalNamespaces)
                    System.Diagnostics.Debug.WriteLine($"* \"{ns.Path}\" \"{ns.DirectorySeparator}\"");

                System.Diagnostics.Debug.WriteLine("Shared namespaces:");
                foreach (var ns in client.SharedNamespaces)
                    System.Diagnostics.Debug.WriteLine($"* \"{ns.Path}\" \"{ns.DirectorySeparator}\"");

                System.Diagnostics.Debug.WriteLine("Other namespaces:");
                foreach (var ns in client.OtherNamespaces)
                    System.Diagnostics.Debug.WriteLine($"* \"{ns.Path}\" \"{ns.DirectorySeparator}\"");


                // get the folder that represents the first personal namespace
                var rootFolder = client.GetFolder(client.PersonalNamespaces[0]);                

                // list the folders under the first personal namespace
                var folders = rootFolder.GetSubfolders();
                foreach(var folder in folders)
                {
                    var statistics = DownloadFolder(folder, emailRepository, null, new List<string>() { folder.Name },
                                        downloadAttachments, cancellationToken, folderStartAction, folderEndAction);

                    emailDownloadStatistics.AppendFrom(statistics);

                    if (cancellationToken.IsCancellationRequested) break;                    
                }

                /*
                var folders = client.GetFolders(client.PersonalNamespaces[0]);

                foreach (var folder in folders)
                {
                    System.Diagnostics.Debug.WriteLine($"{folder.Name} - {folder.FullName}");
                }
                */

                /*
                var uids = client.Inbox.Search(SearchQuery.All);

                foreach (var uid in uids)
                {
                    var message = client.Inbox.GetMessage(uid);

                    // write the message to a file
                    message.WriteTo(string.Format("{0}.eml", uid));
                }
                */

                client.Disconnect(true);
            }

            return emailDownloadStatistics;
        }

        /// <summary>
        /// Downloads mail folder messages and optionally attachments, includes sub-folders
        /// </summary>
        /// <param name="mailFolder"></param>
        /// <param name="emailRepository"></param>
        private EmailDownloadStatistics DownloadFolder(IMailFolder mailFolder, IEmailStorageService emailRepository, 
                            EmailFolder parentEmailFolder, List<string> folderNames,
                            bool downloadAttachments,
                            CancellationToken cancellationToken,
                            Action<string> folderStartAction = null, Action<string> folderEndAction = null)
        {
            var emailDownloadStatistics = new EmailDownloadStatistics();

            // Set folder path for progress indication
            StringBuilder folderPath = new StringBuilder("");
            folderNames.ForEach(folderName =>
            {
                if (folderPath.Length > 0) folderPath.Append("/");
                folderPath.Append(folderName);
            });

            // Action on folder start
            if (folderStartAction != null)
            {
                folderStartAction(folderPath.ToString());
            }

            System.Threading.Thread.Yield();

            // Check if folder XML exists            
            EmailFolder emailFolder = emailRepository.GetFolderByPath(folderNames.ToArray());

            if (emailFolder == null)   // Top level folder
            {
                emailFolder = new EmailFolder()
                {
                    ID = Guid.NewGuid(),
                    ParentFolderID = (parentEmailFolder == null ? Guid.Empty : parentEmailFolder.ID),
                    Name = mailFolder.Name,
                    //LocalFolder = localFolder,
                    SyncEnabled = Array.IndexOf(new string[] { "Junk", "Spam" }, mailFolder.Name) == -1,
                    ExistsOnServer = true
                };

                // Save Folder.xml
                emailRepository.Update(emailFolder);    // Sets LocalFolder property
            }

            System.Threading.Thread.Yield();

            // Set all child email folders as not existing on server so that we can check if it does exit
            var childEmailFolders = emailRepository.GetChildFolders(emailFolder);
            foreach (var childEmailFolder in childEmailFolders)
            {
                emailRepository.SetNotExistsOnServer(childEmailFolder);
            }

            System.Threading.Thread.Yield();

            // Get old emails in folder so that we can find new items
            var oldEmails = emailRepository.GetEmails(emailFolder);

            System.Threading.Thread.Yield();

            mailFolder.Open(FolderAccess.ReadOnly);

            System.Threading.Thread.Yield();

            // Download messages
            for (int messageIndex = 0; messageIndex < mailFolder.Count; messageIndex++)
            {
                var mail = mailFolder.GetMessage(messageIndex);

                // Check if email has been previously downloaded
                string newEmailKey = InternalUtilities.GetEmailKey(mail);

                EmailObject oldEmail = oldEmails.FirstOrDefault(e => InternalUtilities.GetEmailKey(e) == newEmailKey);

                // Save email if new
                if (oldEmail == null)     // New email
                {
                    // Save email to local folder
                    SaveEmail(mail, downloadAttachments, emailFolder, emailRepository);

                    emailDownloadStatistics.CountEmailsDownloaded++;
                }
                else if (oldEmail.ExistsOnServer == false)  // Previously downloaded
                {
                    // Ensure that Email.ExistsOnServer=true
                    oldEmail.ExistsOnServer = true;
                    emailRepository.Update(oldEmail, new byte[0], new List<byte[]>());
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (messageIndex % 20 == 0)
                {
                    System.Threading.Thread.Yield();
                }
            }

            System.Threading.Thread.Yield();

            // Process sub-folders
            var subFolders = mailFolder.GetSubfolders();
            foreach(var subFolder in subFolders)
            {
                var subFolderNames = new List<string>();
                subFolderNames.AddRange(folderNames);
                subFolderNames.Add(subFolder.Name);

                var emailDownloadStatistics2 = DownloadFolder(subFolder, emailRepository, emailFolder, subFolderNames, 
                            downloadAttachments, cancellationToken, folderStartAction, folderEndAction);

                emailDownloadStatistics.AppendFrom(emailDownloadStatistics2);
            }

            System.Threading.Thread.Yield();

            if (mailFolder.IsOpen)
            {
                mailFolder.Close();
            }

            if (folderEndAction != null)
            {
                folderEndAction(folderPath.ToString());
            }

            return emailDownloadStatistics;
        }

        /// <summary>
        /// Saves email to email storage service
        /// </summary>
        /// <param name="email"></param>
        /// <param name="downloadAttachments"></param>
        /// <param name="emailFolder"></param>
        /// <param name="emailStorageService"></param>
        private static EmailObject SaveEmail(MimeMessage email, bool downloadAttachments, EmailFolder emailFolder, IEmailStorageService emailStorageService)
        {                        
            EmailObject emailObject = new EmailObject()
            {
                ID = Guid.NewGuid(),
                FolderID = emailFolder.ID,                
                Priority = email.Priority.ToString(),
                From = new EmailAddress() { Address = email.From.First().Name },
                To = email.To.Select(to => new EmailAddress() { Address = to.Name }).ToList(),
                CC = email.Cc == null ? null :
                            email.Cc.Select(cc => new EmailAddress() { Address = cc.Name }).ToList(),
                BCC = email.Bcc == null ? null :
                            email.Bcc.Select(cc => new EmailAddress() { Address = cc.Name }).ToList(),
                ReplyTo = email.ReplyTo == null ? null :
                            email.ReplyTo.Select(rt => new EmailAddress() { Address = rt.Name }).ToList(),
                Subject = email.Subject,
                SentDate = email.Date,       // TODO: Check this
                ReceivedDate = email.Date,  // TODO: Check this
                ExistsOnServer = true
            };            
         
            // Get email content
            var content = new byte[0];            
            using (var stream = new MemoryStream())
            {
                email.WriteTo(stream);
                stream.Position = 0;
                content = new byte[stream.Length];
                stream.Read(content, 0, content.Length);

                int xxxx = 1000;
            }
            
            // Download attachments
            List<byte[]> attachments = new List<byte[]>();
            if (downloadAttachments && email.Attachments != null && email.Attachments.Any())
            {
                foreach(var attachment in email.Attachments)
                {
                    if (attachment.IsAttachment)
                    {
                        if (emailObject.Attachments == null) emailObject.Attachments = new List<EmailAttachment>();

                        using (var stream = new MemoryStream())
                        {
                            attachment.WriteTo(stream);
                            stream.Position = 0;
                            var attachmentData = new byte[stream.Length];
                            stream.Read(attachmentData, 0, attachmentData.Length);
                            attachments.Add(attachmentData);
                        }
                        
                        EmailAttachment emailAttachment = new EmailAttachment()
                        {
                            ID = Guid.NewGuid(),
                            Name = attachment.ContentDisposition?.FileName
                        };
                        if (String.IsNullOrEmpty(emailAttachment.Name)) emailAttachment.Name = "Attachment";
                        emailObject.Attachments.Add(emailAttachment);
                    }
                }
            }
           
            emailStorageService.Update(emailObject, content, attachments);

            return emailObject;
        }        
    }
}
