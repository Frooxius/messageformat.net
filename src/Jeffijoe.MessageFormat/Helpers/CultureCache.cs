using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace Jeffijoe.MessageFormat.Helpers
{
    public static class CultureCache
    {
        private static ConcurrentDictionary<string, CultureInfo> cultures = new ConcurrentDictionary<string, CultureInfo>();

        /// <summary>
        /// Get and cache the culture for a locale.
        /// </summary>
        /// <param name="locale">Locale for which to get the culture.</param>
        /// <returns>
        /// Culture of locale.
        /// </returns>
        public static CultureInfo GetCultureInfo(string locale)
        {
            if (!cultures.ContainsKey(locale))
            {
                try
                {
                    cultures[locale] = new CultureInfo(locale);
                }
                catch (CultureNotFoundException)
                {
                    // fallback to invariant culture
                    cultures[locale] = CultureInfo.InvariantCulture;
                }
            }
            return cultures[locale];
        }
    }
}
