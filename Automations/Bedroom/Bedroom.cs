using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChandlerHome.Automations;

namespace ChandlerHome.Automations.Bedroom
{
    public class Bedroom : Home
    {

        public Bedroom(IHaContext ha) : base(ha)
        {
            _entities = new Entities(ha);
        }

        public void TurnLightOn(LightEntity light, int brightnessPercent, int transition, object? colorName = null, bool doesOverrideMatter = true, long? kelvin = null)
        {
            if (!IsBedroomOverrideOn())
            {
                base.TurnOn(light, brightnessPercent: brightnessPercent, transition: transition, colorName: colorName, kelvin: kelvin);
            }
            else if (!doesOverrideMatter)
            {
                base.TurnOn(light, brightnessPercent: brightnessPercent, transition: transition, colorName: colorName, kelvin: kelvin);
            }
        }

        public void TurnLightOff(LightEntity light, int transition, bool doesOverrideMatter = true)
        {
            if (!IsBedroomOverrideOn())
            {
                base.TurnOff(light, transition: transition);
            }
            else if (!doesOverrideMatter)
            {
                base.TurnOff(light, transition: transition);
            }
        }

        public bool IsBedroomOverrideOn()
        {
            return _entities?.InputBoolean.BedroomOverride?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
