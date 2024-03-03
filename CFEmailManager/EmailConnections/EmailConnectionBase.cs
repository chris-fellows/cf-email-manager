using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EAGetMail;
using CFEmailManager.Model;
using CFEmailManager.Utilities;
using CFUtilities.XML;
using System.Threading;

namespace CFEmailManager.EmailConnections
{
    /// <summary>
    /// Downloads emails via POP3
    /// </summary>
    public abstract class EmailConnectionBase
    {
        ///// <summary>
        ///// Downloads emails via POP
        ///// </summary>
        ///// <param name="server"></param>
        ///// <param name="username"></param>
        ///// <param name="password"></param>
        ///// <param name="downloadFolder"></param>
        //public void DownloadViaPop(string server, string username, string password, string downloadFolder)
        //{
        //    Directory.CreateDirectory(downloadFolder);

        //    // Hotmail/MSN POP3 server is "pop3.live.com"
        //    MailServer oServer = new MailServer(server,
        //                username,
        //                password,
        //                ServerProtocol.Pop3);

        //    // Enable SSL connection.
        //    oServer.SSLConnection = true;

        //    // Set 995 SSL port
        //    oServer.Port = 995;

        //    MailClient oClient = new MailClient("TryIt");
        //    oClient.Connect(oServer);

        //    MailInfo[] infos = oClient.GetMailInfos();
        //    Console.WriteLine("Total {0} email(s)\r\n", infos.Length);
        //    for (int i = 0; i < infos.Length; i++)
        //    {
        //        MailInfo info = infos[i];
        //        Console.WriteLine("Index: {0}; Size: {1}; UIDL: {2}",
        //            info.Index, info.Size, info.UIDL);

        //        // download email from  Hotmail/MSN server
        //        Mail oMail = oClient.GetMail(info);

        //        Console.WriteLine("From: {0}", oMail.From.ToString());
        //        Console.WriteLine("Subject: {0}\r\n", oMail.Subject);

        //        // Generate an unqiue email file name based on date time.
        //        //string fileName = _generateFileName(i + 1);
        //        //string fullPath = string.Format("{0}\\{1}", localInbox, fileName);

        //        string filename = string.Format("{0}.xml", Guid.NewGuid().ToString());
        //        string emailPath = Path.Combine(downloadFolder, filename);
        //        oMail.SaveAs(emailPath, true);

        //        // Save email to local disk
        //        //oMail.SaveAs(fullPath, true);                

        //        // Mark email as deleted from POP3 server.
        //        //oClient.Delete(info);
        //    }

