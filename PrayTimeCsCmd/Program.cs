using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrayTimeCsCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, DateTime> result = new Dictionary<string, DateTime>();
            PrayTimeCs.PrayTimeCs prayTimeCs = new PrayTimeCs.PrayTimeCs("Egypt");
            result = prayTimeCs.GetTimes(DateTime.Now, -6.910237756030072, 107.59923934936523, 7, 0);
            foreach (var r in result)
            {
                if (DateTime.Now < r.Value)
                {
                    Console.WriteLine("Next Prayer is: {0} ({1})", r.Key, r.Value.ToShortTimeString());
                    break;
                }
            }
        }
    }
}
