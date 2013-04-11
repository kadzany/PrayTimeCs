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
            PrayTimeCs.PrayTimeCs prayTimeCs = new PrayTimeCs.PrayTimeCs("Egypt");
            prayTimeCs.GetTimes(DateTime.Now, -6.910237756030072, 107.59923934936523, 7, 0);
        }
    }
}
