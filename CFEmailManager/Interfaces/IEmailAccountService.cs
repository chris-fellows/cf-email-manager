using System;
using System.Collections.Generic;
using CFEmailManager.Model;
using CFUtilities.Repository;

namespace CFEmailManager.Interfaces
{
    /// <summary>
    /// Service for list of email accounts
    /// </summary>
    public interface IEmailAccountService : IItemRepository<EmailAccount, string>
    {
        List<EmailAccount> GetAll();
    }
}
