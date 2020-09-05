using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheFlightShop.Payment
{
    public class NmiLineItem
    {
        [XmlElement("product-code")]
        public string PartNumber { get; set; }

        [XmlElement("description")]
        public string ProductId { get; set; }

        [XmlElement("quantity")]
        public decimal Quantity { get; set; }
    }
}
