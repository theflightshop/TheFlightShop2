﻿using System;
using System.Collections.Generic;

namespace TheFlightShop.Models
{
    public class ClientOrder
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int ShippingType { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Notes { get; set; }
        public IEnumerable<ClientOrderLine> OrderLines { get; set; }

        private const int CONF_NR_RANDOM_CHARS_LENGTH = 4;
        private string _confirmationNumber;
        public string ConfirmationNumber
        {
            get
            {
                if (_confirmationNumber == null)
                {
                    _confirmationNumber = GenerateConfirmationNumber();
                }
                return _confirmationNumber;
            }
        }

        private string GenerateConfirmationNumber()
        {
            var possibleCharacters = new char[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 
                'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            var now = DateTime.UtcNow;

            var confirmationNumber = now.Year.ToString() + possibleCharacters[now.Month].ToString() + possibleCharacters[now.Day].ToString() + 
                possibleCharacters[now.Hour].ToString() + now.Minute.ToString("00") + now.Second.ToString("00");

            var random = new Random();
            for (var i = 0; i < CONF_NR_RANDOM_CHARS_LENGTH; i++)
            {
                var nextRandomChar = possibleCharacters[random.Next(possibleCharacters.Length)];
                confirmationNumber += nextRandomChar;
            }

            return confirmationNumber;
        }
    }
}
