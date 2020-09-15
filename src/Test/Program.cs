using Jeffijoe.MessageFormat;
using Jeffijoe.MessageFormat.Helpers;
using System;
using System.Collections.Generic;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(PluralizerHelper.ComputePluralOperands("", "1", 0));
            Console.WriteLine(PluralizerHelper.ComputePluralOperands("", "1.0", 0));
            Console.WriteLine(PluralizerHelper.ComputePluralOperands("", "1.00", 0));
            Console.WriteLine(PluralizerHelper.ComputePluralOperands("", "1.3", 0));
            Console.WriteLine(PluralizerHelper.ComputePluralOperands("", "1.30", 0));
            Console.WriteLine(PluralizerHelper.ComputePluralOperands("", "1.03", 0));
            Console.WriteLine(PluralizerHelper.ComputePluralOperands("", "1.230", 0));

            var mf = new MessageFormatter();

            var str = "Test {n, plural, one {# item} other {# items}}";

            Console.WriteLine(mf.FormatMessage(str, new Dictionary<string, object>()
            {
                {  "n", "1" }
            }));

            Console.WriteLine(mf.FormatMessage(str, new Dictionary<string, object>()
            {
                {  "n", "1,0" }
            }));

            Console.WriteLine(mf.FormatMessage(str, new Dictionary<string, object>()
            {
                {  "n", "1,1" }
            }));

            Console.WriteLine(mf.FormatMessage(str, new Dictionary<string, object>()
            {
                {  "n", 1.0 }
            }));
        }
    }
}
