﻿using System;
using System.Xml.Serialization;

namespace CFEmailManager.Model
{
    [XmlType("EmailFolder")]
    public class EmailFolder
    {
        [XmlAttribute("ID")]
        public Guid ID { get; set; }

        [XmlAttribute("ParentFolderID")]
        public Guid ParentFolderID { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }          

        [XmlAttribute("SyncEnabled")]
        public bool SyncEnabled { get; set; }

        /// <summary>
        /// Whether the folder exists on the server
        /// </summary>
        [XmlAttribute("ExistsOnServer")]
        public bool ExistsOnServer { get; set; }
    }
}
