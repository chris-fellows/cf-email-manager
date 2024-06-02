using System;
using System.Xml.Serialization;

namespace CFEmailManager.Model
{

    [XmlType("EmailAddress")]
    public class EmailAddress
    {
        public string Address { get; set; }
    }
}
