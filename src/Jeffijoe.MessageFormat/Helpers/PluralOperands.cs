using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jeffijoe.MessageFormat.Helpers
{
    // Plural operands as defined in https://unicode.org/reports/tr35/tr35-numbers.html#Language_Plural_Rules
    public struct PluralOperands
    {
        public double n; // absolute value of the source number (integer and decimals).
        public int i;    // integer digits of n.
        public int v;    // number of visible fraction digits in n, with trailing zeros.
        public int w;    // number of visible fraction digits in n, without trailing zeros.
        public string f; // visible fractional digits in n, with trailing zeros.
        public string t; // visible fractional digits in n, without trailing zeros.

        // Extra parameters
        public string originalString;
        public double offset;
        public double originalNumber;
        public CultureInfo culture;

        public string ReconstructWithOffset()
        {
            if (offset == 0)
                return originalString;

            return n.ToString("F" + v, culture);
        }

        public override string ToString() => $"n: {originalString}\t\ti: {i}\tv: {v}\tw: {w}\tf: {f}\tt: {t}";
    }
}
