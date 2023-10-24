using ChandlerHome.Automations;

namespace ChandlerHome.Automations.Kitchen;

public class Kitchen : Home
{

    public Kitchen(IHaContext ha) : base(ha)
    {
        _entities = new Entities(ha);
    }

    public void TurnLightOn(LightEntity light, int brightnessPercent, int transition, object? colorName = null, bool doesOverrideMatter = true)
    {
        if (!KitchenOverride())
        {
            base.TurnOn(light, brightnessPercent: brightnessPercent, transition: transition, colorName: colorName);
        }
        else if (!doesOverrideMatter)
        {
            base.TurnOn(light, brightnessPercent: brightnessPercent, transition: transition, colorName: colorName);
        }
    }

    public void TurnLightOff(LightEntity light, int transition, bool doesOverrideMatter = true)
    {
        if (!KitchenOverride())
        {
            base.TurnOff(light, transition: transition);
        }
        else if (!doesOverrideMatter)
        {
            base.TurnOff(light, transition: transition);
        }
    }

    internal bool KitchenOverride()
    {
        return _entities?.InputBoolean.KitchenOverride?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
