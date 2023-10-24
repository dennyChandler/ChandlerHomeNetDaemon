using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Livingroom.Switches
{
    [NetDaemonApp(Id = "Fireplace Light Switch")]
    internal class FireplaceLightswitch:Livingroom
    {
        Services _services;
        public FireplaceLightswitch(IHaContext ha):base(ha)
        {
            _entities ??= new Entities(ha);

            _services ??= new Services(ha);

            var lightswitch = _entities.Light.FireplaceLightswitchLight;
            var fireplaceLights = _entities.Light.FireplaceLights;

            fireplaceLights.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x => 
                {
                    if (HolidayLights())
                    {
                        switch(DateTime.Now.Month)
                        {
                            case 9:
                            case 10:
                                CycleHalloweenLightswitchColors();
                                break;
                            case 12:
                                CycleChristmasLightswitchColors();
                                break;
                            default:
                                break;
                        }
                    }                        
                    else
                        _services.Script.InovelliLed(entityId: "light.fireplace_lightswitch_light", model: "dimmer", color: "pink", effect: "solid", duration: "Indefinitely");
                });

            fireplaceLights.StateChanges().Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    _services.Script.InovelliLed(entityId: "light.fireplace_lightswitch_light", model: "dimmer", color: "orange", effect: "pulse", duration: "Indefinitely");
                    
                });            

        }

        private void CycleHalloweenLightswitchColors()
        {
            while (_entities?.Light.FireplaceLights.IsOn() ?? false && HolidayLights())
            {
                _services.Script.InovelliLed(entityId: "light.fireplace_lightswitch_light", model: "dimmer", color: "purple", effect: "chase", duration: "4 seconds");
                Thread.Sleep(4000);
                _services.Script.InovelliLed(entityId: "light.fireplace_lightswitch_light", model: "dimmer", color: "green", effect: "chase", duration: "4 seconds");
                Thread.Sleep(4000);
            }
        }

        private void CycleChristmasLightswitchColors()
        {
            while (_entities?.Light.FireplaceLights.IsOn() ?? false && HolidayLights())
            {
                _services.Script.InovelliLed(entityId: "light.fireplace_lightswitch_light", model: "dimmer", color: "red", effect: "chase", duration: "4 seconds");
                Thread.Sleep(4000);
                _services.Script.InovelliLed(entityId: "light.fireplace_lightswitch_light", model: "dimmer", color: "green", effect: "chase", duration: "4 seconds");
                Thread.Sleep(4000);
            }
        }
    }
}
