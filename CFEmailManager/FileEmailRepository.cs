using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CFEmailManager.Model;
using CFUtilities.XML;

namespace CFEmailManager
{
    public class FileEmailRepository : IEmailRepository
    {
        private string _folder;

        public FileEmailRepository(string folder)
        {
            _folder = folder;
            Directory.CreateDirectory(_folder);
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
            string folderXmlFile = Path.Combine(emailFolder.LocalFolder, string.Format("Folder.{0}.xml", emailFolder.ID));
            XmlSerialization.SerializeToFile<EmailFolder>(emailFolder, folderXmlFile);
        }

        public void Update(EmailObject email)
        {
            var folder = GetAllFolders().First(f => f.ID == email.FolderID);
            string emailXmlFile = Path.Combine(folder.LocalFolder, string.Format("Email.{0}.xml", email.ID));
            XmlSerialization.SerializeToFile<EmailObject>(email, emailXmlFile);
        }

        public List<EmailObject> Search(EmailSearch emailSearch)
        {
            List<EmailObject> emails = new List<EmailObject>();

            return emails;
        }

        private List<EmailObject> Search(EmailSearch emailSearch, EmailFolder emailFolder)
        {
            List<EmailObject> emails = new List<EmailObject>();
            List<EmailObject> emailsToCheck = GetEmails(emailFolder);
            foreach(var email in emailsToCheck)
            {

            }
            return emails;
        }
    }
}