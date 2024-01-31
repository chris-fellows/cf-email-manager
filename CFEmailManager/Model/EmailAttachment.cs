using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
