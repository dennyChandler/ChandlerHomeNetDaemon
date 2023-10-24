using ChandlerHome.apps.HassModel.FrontOfHouse.Lighting.LightRoutines;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.FrontOfHouse.Lighting
{
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

            sun.StateChanges().Where(e => e.New?.State?.Equals("below_horizon", StringComparison.OrdinalIgnoreCase) ?? false)
                .SubscribeAsync(async x =>
                {
                    switch (DateTime.Now.Month)
                    {
                        case 1:
                            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                ChiefsLights(doorLight, northLight, southLight);
                            }
                            else
                                await DefaultFrontLights(doorLight, northLight, southLight);
                            break;
                        case 6:
                            await PrideFrontLights(doorLight, northLight, southLight);
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
                        case 12:
                            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                ChiefsLights(doorLight, northLight, southLight);
                            }
                            else
                                await ChristmasFrontLights(doorLight, northLight, southLight);
                            break;
                        default:
                            if (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Friday))
                            {
                                ChiefsLights(doorLight, northLight, southLight);
                            }
                            else
                                await DefaultFrontLights(doorLight, northLight, southLight);
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
                                    _ = DefaultFrontLights(doorLight, northLight, southLight);
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
                                    _ = DefaultFrontLights(doorLight, northLight, southLight);
                                break;
                        }
                    }
                }
            }
        }
    }
}
