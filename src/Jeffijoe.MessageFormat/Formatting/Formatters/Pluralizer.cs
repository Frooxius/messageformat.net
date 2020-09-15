// MessageFormat for .NET
// - Pluralizer.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.
using Jeffijoe.MessageFormat.Helpers;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    ///     Given the specified number, determines what plural form is being used.
    /// </summary>
    /// <param name="operands">Plural operands</param>
    /// <returns>The plural form to use.</returns>
    public delegate string Pluralizer(PluralOperands operands);
}