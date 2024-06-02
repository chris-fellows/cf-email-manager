using System;
using System.Collections.Generic;
using CFEmailManager.Model;

namespace CFEmailManager.Interfaces
{
    /// <summary>
    /// Service for list of email accounts
    /// </summary>
    public interface IEmailAccountService
    {
        List<EmailAccount> GetAll();
    }
}
