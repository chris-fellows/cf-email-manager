using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CFEmailManager.Model
{
    [XmlType("Email")]
    public class EmailObject
    {
        [XmlAttribute("ID")]
        public Guid ID { get; set; }

        [XmlAttribute("FolderID")]
        public Guid FolderID { get; set; }

        [XmlAttribute("Subject")]
        public string Subject { get; set; }

        [XmlAttribute("Priority")]
        public string Priority { get; set; }

        [XmlElement("From")]
        public EmailAddress From { get; set; }

        [XmlArray("To")]
        [XmlArrayItem("Item")]
        public List<EmailAddress> To { get; set; }

        [XmlArray("CC")]
        [XmlArrayItem("Item")]
        public List<EmailAddress> CC { get; set; }

        [XmlArray("BCC")]
        [XmlArrayItem("Item")]
        public List<EmailAddress> BCC { get; set; }

        [XmlArray("ReplyTo")]
        [XmlArrayItem("Item")]
        public List<EmailAddress> ReplyTo { get; set; }

        [XmlIgnore]     // Cannot XML serialize a DateTimeOffset
        public DateTimeOffset SentDate { get; set; }

        [XmlElement("SentDate")]
        public string SentDateForForXml // format: 2011-11-11T15:05:46.4733406+01:00
        {
            get { return SentDate.ToString("o"); } // o = yyyy-MM-ddTHH:mm:ss.fffffffzzz
            set { SentDate = DateTimeOffset.Parse(value); }
        }

        [XmlIgnore]    // Cannot XML serialize a DateTimeOffset
        public DateTimeOffset ReceivedDate { get; set; }

        [XmlElement("ReceivedDate")]
        public string ReceivedDateForXml // format: 2011-11-11T15:05:46.4733406+01:00
        {
            get { return ReceivedDate.ToString("o"); } // o = yyyy-MM-ddTHH:mm:ss.fffffffzzz
            set { ReceivedDate = DateTimeOffset.Parse(value); }
        }

        /// <summary>
        /// Whether email exists on server. Used for reporting.
        /// </summary>
        [XmlAttribute("ExistsOnServer")]
        public bool ExistsOnServer { get; set; }

        [XmlArray("Attachments")]
        [XmlArrayItem("Attachment")]
        public List<EmailAttachment> Attachments;
    }
}
