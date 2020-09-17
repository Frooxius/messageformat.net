// MessageFormat for .NET
// - PluralFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using Jeffijoe.MessageFormat.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    ///     Plural Formatter
    /// </summary>
    public class PluralFormatter : BaseFormatter, IFormatter
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PluralFormatter" /> class.
        /// </summary>
        public PluralFormatter()
        {
            this.Pluralizers = new Dictionary<string, Pluralizer>();
            this.AddStandardPluralizers();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the pluralizers dictionary. Key is the locale.
        /// </summary>
        /// <value>
        ///     The pluralizers.
        /// </value>
        public IDictionary<string, Pluralizer> Pluralizers { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether this instance can format a message based on the specified parameters.
        /// </summary>
        /// <param name="request">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CanFormat(FormatterRequest request)
        {
            return request.FormatterName == "plural";
        }

        /// <summary>
        ///     Using the specified parameters and arguments, a formatted string shall be returned.
        ///     The <see cref="IMessageFormatter" /> is being provided as well, to enable
        ///     nested formatting. This is only called if <see cref="CanFormat" /> returns true.
        ///     The args will always contain the <see cref="FormatterRequest.Variable" />.
        /// </summary>
        /// <param name="locale">
        ///     The locale being used. It is up to the formatter what they do with this information.
        /// </param>
        /// <param name="request">
        ///     The parameters.
        /// </param>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <param name="value">The value of <see cref="FormatterRequest.Variable"/> from the given args dictionary. Can be null.</param>
        /// <param name="messageFormatter">
        ///     The message formatter.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string Format(
            string locale,
            FormatterRequest request,
            IDictionary<string, object> args,
            object value,
            IMessageFormatter messageFormatter)
        {
            var arguments = this.ParseArguments(request);
            double offset = 0;

            var offsetExtension = arguments.Extensions.FirstOrDefault(x => x.Extension == "offset");

            if (offsetExtension != null)
                offset = Convert.ToDouble(offsetExtension.Value);

            var operands = PluralizerHelper.ComputePluralOperands(locale, value, offset);

            var pluralized = new StringBuilder(this.Pluralize(locale, arguments, operands));
            var result = this.ReplaceNumberLiterals(pluralized, operands.ReconstructWithOffset());
            var formatted = messageFormatter.FormatMessage(result, args);
            return formatted;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns the correct plural block.
        /// </summary>
        /// <param name="locale">
        ///     The locale.
        /// </param>
        /// <param name="arguments">
        ///     The parsed arguments string.
        /// </param>
        /// <param name="n">
        ///     The n.
        /// </param>
        /// <param name="offset">
        ///     The offset.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <exception cref="MessageFormatterException">
        ///     The 'other' option was not found in pattern.
        /// </exception>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        internal string Pluralize(string locale, ParsedArguments arguments, PluralOperands operands)
        {
            if (!this.Pluralizers.TryGetValue(locale, out Pluralizer pluralizer))
                pluralizer = this.Pluralizers["en"];

            var pluralForm = pluralizer(operands);
            KeyedBlock other = null;
            foreach (var keyedBlock in arguments.KeyedBlocks)
            {
                if (keyedBlock.Key == OtherKey)
                {
                    other = keyedBlock;
                }

                if (keyedBlock.Key.StartsWith("="))
                {
                    var numberLiteral = Convert.ToDouble(keyedBlock.Key.Substring(1));

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (numberLiteral == operands.originalNumber)
                    {
                        return keyedBlock.BlockText;
                    }
                }

                if (keyedBlock.Key == pluralForm)
                {
                    return keyedBlock.BlockText;
                }
            }

            if (other == null)
            {
                throw new MessageFormatterException("'other' option not found in pattern.");
            }

            return other.BlockText;
        }

        /// <summary>
        ///     Replaces the number literals with the actual number.
        /// </summary>
        /// <param name="pluralized">
        ///     The pluralized.
        /// </param>
        /// <param name="n">
        ///     The n.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal string ReplaceNumberLiterals(StringBuilder pluralized, string n)
        {
            // I've done this a few times now..
            const char OpenBrace = '{';
            const char CloseBrace = '}';
            const char Pound = '#';
            const char EscapeChar = '\'';
            var braceBalance = 0;
            var insideEscapeSequence = false;
            var sb = new StringBuilder();
            for (int i = 0; i < pluralized.Length; i++)
            {
                var c = pluralized[i];

                if (c == EscapeChar)
                {
                    sb.Append(EscapeChar);

                    if (i == pluralized.Length - 1)
                    {
                        // The last char can't open a new escape sequence, it can only close one
                        if (insideEscapeSequence)
                        {
                            insideEscapeSequence = false;
                        }
                        continue;
                    }

                    var nextChar = pluralized[i + 1];
                    if (nextChar == EscapeChar)
                    {
                        sb.Append(EscapeChar);
                        ++i;
                        continue;
                    }

                    if (insideEscapeSequence)
                    {
                        insideEscapeSequence = false;
                        continue;
                    }

                    if (nextChar == '{' || nextChar == '}' || nextChar == '#')
                    {
                        sb.Append(nextChar);
                        insideEscapeSequence = true;
                        ++i;
                        continue;
                    }

                    continue;
                }

                if (insideEscapeSequence)
                {
                    sb.Append(c);
                    continue;
                }

                if (c == OpenBrace)
                {
                    braceBalance++;
                }
                else if (c == CloseBrace)
                {
                    braceBalance--;
                }
                else if (c == Pound)
                {
                    if (braceBalance == 0)
                    {
                        sb.Append(n);
                        continue;
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Adds the standard pluralizers.
        /// </summary>
        private void AddStandardPluralizers()
        {
            // Implementations based on https://github.com/unicode-org/cldr/blob/master/common/supplemental/plurals.xml

            AddPluralizer(o => "other", "bm bo dz id ig ii in ja jbo jv jw kde kea km ko lkt lo ms my nqo osa root sah ses sg su th to vi wo yo yue zh");

            AddPluralizer(o => (o.i == 1 && o.v == 0) ? "one" : "other", "ast ca de en et fi fy gl ia io it ji lij nl pt_PT sc scn sv sw ur yi");

            AddPluralizer(o => o.n == 1 ? "one" : "other", "af an asa az bem bez bg brx ce cgg chr ckb dv ee el eo es eu fo fur gsw ha haw hu jgo jmc ka kaj kcg kk kkj kl ks ksb ku ky lb lg mas mgo ml mn mr nah nb nd ne nn nnh no nr ny nyn om or os pap ps rm rof rwk saq sd sdh seh sn so sq ss ssy st syr ta te teo tig tk tn tr ts ug uz ve vo vun wae xh xog");

            AddPluralizer(o =>
            {
                if (o.i == 1 && o.v == 0)
                    return "one";

                if (o.i >= 2 && o.i <= 4 && o.v == 0)
                    return "few";

                if (o.v != 0)
                    return "many";

                return "other";

            }, "cs sk");

            AddPluralizer(o =>
            {
                if ((o.t == "0" && o.i % 10 == 1 && o.i % 100 != 11) || o.t != "0")
                    return "one";

                return "other";

            }, "is");

            AddPluralizer(o =>
            {
                var mod10 = o.i % 10;
                var mod100 = o.i % 100;

                if (o.v == 0 && mod10 == 1 && mod100 != 11)
                    return "one";

                if (o.v == 0 && mod10 >= 2 && mod10 <= 4 && !(mod100 >= 12 && mod100 <= 14))
                    return "few";

                if ((o.v == 0 && mod10 == 0) || (o.v == 0 && mod10 >= 5 && mod10 <= 9) || (o.v == 0 && mod100 >= 11 && mod10 <= 14))
                    return "many";

                return "other";
            }, "ru uk");
        }

        void AddPluralizer(Pluralizer pluralizer, string locales)
        {
            foreach (var locale in locales.Split(' '))
                if (!string.IsNullOrWhiteSpace(locale))
                    this.Pluralizers.Add(locale.Trim(), pluralizer);
        }

        #endregion
    }
}