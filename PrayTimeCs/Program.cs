using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrayTimeCs
{
    class Program
    {
        static void Main(string[] args)
        {
            PrayTimeCs prayTimeCs = new PrayTimeCs("Egypt");
            prayTimeCs.GetTimes(DateTime.Now, -6.910237756030072, 107.59923934936523, 7, 0);
        }
    }
}
