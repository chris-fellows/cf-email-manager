using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using CFUtilities.Encryption;
using CFUtilities.Utilities;
using CFUtilities.XML;

namespace CFEmailManager
{
    /// <summary>
    /// Emails stored in local files
    /// </summary>
    public class FileEmailStorageService : IEmailStorageService
    {
        private string _folder;
        private string _emailAddress;
        private readonly byte[] _encryptionKey;
        private readonly byte[] _encryptionIV;

        public FileEmailStorageService(string emailAddress, string folder)
        {
            _emailAddress = emailAddress;
            _folder = folder;
            _encryptionKey = Convert.FromBase64String(System.Configuration.ConfigurationSettings.AppSettings.Get("Random1").ToString()) ;
            _encryptionIV = Convert.FromBase64String(System.Configuration.ConfigurationSettings.AppSettings.Get("Random2").ToString());            
            Directory.CreateDirectory(_folder);
        }

        public string EmailAddress => _emailAddress;

        /// <summary>
        /// Gets the local folder path for the folder object
        /// </summary>
        /// <param name="emailFolder"></param>
        /// <returns></returns>
        private string GetEmailFolderPath(EmailFolder emailFolder)
        {
            var emailFolders = GetAllFoldersInternal(_folder, true);
            return GetLocalFolder(emailFolder, emailFolders);            
        }

        /// <summary>
        /// Gets the local folder path for the email object
        /// </summary>
        /// <param name="emailObject"></param>
        /// <returns></returns>
        private string GetEmailFolderPath(EmailObject emailObject)
        {
            var emailFolders = GetAllFoldersInternal(_folder, true);
            return GetLocalFolder(emailObject, emailFolders);
        }
      
        private string GetLocalFolder(EmailObject emailObject, List<EmailFolder> emailFolders)
        {
            EmailFolder emailFolder = emailFolders.First(ef => ef.ID == emailObject.FolderID);
            return GetLocalFolder(emailFolder, emailFolders);
        }

        /// <summary>
        /// Gets the local folder
        /// </summary>
        /// <param name="emailFolder"></param>
        /// <param name="emailFolders"></param>
        /// <returns></returns>
        private string GetLocalFolder(EmailFolder emailFolder, List<EmailFolder> emailFolders)
        {
            if (emailFolder.ParentFolderID == Guid.Empty)
            {
                return Path.Combine(_folder, emailFolder.Name);                
            }
            else
            {
                // From current folder then work up to the root getting the folder names
                List<string> folderNames = new List<string>() { emailFolder.Name };
                EmailFolder currentFolder = emailFolder;
                do
                {
                    // Set to parent folder
                    currentFolder = emailFolders.First(p => p.ID == currentFolder.ParentFolderID);
                    folderNames.Insert(0, currentFolder.Name);
                } while (currentFolder.ParentFolderID != Guid.Empty);

                // Create folder path from all folder names
                string folderForXmlFile = _folder;
                foreach (var folderName in folderNames)
                {
                    folderForXmlFile = Path.Combine(folderForXmlFile, folderName);
                }
                return folderForXmlFile;                               
            }
        }

        public EmailFolder GetFolderByPath(string[] folderPath)
        {
            if (folderPath.Length == 0) return null;    // No Folder.*.xml in root

            string path = _folder;

            foreach(var folderName in folderPath)
            {
                path = Path.Combine(path, folderName);
            }

            if (Directory.Exists(path))
            {
                var folderXmlFiles = Directory.GetFiles(path, "*.Folder.xml", SearchOption.TopDirectoryOnly);
                if (folderXmlFiles.Any())
                {                    
                    EmailFolder emailFolder = XmlSerialization.DeserializeFromFile<EmailFolder>(folderXmlFiles.First());                    
                    return emailFolder;
                }
            }

            return null;
        }

        public List<EmailFolder> GetAllFolders()
        {
            return GetAllFoldersInternal(_folder, true);
        }

