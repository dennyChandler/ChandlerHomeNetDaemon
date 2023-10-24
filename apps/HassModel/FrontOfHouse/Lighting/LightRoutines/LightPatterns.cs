using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.FrontOfHouse.Lighting.LightRoutines
{
    public class LightPatterns : Home
    {
        bool frontLightsOn = false;
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
            TurnOn(doorLight, 100, 5, "white", false);

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

        internal async Task DefaultFrontLights(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
        {
            TurnOn(doorLight, 100, 5, "white", false);
            frontLightsOn = true;

            while (frontLightsOn)
            {
                TurnOn(northLight, 100, 5, "purple", false);
                TurnOn(southLight, 100, 5, "purple", false);
                await Task.Delay(TimeSpan.FromSeconds(10));

                TurnOn(northLight, 100, 5, "pink", false);
                TurnOn(southLight, 100, 5, "pink", false);
                await Task.Delay(TimeSpan.FromSeconds(10));

                TurnOn(northLight, 100, 5, "orange", false);
                TurnOn(southLight, 100, 5, "orange", false);
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            _entities.Light.FrontOfHouseLights.TurnOff();
        }

        internal void ChiefsLights(LightEntity doorLight, LightEntity northLight, LightEntity southLight)
        {
            TurnOn(doorLight, 100, 5, "yellow", false);
            TurnOn(northLight, 100, 5, "red", false);
            TurnOn(southLight, 100, 5, "red", false);
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
                TurnOn(northLight, 100, 5, "red", false);
                TurnOn(southLight, 100, 5, "red", false);
                await Task.Delay(TimeSpan.FromSeconds(10));

                TurnOn(northLight, 100, 5, "orange", false);
                TurnOn(southLight, 100, 5, "orange", false);
                await Task.Delay(TimeSpan.FromSeconds(10));

                TurnOn(northLight, 100, 5, "yellow", false);
                TurnOn(southLight, 100, 5, "yellow", false);
                await Task.Delay(TimeSpan.FromSeconds(10));

                TurnOn(northLight, 100, 5, "green", false);
                TurnOn(southLight, 100, 5, "green", false);
                await Task.Delay(TimeSpan.FromSeconds(10));

                TurnOn(northLight, 100, 5, "blue", false);
                TurnOn(southLight, 100, 5, "blue", false);
                await Task.Delay(TimeSpan.FromSeconds(10));

                TurnOn(northLight, 100, 5, "indigo", false);
                TurnOn(southLight, 100, 5, "indigo", false);
                await Task.Delay(TimeSpan.FromSeconds(10));

                TurnOn(northLight, 100, 5, "purple", false);
                TurnOn(southLight, 100, 5, "purple", false);
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            _entities.Light.FrontOfHouseLights.TurnOff();
        }
    }
}
