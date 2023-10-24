using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.Helpers.SpirePriceTracker
{
    internal static class SpirePriceTracker
    {
        private static int CurrentMonth = DateTime.Now.Month;
        private static double WinterBillingRate = 0.36538;
        private static double SummerBillingRateFirst50Ccf = 0.32877;
        private static double SummerBillingRateOver50Ccf = 0.39835;

        public static double GetCurrentPricePerCcf(int currentUsage)
        {
            if (CurrentMonth > 10 || CurrentMonth < 5)
            {
                return WinterBillingRate;
            }
            else if (CurrentMonth > 4 && CurrentMonth < 11)
            {
                if (currentUsage < 50)
                    return SummerBillingRateFirst50Ccf;
                else
                    return SummerBillingRateOver50Ccf;
            }
            return 0;
        }
    }
}
