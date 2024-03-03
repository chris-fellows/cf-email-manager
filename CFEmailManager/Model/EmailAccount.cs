using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFEmailManager.Model
{
    public class EmailAccount
    {
        public string Server { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string LocalFolder { get; set; }

        public string ServerType { get; set; }
    }
}