        //    // Quit and expunge emails marked as deleted from POP3 server.
        //    oClient.Quit();
        //}

   
        /// <summary>
        /// Downloads email. Aborts if cancelled
        /// </summary>
        /// <param name="mailClient"></param>
        /// <param name="folder"></param>
        /// <param name="localFolder"></param>
        /// <param name="downloadAttachments"></param>
        protected void DownloadFolder(MailClient mailClient, Imap4Folder folder, string localFolder, bool downloadAttachments,
                                    EmailFolder parentEmailFolder,
                                    IEmailRepository emailRepository,
                                     CancellationToken cancellationToken,
                                    Action<string> folderStartAction, Action<string> folderEndAction)
        {
            // Select folder
            mailClient.SelectFolder(folder);

            // Create local folder            
            Directory.CreateDirectory(localFolder);

            // Check if folder XML exists
            EmailFolder emailFolder = null;
            string folderXmlFile = "";
            var folderXmlFiles = Directory.GetFiles(localFolder, "Folder.*.xml");
            if (folderXmlFiles.Any())
            {
                folderXmlFile = folderXmlFiles.First();
                emailFolder = XmlSerialization.DeserializeFromFile<EmailFolder>(folderXmlFile);
                emailFolder.LocalFolder = localFolder;
            }

            if (emailFolder == null)   // Top level folder
            {
                emailFolder = new EmailFolder()
                {
                    ID = Guid.NewGuid(),
                    ParentFolderID = (parentEmailFolder == null ? Guid.Empty : parentEmailFolder.ID),
                    Name = folder.Name,
                    LocalFolder = localFolder,
                    SyncEnabled = Array.IndexOf(new string[] { "Junk", "Spam", "Work" }, folder.Name) == -1,
                    ExistsOnServer = true
                };

                // Save Folder.xml
                folderXmlFile = string.Format(@"{0}\Folder.{1}.xml", localFolder, emailFolder.ID);
                XmlSerialization.SerializeToFile(emailFolder, folderXmlFile);
            }

            // Set all child email folders as not existing on server so that we can check if it does exit
            var childEmailFolders = emailRepository.GetChildFolders(emailFolder);
            foreach (var childEmailFolder in childEmailFolders)
            {
                SetNotExistsOnServer(childEmailFolder, emailRepository);
            }

            // Only sync if enabled, don't want to sync Junk folder
            if (emailFolder.SyncEnabled)
            {             
                // Action on folder start
                if (folderStartAction != null)
                {
                    folderStartAction(emailFolder.Name);
                }

                // Get old emails in folder so that we can find new items
                var oldEmails = emailRepository.GetEmails(emailFolder);

                // Get emails
                MailInfo[] mailInfos = mailClient.GetMailInfos();

                for (int mailIndex = 0; mailIndex < mailInfos.Length; mailIndex++)
                {
                    MailInfo info = mailInfos[mailIndex];

                    // Download email from  Hotmail/MSN server
                    Mail mail = mailClient.GetMail(info);

                    // Check if email has been previously downloaded
                    string newEmailKey = InternalUtilities.GetEmailKey(mail);
                    EmailObject oldEmail = oldEmails.FirstOrDefault(e => InternalUtilities.GetEmailKey(e) == newEmailKey);

                    // Save email if new
                    if (oldEmail == null)
                    {
                        // Save email to local folder
                        SaveEmail(mail, localFolder, downloadAttachments, emailFolder);
                    }
                    else if (oldEmail.ExistsOnServer == false)
                    {
                        // Ensure that Email.ExistsOnServer=true
                        oldEmail.ExistsOnServer = true;
                        emailRepository.Update(oldEmail);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    // Download sub-folders
                    List<string> subFolderNamesProcessed = new List<string>();
                    foreach (var subFolder in folder.SubFolders)
                    {
                        string localSubFolder = Path.Combine(localFolder, subFolder.Name);
                        DownloadFolder(mailClient, subFolder, localSubFolder, downloadAttachments, emailFolder, emailRepository, cancellationToken, folderStartAction, folderEndAction);
                        subFolderNamesProcessed.Add(subFolder.Name);

                        if (cancellationToken.IsCancellationRequested) break;                        
                    }
                }

                if (folderEndAction != null && !cancellationToken.IsCancellationRequested)
                {
                    folderEndAction(emailFolder.Name);
                }
            }
            
            // Flag folder as existing on server
            emailFolder.ExistsOnServer = true;
            emailRepository.Update(emailFolder);                                      
        }

        /// <summary>
        /// Sets folder, all sub-folders and all emails as not on the server. Typically the status will be changed
        /// later when we verify that the folder/email exists.
        /// </summary>
        /// <param name="emailFolder"></param>
        /// <param name="emailRepository"></param>
        protected void SetNotExistsOnServer(EmailFolder emailFolder, IEmailRepository emailRepository)
        {
            emailFolder.ExistsOnServer = false;
            emailRepository.Update(emailFolder);

            // Set emails not on server
            var emails = emailRepository.GetEmails(emailFolder);
            foreach(var email in emails)
            {
                email.ExistsOnServer = false;
                emailRepository.Update(email);
            }

            // Set sub-folders not on server
            var childFolders = emailRepository.GetChildFolders(emailFolder);
            foreach(var childFolder in childFolders)
            {
                SetNotExistsOnServer(childFolder, emailRepository);
            }
        }

        protected EmailObject SaveEmail(Mail mail, string localFolder, bool downloadAttachments,
                                      EmailFolder emailFolder)
        {
            EmailObject emailObject = new EmailObject()
            {
                ID = Guid.NewGuid(),
                FolderID = emailFolder.ID,
                SenderAddress = mail.From.Address,
                Subject = mail.Subject,
                SentDate = mail.SentDate,
                ReceivedDate = mail.ReceivedDate,
                ExistsOnServer = true
            };            

            // Set file containing email
            string emailFile = string.Format(@"{0}\Body.{1}.eml", localFolder, emailObject.ID);

            // Save email                    
            mail.SaveAs(emailFile, true);

            // Save attachments to file [Email File].[Attachment Name]
            if (downloadAttachments && mail.Attachments != null && mail.Attachments.Any())
            {
                emailObject.Attachments = new List<EmailAttachment>();
                foreach (var attachment in mail.Attachments)
                {
                    EmailAttachment emailAttachment = new EmailAttachment()
                    {
                        ID = Guid.NewGuid(),
                        Name = attachment.Name
                    };

                    string attachmentFile = Path.Combine(localFolder, string.Format("Attachment.{0}.{1}", emailObject.ID, emailAttachment.ID));
                    attachment.SaveAs(attachmentFile, true);

                    emailObject.Attachments.Add(emailAttachment);

                    //if (cancellationToken.IsCancellationRequested)
                    //{
                    //    break;
                    //}
                }
            }

            // Save email to local disk
            //oMail.SaveAs(fullPath, true);                

            // Mark email as deleted from POP3 server.
            //oClient.Delete(info);
            
            string emailXmlFile = string.Format(@"{0}\Email.{1}.xml", localFolder, emailObject.ID);
            XmlSerialization.SerializeToFile(emailObject, emailXmlFile);            

            return emailObject;
        }

        ///// <summary>
        ///// Downlaods emails via IMAP
        ///// </summary>
        ///// <param name="server"></param>
        ///// <param name="username"></param>
        ///// <param name="password"></param>
        ///// <param name="downloadFolder"></param>
        //public void DownloadViaImap(string server, string username, string password, string downloadFolder,
        //                            bool downloadAttachments, IEmailRepository emailRepository)
        //{
        //    Directory.CreateDirectory(downloadFolder);

        //    // Set mail server connection settings
        //    MailServer mailServer = new MailServer(server,
        //                username,
        //                password,
        //                ServerProtocol.Imap4);
            
        //    mailServer.SSLConnection = true;            
        //    mailServer.Port = 993;

        //    MailClient mailClient = new MailClient("TryIt");
        //    mailClient.Connect(mailServer);
            
        //    var folders = mailClient.GetFolders();
        //    foreach (var folder in folders)
        //    {           
        //        string localFolder = Path.Combine(downloadFolder, folder.Name);

        //        DownloadFolder(mailClient, folder, localFolder, downloadAttachments, null, emailRepository);         
        //    }

        //    // Quit and expunge emails marked as deleted from POP3 server.
        //    mailClient.Quit();
        //}
    }
}
