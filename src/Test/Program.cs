using Jeffijoe.MessageFormat;
using System;
using System.Collections.Generic;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var mf = new MessageFormatter();

            var str = "Percetange {n, number,  percent   }";

            var formatted = mf.FormatMessage(str, new Dictionary<string, object>()
            {
                {  "n", 0.8 }
            });

            Console.WriteLine(formatted);
        }
    }
}
