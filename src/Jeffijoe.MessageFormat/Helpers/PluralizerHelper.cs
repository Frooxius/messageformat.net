using System;
using System.Collections.Generic;
using System.Globalization;
using Jeffijoe.MessageFormat.Helpers;
using System.Text;

namespace Jeffijoe.MessageFormat.Helpers
{
    public static class PluralizerHelper
    {
        public static PluralOperands ComputePluralOperands(string locale, object value, double offset)
        {
            var operands = new PluralOperands();

            var numberString = Convert.ToString(value);

            operands.originalString = numberString;

            operands.culture = CultureCache.GetCultureInfo(locale);
            var separator = operands.culture.NumberFormat.NumberDecimalSeparator;

            int index = -1;

            if (double.TryParse(numberString, NumberStyles.Float, operands.culture, out operands.n))
                index = numberString.IndexOf(separator);
            else if (double.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out operands.n))
            {
                operands.culture = CultureInfo.InvariantCulture;
                separator = operands.culture.NumberFormat.NumberDecimalSeparator;

                index = numberString.IndexOf(separator);
            }
            else if (double.TryParse(numberString, NumberStyles.Float, CultureInfo.CurrentCulture, out operands.n))
            {
                operands.culture = CultureInfo.CurrentCulture;
                separator = operands.culture.NumberFormat.NumberDecimalSeparator;

                index = numberString.IndexOf(separator);
            }
            else
                operands.n = 0;

            var eIndex = numberString.IndexOf("e", StringComparison.OrdinalIgnoreCase);

            if(eIndex >= 0)
            {
                var eSubstr = numberString.Substring(eIndex + 1);
                int.TryParse(numberString, NumberStyles.Any, operands.culture, out operands.e);
            }

            operands.originalNumber = operands.n;
            operands.offset = offset;

            operands.n -= offset;
            operands.i = (int)operands.n;

            if (index >= 0)
            {
                var trimStart = index + separator.Length;

                operands.v = numberString.Length - trimStart;

                // trim leading zeroes

                while (numberString[trimStart] == '0' && trimStart < (numberString.Length - 1))
                    trimStart++;

                operands.f = numberString.Substring(trimStart);

                int trimZeroes = 0;

                for (int ch = operands.f.Length - 1; ch >= 0; ch--)
                {
                    if (operands.f[ch] == '0')
                        trimZeroes++;
                    else
                        break;
                }

                if (trimZeroes == 0)
                {
                    operands.t = operands.f;
                    operands.w = operands.v;
                }
                else
                {
                    operands.t = operands.f.Substring(0, operands.f.Length - trimZeroes);
                    operands.w = operands.t.Length;
                }
            }
            else
            {
                operands.v = 0;
                operands.w = 0;
                operands.f = "";
                operands.t = "";
            }

            if (operands.f == "")
                operands.f = "0";

            if (operands.t == "")
                operands.t = "0";

            return operands;
        }
    }
}
