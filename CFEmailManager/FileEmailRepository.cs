using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CFEmailManager.Model;
using CFUtilities.XML;
using System.Text;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace CFEmailManager
{
    /// <summary>
    /// Email repository stored in local files
    /// </summary>
    public class FileEmailRepository : IEmailRepository
    {
        private string _folder;

        public FileEmailRepository(string folder)
        {
            _folder = folder;
            Directory.CreateDirectory(_folder);
        }

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

            var folderXmlFiles = Directory.GetFiles(path, "Folder.*.xml", SearchOption.TopDirectoryOnly);
            if (folderXmlFiles.Any())
            {
                string folderXmlFile = folderXmlFiles.First();
                EmailFolder emailFolder = XmlSerialization.DeserializeFromFile<EmailFolder>(folderXmlFile);
                emailFolder.LocalFolder = path;
                return emailFolder;
            }

            return null;
        }

        public List<EmailFolder> GetAllFolders()
        {
            return GetAllFoldersInternal(_folder, true);
        }

        private List<EmailFolder> GetAllFoldersInternal(string folder, bool getChildFolders)
        {
            List<EmailFolder> folders = new List<EmailFolder>();

            var folderXmlFiles = Directory.GetFiles(folder, "Folder.*.xml", SearchOption.TopDirectoryOnly);
            if (folderXmlFiles.Any())
            {
                string folderXmlFile = folderXmlFiles.First();
                EmailFolder emailFolder = XmlSerialization.DeserializeFromFile<EmailFolder>(folderXmlFile);
                emailFolder.LocalFolder = folder;
                folders.Add(emailFolder);                
            }

            if (getChildFolders)
            {
                foreach (string subFolder in Directory.GetDirectories(folder))
                {
                    folders.AddRange(GetAllFoldersInternal(subFolder, getChildFolders));
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

            foreach(string emailXmlFile in Directory.GetFiles(emailFolder.LocalFolder, "Email.*.xml", SearchOption.TopDirectoryOnly))
            {
                EmailObject emailObject = XmlSerialization.DeserializeFromFile<EmailObject>(emailXmlFile);
                emails.Add(emailObject);
            }

            return emails;
        }

        public void Update(EmailFolder emailFolder)
        {
            if (String.IsNullOrEmpty(emailFolder.LocalFolder))   // Downloading email
            {
                emailFolder.LocalFolder = GetEmailFolderPath(emailFolder);
            }
            string folderXmlFile = Path.Combine(emailFolder.LocalFolder, string.Format("Folder.{0}.xml", emailFolder.ID));
            XmlSerialization.SerializeToFile<EmailFolder>(emailFolder, folderXmlFile);
        }

        public void Update(EmailObject email, byte[] data, List<byte[]> attachments)
        {
            var folder = GetAllFolders().First(f => f.ID == email.FolderID);
            string emailXmlFile = Path.Combine(folder.LocalFolder, string.Format("Email.{0}.xml", email.ID));
            XmlSerialization.SerializeToFile<EmailObject>(email, emailXmlFile);

            // Save email
            if (data != null && data.Length > 0)
            {
                string emailFile = Path.Combine(folder.LocalFolder, string.Format("Body.{0}.eml", email.ID));
                File.WriteAllBytes(emailFile, data);
            }

            // Save attachments
            if (attachments != null && attachments.Any())
            {
                for(int index =0; index < email.Attachments.Count; index++)
                {
                    var attachment = email.Attachments[index];
                    var attachmentFile = Path.Combine(folder.LocalFolder, $"Attachment.{email.ID}.{attachment.ID}");
                    File.WriteAllBytes(attachmentFile, attachments[index]);
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

        private List<EmailObject> Search(EmailSearch emailSearch, EmailFolder emailFolder)
        {
            List<EmailObject> emails = new List<EmailObject>();
            List<EmailObject> emailsToCheck = GetEmails(emailFolder);

            var emailResults = emailsToCheck.Where(email => emailSearch.IsMatches(email, emailFolder)).ToList();
            return emailResults;
        }

        public byte[] GetEmailContent(EmailObject emailObject)
        {
            var emailFolderPath = GetEmailFolderPath(emailObject);
            var emailFile = Path.Combine(emailFolderPath, $"Body.{emailObject.ID}.eml");
            return File.Exists(emailFile) ? File.ReadAllBytes(emailFile) : new byte[0];
        }

        public byte[] GetEmailAttachmentContent(EmailObject emailObject, int attachmentIndex)
        {
            var emailFolderPath = GetEmailFolderPath(emailObject);
            var attachmentObject = emailObject.Attachments[attachmentIndex];

            var attachmentFile = Path.Combine(emailFolderPath, $"Attachment.{emailObject.ID}.{attachmentObject.ID}");
            return File.Exists(attachmentFile) ? File.ReadAllBytes(attachmentFile) : new byte[0];
        }
    }
}