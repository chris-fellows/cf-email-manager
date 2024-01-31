using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CFEmailManager.Model;

namespace CFEmailManager
{
    public interface IEmailRepository
    {
        void Update(EmailFolder emailFolder);

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

        List<EmailObject> Search(EmailSearch emailSearch);
    }
}
