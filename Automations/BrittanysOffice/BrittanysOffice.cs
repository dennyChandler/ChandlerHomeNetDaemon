using ChandlerHome.Automations;

namespace ChandlerHome.Automations.BrittanysOffice;

internal class BrittanysOffice : Home
{
    public BrittanysOffice(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);
    }

    private bool BrittanysOfficeOverride()
    {
        return _entities?.InputBoolean.BrittanysOfficeOverride?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public void TurnOn(LightEntity light, int brightnessPercent, int transition, object? colorName = null, string? effect = null, object? rgbwwValue = null)
    {
        if (!BrittanysOfficeOverride())
        {
            base.TurnOn(light, brightnessPercent: brightnessPercent, transition: transition, colorName: colorName, true, effect, rgbwwValue);
        }
    }

    public void TurnOff(LightEntity light, int transition)
    {
        if (!BrittanysOfficeOverride())
        {
            base.TurnOff(light, transition: transition);
        }
    }
}
