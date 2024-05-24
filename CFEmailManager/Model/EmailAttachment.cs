using System;
using System.Xml.Serialization;

namespace CFEmailManager.Model
{
    [XmlType("EmailAttachment")]
    public class EmailAttachment
    {
        [XmlAttribute("ID")]
        public Guid ID { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}
