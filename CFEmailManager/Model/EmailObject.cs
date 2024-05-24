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

        [XmlAttribute("SenderAddress")]
        public string SenderAddress { get; set; }

        [XmlAttribute("SentDate")]
        public DateTime SentDate { get; set; }

        [XmlAttribute("ReceivedDate")]
        public DateTime ReceivedDate { get; set; }

        [XmlAttribute("ExistsOnServer")]
        public bool ExistsOnServer { get; set; }

        [XmlArray("Attachments")]
        [XmlArrayItem("Attachment")]
        public List<EmailAttachment> Attachments;
    }
}
