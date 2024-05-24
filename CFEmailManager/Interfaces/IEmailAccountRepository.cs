using System;
using System.Collections.Generic;
using CFEmailManager.Model;

namespace CFEmailManager.Interfaces
{
    public interface IEmailAccountRepository
    {
        List<EmailAccount> GetAll();
    }
}
