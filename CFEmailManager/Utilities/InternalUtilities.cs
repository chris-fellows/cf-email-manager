using MimeKit;
using CFEmailManager.Model;

namespace CFEmailManager.Utilities
{
    internal class InternalUtilities
    {      
        /// <summary>
        /// Generates a unique key for the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetEmailKey(EmailObject email)
        {
            return string.Format("{0}|{1}|{2}", email.From.Address.ToLower(), email.ReceivedDate.ToString("yyyy-MM-dd HHmmss"), email.Subject.ToLower());
        }

        /// <summary>
        /// Generates a unique key for the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetEmailKey(MimeMessage email)
        {
            return $"{email.MessageId}";
        }
    }
}

