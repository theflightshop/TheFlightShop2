using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop
{
    /// <summary>
    /// Not globally unique. Instead, meant to be easy to read. Should be unique up to 1 error per second, and then is still likely to be unique up to 1 error per millisecond.
    /// </summary>
    public class PseudoUniqueId
    {
        private const int NUMBER_OF_RANDOM_CHARACTERS = 4;

        public string Next()
        {
            var possibleCharacters = new char[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R',
                'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            var now = DateTime.UtcNow;

            var pseudoUniqueId = now.Year.ToString() + possibleCharacters[now.Month].ToString() + possibleCharacters[now.Day].ToString() +
                possibleCharacters[now.Hour].ToString() + now.Minute.ToString("00") + now.Second.ToString("00");

            var random = new Random();
            for (var i = 0; i < NUMBER_OF_RANDOM_CHARACTERS; i++)
            {
                var nextRandomChar = possibleCharacters[random.Next(possibleCharacters.Length)];
                pseudoUniqueId += nextRandomChar;
            }

            return pseudoUniqueId;
        }
    }
}
