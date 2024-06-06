using CFEmailManager.Interfaces;
using CFEmailManager.Model;
using CFUtilities.XML;

namespace CFEmailManager
{
    /// <summary>
    /// Email account service
    /// </summary>
    public class EmailAccountService : XmlItemRepository<EmailAccount, string>, IEmailAccountService
    {
        public EmailAccountService(string folder) : base(folder, (EmailAccount account) => account.ID)
        {

        }
    }
}
