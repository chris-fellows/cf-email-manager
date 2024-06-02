using System.Collections.Generic;
using CFEmailManager.Model;

namespace CFEmailManager.Interfaces
{
    /// <summary>
    /// Interface to repository storing emails
    /// </summary>
    public interface IEmailStorageService
    {        
        string EmailAddress { get; }

        /// <summary>
        /// Updates email folder with emails
        /// </summary>
        /// <param name="emailFolder"></param>
        void Update(EmailFolder emailFolder);

        /// <summary>
        /// Updates single email
        /// </summary>
        /// <param name="email"></param>
        void Update(EmailObject email, byte[] content, List<byte[]> attachments);

        /// <summary>
        /// Gets folder by path. E.g. ["Projects","ProjectXY","Sent"]
        /// </summary>
        /// <param name="folderNames"></param>
        /// <returns></returns>
        EmailFolder GetFolderByPath(string[] folderNames);

        /// <summary>
        /// Gets all email folders
        /// </summary>
        /// <returns></returns>
        List<EmailFolder> GetAllFolders();

        /// <summary>
        /// Gets all email folders that are immediate children of the email folder
        /// </summary>
        /// <param name="emailFolder"></param>
        /// <returns></returns>
        List<EmailFolder> GetChildFolders(EmailFolder emailFolder);

        /// <summary>
        /// Returns all emails in folder. Not sub-folders.
        /// </summary>
        /// <param name="emailFolder"></param>
        /// <returns></returns>
        List<EmailObject> GetEmails(EmailFolder emailFolder);

        /// <summary>
        /// Searches for emails
        /// </summary>
        /// <param name="emailSearch"></param>
        /// <returns></returns>
        List<EmailObject> Search(EmailSearch emailSearch);

        /// <summary>
        /// Gets email content
        /// </summary>
        /// <param name="emailObject"></param>
        /// <returns></returns>
        byte[] GetEmailContent(EmailObject emailObject);

        /// <summary>
        /// Gets email attachment content
        /// </summary>
        /// <param name="emailObject"></param>
        /// <param name="attachmentIndex"></param>
        /// <returns></returns>
        byte[] GetEmailAttachmentContent(EmailObject emailObject, int attachmentIndex);

        /// <summary>
        /// Sets folder, all sub-folders and all emails as not on the server. Typically the status will be changed
        /// later when we verify that the folder/email exists.
        /// </summary>
        /// <param name="emailFolder"></param>
        void SetNotExistsOnServer(EmailFolder emailFolder);
    }
}
