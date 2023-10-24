using ChandlerHome.Automations;

namespace ChandlerHome.Automations.Livingroom;

public class Livingroom : Home
{
    public Livingroom(IHaContext ha) : base(ha)
    {
        _entities = new Entities(ha);
    }

    public void TurnOn(LightEntity light, int brightnessPercent, int transition, object? colorName = null)
    {
        if (!LivingRoomOverride())
        {
            base.TurnOn(light, brightnessPercent: brightnessPercent, transition: transition, colorName: colorName);
        }
    }

    public void TurnOff(LightEntity light, int transition)
    {
        if (!LivingRoomOverride())
        {
            base.TurnOff(light, transition: transition);
        }
    }

    private bool LivingRoomOverride()
    {
        return _entities?.InputBoolean.LivingRoomOverride?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }
    internal bool HolidayLights()
    {
        return _entities?.InputBoolean.HolidayLights?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
