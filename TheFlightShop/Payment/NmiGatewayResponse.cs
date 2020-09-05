using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheFlightShop.Models;

namespace TheFlightShop.Payment
{
    /// <summary>
    /// Represents response body returned from Steps 1 and 3 of NMI's Three Step Redirect API, which is documented at 
    /// https://secure.nmi.com/merchants/resources/integration/integration_portal.php#3step_methodology 
    /// as of this writing. 
    /// </summary>
    [XmlRoot("response")]
    public class NmiGatewayResponse
    {
        [XmlIgnore]
        public bool Succeeded => GetResponseStatus() == NmiGatewayResponseStatus.Approved;

        [XmlIgnore]
        public string ErrorReason
        {
            get
            {
                var status = GetResponseStatus();
                var reason = (string)null;
                if (status == NmiGatewayResponseStatus.Declined)
                {
                    reason = "Your card was declined.";
                }
                else if (status == NmiGatewayResponseStatus.Error)
                {
                    reason = ResultText;
                }
                return reason;
            }
        }

        [XmlElement("result")]
        public string Result { get; set; }

        [XmlElement("result-text")]
        public string ResultText { get; set; }

        [XmlElement("result-code")]
        public string ResultCode { get; set; }

        [XmlElement("transaction-id")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Returned from Step 1 of NMI's Three Step Redirect API.
        /// </summary>
        [XmlElement("form-url")]
        public string PaymentAuthFormUrl { get; set; }

        /// <summary>
        /// Returned from Step 3 of NMI's Three Step Redirect API. 
        /// ***WARNING: These elements appear to only be returned on successful response i.e. <see cref="NmiGatewayResponseStatus.Approved"/>, as of Sep 5, 2020.
        /// </summary>
        #region NMI Redirect API Step 3

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

        #endregion

        public override string ToString()
        {
            return $"TransactionId={TransactionId},Result={Result},ResultText={ResultText},ResultCode={ResultCode},PaymentAuthFormUrl={PaymentAuthFormUrl}";
        }

        public NmiGatewayResponseStatus? GetResponseStatus()
        {
            return Enum.TryParse(Result, out NmiGatewayResponseStatus status) ? status : (NmiGatewayResponseStatus?)null;
        }
    }
}
