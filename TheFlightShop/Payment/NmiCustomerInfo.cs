using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheFlightShop.Payment
{
    /// <summary>
    /// Represents body sent in Step 1 of NMI's Three Step Redirect API, which is documented at 
    /// https://secure.nmi.com/merchants/resources/integration/integration_portal.php#3step_methodology 
    /// as of this writing. 
    /// </summary>
    public class NmiCustomerInfo
    {
        [XmlElement("api-key")]
        public string ApiKey { get; set; }

        [XmlElement("redirect-url")]
        public string RedirectUrl { get; set; }

        [XmlElement("amount")]
        public decimal Amount { get; set; }

        [XmlElement("billing")]
        public NmiAddress BillingAddress { get; set; }
    }
}
