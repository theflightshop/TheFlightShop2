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

        [XmlElement("po-number")]
        public string PurchaseOrderNumber { get; set; }

        [XmlElement(NmiCustomXmlField.ATTENTION_TO)]
        public string AttentionTo { get; set; }

        [XmlElement(NmiCustomXmlField.CUSTOMER_NOTES)]
        public string Notes { get; set; }

        [XmlElement(NmiCustomXmlField.SHIPPING_TYPE)]
        public string ShippingType { get; set; }

        [XmlElement(NmiCustomXmlField.CUSTOM_SHIPPING_TYPE_VALUE)]
        public string CustomShippingType { get; set; }

        [XmlElement("billing")]
        public NmiAddress BillingAddress { get; set; }

        [XmlElement("shipping")]
        public NmiAddress ShippingAddress { get; set; }

        [XmlElement("product")]
        public NmiLineItem[] LineItems { get; set; }
    }
}
