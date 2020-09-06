using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public class CheckoutSubmissionViewModel
    {
        public bool Succeeded { get; }
        public string ConfirmationNumber { get; }
        public string ErrorReason { get; }

        private CheckoutSubmissionViewModel(bool succeeded, string confirmationNumber, string errorMessage)
        {
            Succeeded = succeeded;
            ConfirmationNumber = confirmationNumber;
            ErrorReason = errorMessage;
        }

        public static CheckoutSubmissionViewModel Success(string confirmationNumber) => new CheckoutSubmissionViewModel(succeeded: true, confirmationNumber: confirmationNumber, errorMessage: null);
        public static CheckoutSubmissionViewModel Failure(string errorMessage) => new CheckoutSubmissionViewModel(succeeded: false, confirmationNumber: null, errorMessage: errorMessage);
    }
}
