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
            PrayTimeCs prayTimeCs = new PrayTimeCs("MWL");
            prayTimeCs.GetTimes(DateTime.Now, 43, -80, -5, 0);
        }
    }
}
