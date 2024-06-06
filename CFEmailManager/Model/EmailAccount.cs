using System;

namespace CFEmailManager.Model
{
    /// <summary>
    /// Email account details
    /// </summary>
    public class EmailAccount
    {
        public string ID { get; set; }

        public string Server { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string LocalFolder { get; set; }

        public string ServerType { get; set; }

        public DateTimeOffset TimeLastDownload { get; set; }
    }
}
