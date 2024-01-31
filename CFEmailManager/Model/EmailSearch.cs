using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CFEmailManager.Model
{
    public class EmailSearch
    {
        /// <summary>
        /// Min received date
        /// </summary>
        public DateTime MinReceivedDate { get; set; }

        /// <summary>
        /// Max received date
        /// </summary>
        public DateTime MaxReceivedDate { get; set; }

        /// <summary>
        /// Whether email exists on server
        /// </summary>
        public bool? ExistsOnServer { get; set; }

        /// <summary>
        /// Text to find (Sender, subject, body, attachments)
        /// </summary>
        public string TextToFind { get; set; }

        /// <summary>
        /// Whether to search attachments
        /// </summary>
        public bool SearchAttachments { get; set; }

        public bool IsMatches(EmailObject emailObject, EmailFolder emailFolder)
        {
            bool isMatches = true;
            if (isMatches && emailObject.ReceivedDate < this.MinReceivedDate)
            {
                isMatches = false;
            }
            if (isMatches && emailObject.ReceivedDate > this.MaxReceivedDate)
            {
                isMatches = false;
            }
           
            if (ExistsOnServer != null && emailObject.ExistsOnServer != this.ExistsOnServer.Value)
            {
                isMatches = false;
            }

            // Check text to find
            if (!String.IsNullOrEmpty(this.TextToFind))
            {                
                int countMatches = 0;

                if (countMatches == 0 && emailObject.SenderAddress.ToLower().Contains(this.TextToFind.ToLower()))
                {
                    countMatches++;
                }

                if (countMatches == 0 && emailObject.Subject.ToLower().Contains(this.TextToFind.ToLower()))
                {
                    countMatches++;
                }

                // Check attachments
                if (countMatches == 0 && emailObject.Attachments != null && this.SearchAttachments)
                {
                    foreach(var attachment in emailObject.Attachments)
                    {
                        string attachmentFile = Path.Combine(emailFolder.LocalFolder, string.Format("Attachment.{0}.{1}", emailObject.ID, attachment.ID));
                        if (File.Exists(attachmentFile))
                        {
                            string attachmentBody = File.ReadAllText(attachmentFile);
                            if (attachmentBody.ToLower().Contains(this.TextToFind.ToLower()))
                            {
                                countMatches++;
                            }
                        }
                        if (countMatches > 0)
                        {
                            break;
                        }
                    }
                }
              
                string emailFile = Path.Combine(emailFolder.LocalFolder, string.Format("{0}.eml", emailObject.ID));
                if (countMatches == 0 && File.Exists(emailFile))
                {
                    string emailBody = File.ReadAllText(emailFile);
                    if (emailBody.ToLower().Contains(this.TextToFind.ToLower()))
                    {
                        countMatches++;
                    }
                }

                isMatches = (countMatches > 0);
            }

            return isMatches;
        }
    }
}
