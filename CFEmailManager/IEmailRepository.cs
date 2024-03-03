using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFEmailManager.Model;

namespace CFEmailManager
{
    /// <summary>
    /// Interface to repository storing emails
    /// </summary>
    public interface IEmailRepository
    {
        /// <summary>
        /// Updates email folder with emails
        /// </summary>
        /// <param name="emailFolder"></param>
        void Update(EmailFolder emailFolder);

        /// <summary>
        /// Updates single email
        /// </summary>
        /// <param name="email"></param>
        void Update(EmailObject email);

        /// <summary>
        /// Gets all email folders
        /// </summary>
        /// <returns></returns>
        List<EmailFolder> GetAllFolders();

        /// <summary>
        /// Gets all email folders that are children of the email folder
        /// </summary>
        /// <param name="emailFolder"></param>
        /// <returns></returns>
        List<EmailFolder> GetChildFolders(EmailFolder emailFolder);

        /// <summary>
        /// Returns all emails in folder
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
    }
}
