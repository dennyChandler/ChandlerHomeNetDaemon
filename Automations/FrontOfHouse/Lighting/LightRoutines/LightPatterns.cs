using System.Threading.Tasks;
using ChandlerHome.Automations;

namespace ChandlerHome.Automations.FrontOfHouse.Lighting.LightRoutines;

public class LightPatterns : Home
{
    internal bool frontLightsOn = false;
    public LightPatterns(IHaContext ha, IScheduler scheduler) : base(ha)
    {
        _entities ??= new Entities(ha);

        _entities.Light.FrontOfHouseLights.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                frontLightsOn = false;
            });

        _entities.Light.FrontOfHouseLights.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                frontLightsOn = true;
            });

        _entities.Sun.Sun.StateChanges().Where(e => e.New.State.Equals("above_horizon", StringComparison.OrdinalIgnoreCase))
            .Subscribe(x =>
            {
                if (frontLightsOn)
                    frontLightsOn = false;
            });

    }

    internal async Task ChristmasFrontLights(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(doorLight, 30, 5, "white", false); //30% because of laser light visibility

        frontLightsOn = true;
        while (frontLightsOn)
        {
            TurnOn(northLight, 100, 5, "red", false);
            TurnOn(southLight, 100, 5, "green", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "green", false);
            TurnOn(southLight, 100, 5, "red", false);
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
        _entities.Light.FrontOfHouseLights.TurnOff();
    }

    internal void FrontLightsWhite(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(doorLight, 100, 5, "white", false);
        TurnOn(northLight, 100, 5, "white", false);
        TurnOn(southLight, 100, 5, "white", false);
        frontLightsOn = true;
    }

    internal void FrontLightsRain(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(doorLight, 100, 5, "dodgerblue", false);
        TurnOn(northLight, 100, 5, "dodgerblue", false);
        TurnOn(southLight, 100, 5, "dodgerblue", false);
        frontLightsOn = true;
    }

    internal void FrontLightsAmerica(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(doorLight, 100, 5, "white", false);
        TurnOn(northLight, 100, 5, "blue", false);
        TurnOn(southLight, 100, 5, "red", false);
        frontLightsOn = true;
    }

    internal void DefaultFrontLights(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(doorLight, 100, 5, "white", false);
        TurnOn(northLight, 100, 5, "white", false);
        TurnOn(southLight, 100, 5, "white", false);
        frontLightsOn = true;
    }

    internal void ChiefsLights(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(doorLight, 100, 5, "yellow", false);
        TurnOn(northLight, 100, 5, "red", false);
        TurnOn(southLight, 100, 5, "red", false);
        frontLightsOn = true;
    }

    internal async Task HalloweenFrontLights(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(doorLight, 100, 5, "orange", false);
        frontLightsOn = true;

        while (frontLightsOn)
        {
            TurnOn(northLight, 100, 5, "purple", false);
            TurnOn(southLight, 100, 5, "purple", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "aquamarine", false);
            TurnOn(southLight, 100, 5, "aquamarine", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "orange", false);
            TurnOn(southLight, 100, 5, "orange", false);
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
        _entities.Light.FrontOfHouseLights.TurnOff();
    }

    internal async Task PrideFrontLights(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(doorLight, 100, 5, "white", false);
        frontLightsOn = true;

        while (frontLightsOn)
        {
            TurnOn(northLight, 100, 5, "orange", false);
            TurnOn(southLight, 100, 5, "red", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "yellow", false);
            TurnOn(southLight, 100, 5, "orange", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "green", false);
            TurnOn(southLight, 100, 5, "yellow", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "blue", false);
            TurnOn(southLight, 100, 5, "green", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "indigo", false);
            TurnOn(southLight, 100, 5, "blue", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "purple", false);
            TurnOn(southLight, 100, 5, "indigo", false);
            await Task.Delay(TimeSpan.FromSeconds(10));

            TurnOn(northLight, 100, 5, "red", false);
            TurnOn(southLight, 100, 5, "purple", false);
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
        _entities.Light.FrontOfHouseLights.TurnOff();
    }

    internal void JuneteenthFrontLights(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
    {
        TurnOn(southLight, 100, 5, "red", false);
        TurnOn(doorLight, 100, 5, "yellow", false);
        TurnOn(northLight, 100, 5, "green", false);
        frontLightsOn = true;
    }
}
