using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAGetMail;
using CFEmailManager.Model;

namespace CFEmailManager
{
    internal class InternalUtilities
    {
        /// <summary>
        /// Generates a unique key for the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetEmailKey(Mail email)
        {
            return string.Format("{0}|{1}|{2}", email.From.Address.ToLower(), email.ReceivedDate.ToString("yyyy-MM-dd HHmmss"), email.Subject.ToLower());
        }

        /// <summary>
        /// Generates a unique key for the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetEmailKey(EmailObject email)
        {
            return string.Format("{0}|{1}|{2}", email.SenderAddress.ToLower(), email.ReceivedDate.ToString("yyyy-MM-dd HHmmss"), email.Subject.ToLower());
        }
    }
}

