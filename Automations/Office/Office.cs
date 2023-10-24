using System.Threading.Tasks;
using ChandlerHome.Automations;

namespace ChandlerHome.Automations.Office;

public class Office : Home
{
    public Office(IHaContext ha) : base(ha)
    {
        _entities = new Entities(ha);

        _entities.InputBoolean.MuteDennysPc.StateChanges().Where(e => e.New.IsOn())
            .SubscribeAsync(async x =>
            {
                while (_entities.Sensor.RocinanteVolume.State > 0)
                {
                    _entities.Switch.RocinanteStepVolumeDown.TurnOn();
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
    }

    public void TurnOn(LightEntity light, int brightnessPercent, int transition, object? colorName = null, string? effect = null)
    {
        if (!IsOfficeOverrideOn())
        {
            if (IsItTooDamnHot(light))
                base.TurnOn(light, brightnessPercent: 30, transition: transition, colorName: colorName, true, effect);
            else
                base.TurnOn(light, brightnessPercent: brightnessPercent, transition: transition, colorName: colorName, true, effect);
        }
    }

    public void OpenBlinds(CoverEntity cover, long? position)
    {
        if (!IsOfficeOverrideOn() && !IsItTooDamnHot(cover))
            cover.SetCoverPosition(new CoverSetCoverPositionParameters { Position = position });
    }
    public void CloseBlinds(CoverEntity cover, long? position)
    {
        if (!IsOfficeOverrideOn() && !IsItTooDamnHot(cover))
            cover.SetCoverPosition(new CoverSetCoverPositionParameters { Position = position });
    }

    public void TurnOff(LightEntity light, int transition)
    {
        if (!IsOfficeOverrideOn())
        {
            base.TurnOff(light, transition: transition, true);
        }
    }

    private bool IsOfficeOverrideOn()
    {
        return _entities?.InputBoolean.DennysOfficeOverride?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }
    private bool IsItTooDamnHot(Entity entity)
    {
        if (entity != _entities?.Light.DennysOfficeSwitch)
            return _entities?.InputBoolean.ItsTooDamnHot?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;

        return false;
    }
}
