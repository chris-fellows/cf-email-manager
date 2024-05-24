using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using System.Collections.Generic;

namespace CFEmailManager
{
    /// <summary>
    /// Email account repository from config file
    /// </summary>
    public class ConfigEmailAccountRepository : IEmailAccountRepository
    {
        public List<EmailAccount> GetAll()
        {
            int count = 0;
            List<EmailAccount> emailAccounts = new List<EmailAccount>();
            do
            {
                count++;
                if (System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.EmailAddress", count)) != null)
                {
                    string emailAddress = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.EmailAddress", count)).ToString();
                    EmailAccount emailAccount = new EmailAccount()
                    {
                        EmailAddress = emailAddress,
                        Password = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.Password", count)).ToString(),
                        LocalFolder = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.LocalEmailFolder", count)).ToString(),
                        Server = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.Server", count)).ToString(),
                        ServerType = System.Configuration.ConfigurationSettings.AppSettings.Get(string.Format("EmailAccount.{0}.ServerType", count)).ToString()
                    };
                    emailAccounts.Add(emailAccount);
                }
                else
                {
                    count = -1;
                }
            } while (count > 0);

            return emailAccounts;
        }
    }
}
