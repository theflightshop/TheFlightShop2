using System.Collections.Generic;

namespace TheFlightShop.Models
{
    public class ShippingType
    {
        public static readonly ShippingType UpsGround = new ShippingType(0, "UPS Ground");
        public static readonly ShippingType UpsRed = new ShippingType(1, "UPS-Red");
        public static readonly ShippingType UpsTwoDay = new ShippingType(2, "UPS 2-day");
        public static readonly ShippingType UpsThreeDay = new ShippingType(3, "UPS 3-day");
        public static readonly ShippingType FedExGround = new ShippingType(4, "Fed Ex. Ground");
        public static readonly ShippingType FedExP2 = new ShippingType(5, "Fed Ex. P2");
        public static readonly ShippingType FedExP1= new ShippingType(6, "Fed Ex. P1");
        public static readonly ShippingType Other = new ShippingType(7, "Other");

        public int Value { get; }
        public string Name { get; }

        private static readonly Dictionary<int, ShippingType> _shippingTypes = new Dictionary<int, ShippingType>
        {
            { 0, UpsGround },
            { 1, UpsRed },
            { 2, UpsTwoDay },
            { 3, UpsThreeDay },
            { 4, FedExGround },
            { 5, FedExP2 },
            { 6, FedExP1 },
            { 7, Other }
        };

        private ShippingType(int value, string name)
        {
            Value = value;
            Name = name;
        }

        public static explicit operator int(ShippingType shippingType)
        {
            return shippingType.Value;
        }

        public static explicit operator ShippingType(int shippingTypeValue)
        {
            return _shippingTypes.ContainsKey(shippingTypeValue) ? _shippingTypes[shippingTypeValue] : null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
