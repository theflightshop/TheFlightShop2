using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheFlightShop.Payment
{
    [XmlRoot("complete-action")]
    public class NmiPaymentAuth
    {
        [XmlElement("api-key")]
        public string ApiKey { get; set; }

        [XmlElement("token-id")]
        public string TokenId { get; set; }
    }
}
