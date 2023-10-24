using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.Helpers.EvergyPriceTracker
{
    public static class EvergyPriceDictionary
    {
        public enum Season
        {
            Summer = 1,
            NonSummer = 2
        }

        public static decimal GetCurrentRate(int kilowattHoursUsed)
        {
            Season season;
            // Get the current time of day.
            var now = DateTime.Now;

            season = isSummer(now) ? Season.Summer : Season.NonSummer;

            var timeOfDay = now.Hour;

            // Calculate the peak adjustment charge.
            var peakAdjustmentCharge = 0m;
            if (timeOfDay >= 16 && timeOfDay <= 20 && isSummer(now))
                peakAdjustmentCharge = 0.01m;
            else if (timeOfDay >= 16 && timeOfDay <= 20 && !isSummer(now))
                peakAdjustmentCharge = 0.0025m;
            else if (timeOfDay >= 1 && timeOfDay <= 6)
                peakAdjustmentCharge = -0.01m;

            // Calculate the energy charge.
            var energyCharge = 0m;
            switch (season)
            {
                case Season.Summer:
                    if (kilowattHoursUsed <= 600)
                    {
                        energyCharge = 0.11829m;
                    }
                    else if (kilowattHoursUsed <= 1000)
                    {
                        energyCharge =  0.11829m;
                    }
                    else
                    {
                        energyCharge = 0.12829m;
                    }
                    break;
                case Season.NonSummer:
                    if (kilowattHoursUsed <= 600)
                    {
                        energyCharge = 0.09784m;
                    }
                    else if (kilowattHoursUsed <= 1000)
                    {
                        energyCharge = 0.09784m;
                    }
                    else
                    {
                        energyCharge = 0.07718m;
                    }
                    break;
            }

            // Return the total charge.
            return energyCharge + peakAdjustmentCharge;
        }

        private static bool isSummer(DateTime now)
        {
            switch (now.Month)
            {
                case 6:
                case 7:
                case 8:
                case 9:
                    return true;
                default:
                    return false;


            }
        }
    }
}
