using ChandlerHome.apps.HassModel.FrontOfHouse.Lighting.LightRoutines;
using ChandlerHome.Models.ResponseModels;
using Serilog;

namespace ChandlerHome.apps.HassModel.FrontOfHouse.Lighting;

[NetDaemonApp(Id = "Front of House Lights")]
internal class FrontOfHouseLights : LightPatterns
{

    public FrontOfHouseLights(IHaContext ha, IScheduler scheduler) : base(ha, scheduler)
    {
        _entities ??= new Entities(ha);

        scheduler.ScheduleCron("0 2 * * *", () => _entities.Light.FrontOfHouseLights.TurnOff());

        var sun = _entities.Sun.Sun;
        var doorLight = _entities.Light.FrontDoorLight;
        var northLight = _entities.Light.FrontOfHouseNorth;
        var southLight = _entities.Light.FrontOfHouseSouth;
        var frontOfHouseLights = _entities.Light.FrontOfHouseLights;

        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(x => x.New.IsOn())
            .Subscribe(x =>
            {
                if (_entities.Light.FrontOfHouseLights.IsOff())
                {
                    FrontLightsRain(doorLight, northLight, southLight);
                }
            });

        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(e => e.New.IsOff() && DateTime.Now.TimeOfDay.Hours > 2)
           .Subscribe(x =>
           {
               TurnOff(_entities.Light.FrontOfHouseLights, 30);
           });

        sun.StateChanges().Where(e => e.New?.State?.Equals("below_horizon", StringComparison.OrdinalIgnoreCase) ?? false)
            .SubscribeAsync(async x =>
            {
                var longWeekend = new List<LongWeekendAPIResponse>();
                var holidays = new List<PublicHolidayAPIResponse>();
                try
                {
                    holidays = await QuerySrv.Instance.GetRequest<List<PublicHolidayAPIResponse>>("https://date.nager.at", $"api/v3/PublicHolidays/{DateTime.Now.Year}/US");
                    longWeekend = await QuerySrv.Instance.GetRequest<List<LongWeekendAPIResponse>>("https://date.nager.at", $"api/v3/LongWeekend/{DateTime.Now.Year}/US");
                }
                catch (Exception ex)
                {
                    Log.Warning(ex.Message + "\n\n" + ex.StackTrace);
                }
                switch (DateTime.Now.Month)
                {
                    case 1:
                        if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                        {
                            ChiefsLights(doorLight, northLight, southLight);
                        }
                        else
                            DefaultFrontLights(doorLight, northLight, southLight);
                        break;
                    case 5:
                        var memorialDayWeekend = longWeekend.Where(x => x.startDate.Month.Equals(5)).FirstOrDefault();
                        if ((memorialDayWeekend?.startDate <= DateTime.Now.Date) && (DateTime.Now.Date <= memorialDayWeekend.endDate))
                        {
                            FrontLightsAmerica(doorLight, northLight, southLight);
                        }
                        else
                            DefaultFrontLights(doorLight, northLight, southLight);
                        break;
                    case 6:
                        if (DateTime.Now.Day.Equals(19))
                            JuneteenthFrontLights(doorLight, northLight, southLight);
                        else
                            await PrideFrontLights(doorLight, northLight, southLight);
                        break;
                    case 7:
                        if (DateTime.Now.Day < 6)
                        {
                            FrontLightsAmerica(doorLight, northLight, southLight);
                        }
                        else
                            DefaultFrontLights(doorLight, northLight, southLight);
                        break;
                    case 9:
                    case 10:
                        if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                        {
                            ChiefsLights(doorLight, northLight, southLight);
                        }
                        else
                            await HalloweenFrontLights(doorLight, northLight, southLight);
                        break;
                    case 11:
                        var laborDay = holidays.Where(x => x.name.Contains("Labor Day", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (DateTime.Now.Day.Equals(laborDay.date.Day))
                            FrontLightsAmerica(doorLight, northLight, southLight);
                        if (DateTime.Now.Day.Equals(11))
                            FrontLightsAmerica(doorLight, northLight, southLight);
                        if (IsItAfterThanksgiving())
                        {
                            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                ChiefsLights(doorLight, northLight, southLight);
                            }
                            else
                            {
                                await ChristmasFrontLights(doorLight, northLight, southLight);
                            }
                            break;
                        }
                        break;
                    case 12:
                        if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                        {
                            ChiefsLights(doorLight, northLight, southLight);
                        }
                        else
                        {
                            if (DateTime.Now.Day < 26)
                            {
                                await ChristmasFrontLights(doorLight, northLight, southLight);
                            }
                            else
                            {
                                DefaultFrontLights(doorLight, northLight, southLight);
                            }
                        }

                        break;
                    default:

                        DefaultFrontLights(doorLight, northLight, southLight);
                        break;
                }
            });

        if (sun.State.Equals("below_horizon", StringComparison.OrdinalIgnoreCase))
        {
            if (frontOfHouseLights.IsOff())
            {
                if (DateTime.Now.Hour < 2)
                {
                    switch (DateTime.Now.Month)
                    {
                        case 1:
                            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                ChiefsLights(doorLight, northLight, southLight);
                            }
                            else
                                DefaultFrontLights(doorLight, northLight, southLight);
                            break;
                        case 6:
                            _ = PrideFrontLights(doorLight, northLight, southLight);
                            break;
                        case 9:
                        case 10:
                            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                ChiefsLights(doorLight, northLight, southLight);
                            }
                            else
                                _ = HalloweenFrontLights(doorLight, northLight, southLight);
                            break;
                        case 12:
                            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                ChiefsLights(doorLight, northLight, southLight);
                            }
                            else
                                _ = ChristmasFrontLights(doorLight, northLight, southLight);
                            break;
                        default:
                            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                ChiefsLights(doorLight, northLight, southLight);
                            }
                            else
                                DefaultFrontLights(doorLight, northLight, southLight);
                            break;
                    }
                }
            }
        }
    }

    private bool IsItAfterThanksgiving()
    {
        // Get the current date
        DateTime currentDate = DateTime.Today;

        // Get the first Thursday of the month
        DateTime firstThursday = new DateTime(currentDate.Year, currentDate.Month, 1);
        while (firstThursday.DayOfWeek != DayOfWeek.Thursday)
        {
            firstThursday = firstThursday.AddDays(1);
        }

        // Get the third Thursday of the month
        DateTime thirdThursday = firstThursday.AddDays(14);

        // Return true if the current date is after the third Thursday of the month, false otherwise
        return currentDate > thirdThursday;
    }
}
