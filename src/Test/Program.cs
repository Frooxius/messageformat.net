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

            var str = "{gender_of_host, select, " +
  "female {" +
    "{num_guests, plural, offset:1 " +
      "=0 {{host} does not give a party.}" +
      "=1 {{host} invites {guest} to her party.}" +
      "=2 {{host} invites {guest} and one other person to her party.}" + 
      "other {{host} invites {guest} and # other people to her party.}}}" +
  "male {" +
    "{num_guests, plural, offset:1 " +
      "=0 {{host} does not give a party.}" +
      "=1 {{host} invites {guest} to his party.}" +
      "=2 {{host} invites {guest} and one other person to his party.}" +
      "other {{host} invites {guest} and # other people to his party.}}}" +
  "other {" +
    "{num_guests, plural, offset:1 " + 
      "=0 {{host} does not give a party.}" + 
      "=1 {{host} invites {guest} to their party.}" + 
      "=2 {{host} invites {guest} and one other person to their party.}" + 
      "other {{host} invites {guest} and # other people to their party.}}}}";

            for (int i = 0; i < 4; i++)
            {

                var formatted = mf.FormatMessage(str, new Dictionary<string, object>{
                  {"gender_of_host", "male"},
                  {"host", "Mr. Potato" },
                  {"guest", "Vanilla" },
                  { "num_guests", i }
                });

                Console.WriteLine(formatted);
            }
        }
    }
}
