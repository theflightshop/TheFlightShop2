using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheFlightShop.Payment
{
    /// <summary>
    /// Represents body sent in Step 1, and returned from Step 3, of NMI's Three Step Redirect API, which is documented at 
    /// https://secure.nmi.com/merchants/resources/integration/integration_portal.php#3step_methodology 
    /// as of this writing. 
    /// </summary>
    public class NmiCustomerInfo
    {
        /// <summary>
        /// Only sent in Step 1. Isn't returned from Step 3.
        /// </summary>
        [XmlElement("api-key")]
        public string ApiKey { get; set; }

        /// <summary>
        /// Only sent in Step 1. Isn't returned from Step 3.
        /// </summary>
        [XmlElement("redirect-url")]
        public string RedirectUrl { get; set; }

        [XmlElement("amount")]
        public decimal Amount { get; set; }

        [XmlElement("order-id")]
        public string ConfirmationNumber { get; set; }

        [XmlElement("billing")]
        public NmiAddress BillingAddress { get; set; }
    }
}
