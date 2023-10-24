namespace ChandlerHome.Automations.Bedroom.Lighting;

[NetDaemonApp(Id = "Castle Nightlight")]
internal class CastleNightlight : Bedroom
{
    private TimeSpan motionTimeout = TimeSpan.FromHours(2);
    private TimeSpan DayMotionTimeout = TimeSpan.FromMinutes(15);
    private DateTime? onTime;
    bool isNightlightCycle;
    public CastleNightlight(IHaContext ha) : base(ha)
    {
        var rand = new Random();
        _entities ??= new Entities(ha);

        _entities.BinarySensor.NightlightOn.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {

                isNightlightCycle = true;

                _entities.Light.CastleNighlightLight.TurnOn(rgbColor: new int[] { rand.Next(255), rand.Next(255), rand.Next(255) });
            });

        _entities.BinarySensor.NightlightOn.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                isNightlightCycle = false;
                _entities.Light.CastleNighlightLight.TurnOff();
            });

        _entities.BinarySensor.BedroomMotion.StateChanges().Where(e => e.New.IsOn() && DateTime.Now.Hour < 4)
            .Subscribe(x =>
            {
                if (!_entities.Sensor.RocinanteSessionstate.State.Equals("InUse", StringComparison.OrdinalIgnoreCase))
                {
                    if (!_entities.BinarySensor.Pixel8aIsCharging.IsOn())
                    {
                        _entities.Light.CastleNighlightLight.TurnOn(rgbColor: new int[] { rand.Next(255), rand.Next(255), rand.Next(255) });
                        onTime = DateTime.Now;
                    }
                }
            });

        _entities.BinarySensor.BedroomMotion.StateChanges().Where(e => e.New.IsOn() && _entities.Sensor.WeatherstationIlluminance.State < 1500 && _entities.Light.CastleNighlightLight.IsOff())
            .Subscribe(x =>
            {
                if (!_entities.BinarySensor.Pixel8aIsCharging.IsOn() || !_entities.BinarySensor.BrittanysPhoneIsCharging.IsOn())
                    _entities.Light.CastleNighlightLight.TurnOn(rgbColor: new int[] { rand.Next(255), rand.Next(255), rand.Next(255) });
            });

        _entities.BinarySensor.BedroomMotion.StateChanges().Where(e => e.New.IsOn() && _entities.BinarySensor.HomeBinarySensorsIsRaining.IsOn() && !_entities.BinarySensor.NightlightOn.IsOff() && _entities.Light.CastleNighlightLight.IsOff())
            .Subscribe(x =>
            {
                _entities.Light.CastleNighlightLight.TurnOn(rgbColor: new int[] { 0, 0, rand.Next(100, 255) });
            });

        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(e => e.New.IsOn() && _entities.Light.CastleNighlightLight.IsOff())
            .Subscribe(x =>
            {
                _entities.Light.CastleNighlightLight.TurnOn(rgbColor: new int[] { 0, 0, rand.Next(100, 255) });
            });

        Observable.Interval(TimeSpan.FromMinutes(15))
        .Subscribe(_ =>
        {
            if (!isNightlightCycle)
            {
                if (onTime != null)
                {
                    if (DateTime.Now.Hour > 20 || DateTime.Now.Hour < 7)
                    {
                        // Check if it's time to turn off the lights
                        if (_entities.Light.CastleNighlightLight.IsOn()
                        && DateTime.Now - onTime >= motionTimeout)
                        {
                            _entities.Light.CastleNighlightLight.TurnOff();
                            onTime = null;
                        }
                    }
                    else
                    {
                        if (_entities.Light.CastleNighlightLight.IsOn()
                        && DateTime.Now - onTime >= DayMotionTimeout
                        && _entities.BinarySensor.HomeBinarySensorsIsRaining.IsOff())
                        {
                            _entities.Light.CastleNighlightLight.TurnOff();
                            onTime = null;
                        }
                    }
                }
                else if (onTime == null && _entities.Light.CastleNighlightLight.IsOn())
                {
                    onTime = DateTime.Now;
                }
            }
        });
    }
}