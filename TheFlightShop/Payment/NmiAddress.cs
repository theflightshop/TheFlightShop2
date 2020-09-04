using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheFlightShop.Payment
{
    public class NmiAddress
    {
        [XmlElement("first-name")]
        public string FirstName { get; set; }

        [XmlElement("last-name")]
        public string LastName { get; set; }

        [XmlElement("phone")]
        public string PhoneNumber { get; set; }

        [XmlElement("email")]
        public string EmailAddress { get; set; }

        [XmlElement("company")]
        public string CompanyName { get; set; }

        [XmlElement("address1")]
        public string Address1 { get; set; }

        [XmlElement("address2")]
        public string Address2 { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("state")]
        public string State { get; set; }

        [XmlElement("postal")]
        public string PostalCode { get; set; }

        [XmlElement("country")]
        public string CountryCode { get; set; }
    }
}
