using System.Threading.Tasks;
using ChandlerHome.Automations;

namespace ChandlerHome.Automations.Nursery;

internal class Nursery : Home
{
    public Nursery(IHaContext ha, IScheduler scheduler) : base(ha)
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

        scheduler.ScheduleCron("30 7 * * *", () => _entities.InputBoolean.NurseryOverride.TurnOff());
    }

    public void TurnOn(LightEntity light, int brightnessPercent, int transition, object? colorName = null, string? effect = null, object? rgbwwValue = null)
    {
        if (!NurseryOverride())
        {
            base.TurnOn(light, brightnessPercent: brightnessPercent, transition: transition, colorName: colorName, true, effect, rgbwwValue);
        }
    }

    public void TurnOff(LightEntity light, int transition)
    {
        if (!NurseryOverride())
        {
            base.TurnOff(light, transition: transition);
        }
    }

    public void OpenBlinds(CoverEntity cover, long? position)
    {
        cover.SetCoverPosition(new CoverSetCoverPositionParameters { Position = position });
    }
    public void CloseBlinds(CoverEntity cover, long? position)
    {
        cover.SetCoverPosition(new CoverSetCoverPositionParameters { Position = position });
    }

    public bool NurseryOverride()
    {
        return _entities?.InputBoolean.NurseryOverride?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }
    public bool IsItTooDamnHot(Entity entity)
    {
        return _entities?.InputBoolean.ItsTooDamnHot?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