        /// <summary>
        /// Gets all EmailFolder instances from within folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="getChildFolders"></param>
        /// <returns></returns>
        private List<EmailFolder> GetAllFoldersInternal(string folder, bool getChildFolders)
        {
            List<EmailFolder> folders = new List<EmailFolder>();

            if (getChildFolders)     // Search child folders too
            {
                var folderXmlFiles = Directory.GetFiles(folder, "*.Folder.xml", SearchOption.AllDirectories);
                foreach (var folderXmlFile in folderXmlFiles)
                {
                    EmailFolder emailFolder = XmlSerialization.DeserializeFromFile<EmailFolder>(folderXmlFile);
                    folders.Add(emailFolder);
                }
            }
            else     //  Search top folder only 
            {
                var folderXmlFiles = Directory.GetFiles(folder, "*.Folder.xml", SearchOption.TopDirectoryOnly);
                if (folderXmlFiles.Any())
                {
                    EmailFolder emailFolder = XmlSerialization.DeserializeFromFile<EmailFolder>(folderXmlFiles.First());
                    folders.Add(emailFolder);
                }
            }

            return folders;
        }

        public List<EmailFolder> GetChildFolders(EmailFolder emailFolder)
        {
            List<EmailFolder> allEmailFolders = GetAllFolders();
            return allEmailFolders.Where(f => f.ParentFolderID == emailFolder.ID).ToList();         
        }

        public List<EmailObject> GetEmails(EmailFolder emailFolder)          
        {
            List<EmailObject> emails = new List<EmailObject>();

            var emailFolderLocalFolder = GetLocalFolder(emailFolder, GetAllFolders());            
            foreach (string emailXmlFile in Directory.GetFiles(emailFolderLocalFolder, "*.Email.xml", SearchOption.TopDirectoryOnly))
            {
                EmailObject emailObject = XmlSerialization.DeserializeFromFile<EmailObject>(emailXmlFile);
                emails.Add(emailObject);
            }

            return emails;
        }

        public void Update(EmailFolder emailFolder)
        {           
            var emailFolderLocalFolder = GetLocalFolder(emailFolder, GetAllFolders());
            string folderXmlFile = Path.Combine(emailFolderLocalFolder, $"{emailFolder.ID}.Folder.xml");
            XmlSerialization.SerializeToFile<EmailFolder>(emailFolder, folderXmlFile);
        }

        public void Update(EmailObject email, byte[] content, List<byte[]> attachments)
        {
            // Get all email folders
            var allEmailFolders = GetAllFolders();

            var folder = allEmailFolders.First(f => f.ID == email.FolderID);
            var emailFolderLocalFolder = GetLocalFolder(folder, allEmailFolders);

            string emailXmlFile = GetEmailObjectFile(emailFolderLocalFolder, email); //  Path.Combine(emailFolderLocalFolder, string.Format("Email.{0}.xml", email.ID));
            XmlSerialization.SerializeToFile<EmailObject>(email, emailXmlFile);

            // Save email
            if (content != null && content.Length > 0)
            {
                string emailFile = GetEmailContentFile(emailFolderLocalFolder, email);  //  Path.Combine(emailFolderLocalFolder, string.Format("Content.{0}.eml", email.ID));
                WriteFileEncrypted(emailFile, content, _encryptionKey, _encryptionIV);                                
            }

            // Delete old attachments, shouldn't exist (because we normally only download email once)
            var filePattern = Path.GetFileName(GetEmailAttachmentFile(emailFolderLocalFolder, email, new EmailAttachment() { ID = Guid.Empty }))
                                        .Replace(Guid.Empty.ToString(), "*");
            var oldAttachments = Directory.GetFiles(emailFolderLocalFolder, filePattern, SearchOption.TopDirectoryOnly);            

            foreach (var oldAttachment in oldAttachments)
            {
                File.Delete(oldAttachment);
            }

            // Save attachments
            if (attachments != null && attachments.Any())
            {                
                for (int index = 0; index < email.Attachments.Count; index++)
                {
                    var attachment = email.Attachments[index];
                    var attachmentFile = GetEmailAttachmentFile(emailFolderLocalFolder, email, attachment);
                    WriteFileEncrypted(attachmentFile, attachments[index], _encryptionKey, _encryptionIV);                    
                }
            }
        }

        public List<EmailObject> Search(EmailSearch emailSearch)
        {
            var emailResults = new List<EmailObject>();

            // Get all folders
            var emailFolders = GetAllFolders();
            
            // Search each folder
            foreach(var emailFolder in emailFolders)
            {
                emailResults.AddRange(Search(emailSearch, emailFolder));
            }

            return emailResults;
        }

