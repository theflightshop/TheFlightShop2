﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Payment
{
    /// <summary>
    /// Names of custom XML fields sent/returned for NMI's Three Step Redirect API.
    /// </summary>
    public class NmiCustomXmlField
    {
        public const string ATTENTION_TO = "merchant-defined-field-1";
        public const string SHIPPING_TYPE = "merchant-defined-field-2";
    }
}
