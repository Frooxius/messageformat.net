using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    public class NumberFormatter : BaseFormatter, IFormatter
    {
        public bool CanFormat(FormatterRequest request)
        {
            return request.FormatterName == "number";
        }

        public string Format(string locale, FormatterRequest request, IDictionary<string, object> args, object value, IMessageFormatter messageFormatter)
        {
            if (string.Equals("percent", request.FormatterArguments))
            {
                var n = Convert.ToDouble(value);
                n *= 100;
                var rounded = Math.Round(n);

                return $"{rounded}%";
            }
            else if (string.Equals("integer", request.FormatterArguments))
            {
                return Convert.ToInt64(value).ToString();
            }
            else if (string.Equals("currency", request.FormatterArguments))
            {
                // TODO!!! Handle currency specifier (e.g. currency:GBP)
                // TODO!!! Handle locale instead of current culture?

                var n = Convert.ToDouble(value);
                return n.ToString("C");
            }
            else
                throw new FormatException("Unsupported number formatter argument: " + request.FormatterArguments);
        }
    }
}