        /// <summary>
        /// Searches folder, not sub-folders
        /// </summary>
        /// <param name="emailSearch"></param>
        /// <param name="emailFolder"></param>
        /// <returns></returns>
        private List<EmailObject> Search(EmailSearch emailSearch, EmailFolder emailFolder)
        {
            // Get local folder for email folder
            var emailFolderLocalFolder = GetLocalFolder(emailFolder, GetAllFolders());

            List<EmailObject> emails = new List<EmailObject>();
            List<EmailObject> emailsToCheck = GetEmails(emailFolder);

            var emailResults = emailsToCheck.Where(email => emailSearch.IsMatches(email, emailFolder,
                        () =>
                        {
                            var emailFile = GetEmailContentFile(emailFolderLocalFolder, email);
                            return File.ReadAllText(emailFile);                                                        
                        })).ToList();
            return emailResults;
        }        

        public byte[] GetEmailContent(EmailObject emailObject)
        {
            var emailFolderPath = GetEmailFolderPath(emailObject);            
            var emailFile = GetEmailContentFile(emailFolderPath, emailObject);
            return File.Exists(emailFile) ? ReadFileEncrypted(emailFile, _encryptionKey, _encryptionIV) : new byte[0];
        }

        public byte[] GetEmailAttachmentContent(EmailObject emailObject, int attachmentIndex)
        {
            var emailFolderPath = GetEmailFolderPath(emailObject);
            var attachmentObject = emailObject.Attachments[attachmentIndex];

            var attachmentFile = GetEmailAttachmentFile(emailFolderPath, emailObject, attachmentObject);    //  Path.Combine(emailFolderPath, $"Attachment.{emailObject.ID}.{attachmentObject.ID}");
            return File.Exists(attachmentFile) ? ReadFileEncrypted(attachmentFile, _encryptionKey, _encryptionIV) : new byte[0];
            //return File.Exists(attachmentFile) ? File.ReadAllBytes(attachmentFile) : new byte[0];
        }

        /// <summary>
        /// Sets folder, all sub-folders and all emails as not on the server. Typically the status will be changed
        /// later when we verify that the folder/email exists.
        /// </summary>
        /// <param name="emailFolder"></param>
        /// <param name="emailRepository"></param>
        public void SetNotExistsOnServer(EmailFolder emailFolder)
        {
            emailFolder.ExistsOnServer = false;
            Update(emailFolder);

            // Set emails not on server
            var emails = GetEmails(emailFolder);
            foreach (var email in emails)
            {
                email.ExistsOnServer = false;
                Update(email, new byte[0], new List<byte[]>());
            }

            // Set sub-folders not on server
            var childFolders = GetChildFolders(emailFolder);
            foreach (var childFolder in childFolders)
            {
                SetNotExistsOnServer(childFolder);
            }
        }

        /// <summary>
        /// Return file to store EmailObject instance
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="emailObject"></param>
        /// <returns></returns>
        private static string GetEmailObjectFile(string localFolder, EmailObject emailObject)
        {
            return Path.Combine(localFolder, $"{emailObject.ID}.Email.xml");
        }

        /// <summary>
        /// Return file to store email content
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="emailObject"></param>
        /// <returns></returns>
        private static string GetEmailContentFile(string localFolder, EmailObject emailObject)
        {
            return Path.Combine(localFolder, $"{emailObject.ID}.Content.eml");
        }

        /// <summary>
        /// Returns file to store email attachment
        /// </summary>
        /// <param name="localFolder"></param>
        /// <param name="emailObject"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private static string GetEmailAttachmentFile(string localFolder, EmailObject emailObject, EmailAttachment attachment)
        {
            return Path.Combine(localFolder, $"{emailObject.ID}.{attachment.ID}.Attachment.obj");
        }

        /// <summary>
        /// Writes file as encrypted and compressed
        /// </summary>
        /// <param name="file"></param>
        /// <param name="content"></param>
        private static void WriteFileEncrypted(string file, byte[] content, byte[] key, byte[] iv)
        {         
            var contentEncrypted = AESEncryption.Encrypt(CompressionUtilities.CompressWithDeflate(content), key, iv);
            File.WriteAllBytes(file, contentEncrypted);
        }

        /// <summary>
        /// Reads encrypted and compressed file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static byte[] ReadFileEncrypted(string file, byte[] key, byte[] iv)
        {            
            return CompressionUtilities.DecompressWithDeflate(AESEncryption.Decrypt(File.ReadAllBytes(file), key, iv));            
        }
    }
}