namespace ChandlerHome.Automations;

public class Home
{
    protected Entities? _entities { get; set; }
    protected Services? _services { get; set; }
    public Home(IHaContext ha)
    {
        _entities = new Entities(ha);
        _services = new Services(ha);
    }

    public virtual void TurnOn(LightEntity light, long? brightnessPercent = null, long? transition = null, object? colorName = null, bool doesOverrideMatter = true, string? effect = null, object? rgbwwColor = null, long? kelvin = null)
    {
        if (!IsVacationModeOn())
        {
            light.TurnOn(brightnessPct: brightnessPercent, transition: transition, colorName: colorName, effect: effect, rgbwwColor: rgbwwColor, kelvin: kelvin);
        }
        else if (!doesOverrideMatter)
        {
            light.TurnOn(brightnessPct: brightnessPercent, transition: transition, colorName: colorName, effect: effect, rgbwwColor: rgbwwColor, kelvin: kelvin);
        }
    }

    public virtual void TurnOff(LightEntity light, int transition, bool doesOverrideMatter = true)
    {
        if (!IsVacationModeOn())
        {
            light.TurnOff(transition: transition);
        }
        else if (!doesOverrideMatter)
        {
            light.TurnOff(transition: transition);
        }
    }
    public bool IsVacationModeOn()
    {
        return _entities?.InputBoolean.VacationMode?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }
    internal bool IsItTooDamnHot()
    {
        return _entities?.InputBoolean.ItsTooDamnHot?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public void SendHouseholdNotification(string title, string message, string? brittanyMessage = null)
    {
        if (brittanyMessage == null)
        {
            _services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters()
            {
                Title = title,
                Message = message
            });
        }
        else
        {
            _services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters()
            {
                Title = title,
                Message = message
            });
            _services.Notify.MobileAppBrittanysPhone(new NotifyMobileAppBrittanysPhoneParameters()
            {
                Title = title,
                Message = brittanyMessage
            });
        }
    }
}
