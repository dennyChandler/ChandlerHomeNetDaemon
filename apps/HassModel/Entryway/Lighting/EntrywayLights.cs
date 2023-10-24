using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Entryway.Lighting
{
    [NetDaemonApp(Id = "Entryway Lights")]
    internal class EntrywayLights : Home
    {
        public EntrywayLights(IHaContext ha) : base(ha)
        {
            _entities ??= new Entities(ha);

            var entrywayMotion = _entities.BinarySensor.EntrywayMotion;
            var entrywayLights = _entities.Light.ZgroupEntrywayLights;

            entrywayMotion.StateChanges().Where(e => e.New.IsOn() && DateTime.Now.Hour < 21 && DateTime.Now.Hour > 7)
                .Subscribe(x =>
                {
                    if (entrywayLights.IsOff() && (DateTime.Now.Hour > 8 && DateTime.Now.Hour < 21) || 
                    (_entities.BinarySensor.DennysPhoneIsCharging.IsOff() && _entities.BinarySensor.BrittanysPhoneIsCharging.IsOff()))
                        TurnOn(entrywayLights, 100, 0);
                    else if (_entities.BinarySensor.DennysPhoneIsCharging.IsOn() || _entities.BinarySensor.BrittanysPhoneIsCharging.IsOn()
                    && _entities.Sensor.WeatherflowBrightness.State < 2000)
                        TurnOn(entrywayLights, 10, 0);
                });

            entrywayMotion.StateChanges().Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    TurnOff(entrywayLights, 3, false);
                });
        }
    }
}
