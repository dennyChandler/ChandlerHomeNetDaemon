namespace ChandlerHome.Automations.Bedroom.Switches
{
    [NetDaemonApp(Id = "Bedroom Nightlight Switch")]
    internal class NightlightSwitch : Bedroom
    {
        private TimeSpan motionTimeout = TimeSpan.FromHours(2);
        private DateTime? onTime;
        bool isNightlightCycle;
        public NightlightSwitch(IHaContext ha) : base(ha)
        {
            _entities ??= new Entities(ha);

            _entities.BinarySensor.NightlightOn.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    isNightlightCycle = true;
                    _entities.Switch.BedroomNightlight.TurnOn();
                });

            _entities.BinarySensor.NightlightOn.StateChanges().Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    isNightlightCycle = false;
                    _entities.Switch.BedroomNightlight.TurnOff();
                });

            _entities.BinarySensor.BedroomMotion.StateChanges().Where(e => e.New.IsOn() && DateTime.Now.Hour < 4)
                .Subscribe(x =>
                {
                    if (!(_entities.Sensor.RocinanteSessionstate.State?.Equals("InUse", StringComparison.OrdinalIgnoreCase) ?? false))
                    {
                        if (!_entities.BinarySensor.Pixel8aIsCharging.IsOn())
                        {
                            _entities.Switch.BedroomNightlight.TurnOn();
                            onTime = DateTime.Now;
                        }
                    }
                });

            Observable.Interval(TimeSpan.FromHours(1))
            .Subscribe(_ =>
                {
                    if (!isNightlightCycle)
                    {
                        if (onTime != null)
                        {
                            // Check if it's time to turn off the lights
                            if (_entities.Switch.BedroomNightlight.IsOn()
                            && DateTime.Now - onTime >= motionTimeout)
                            {
                                _entities.Switch.BedroomNightlight.TurnOff();
                                onTime = null;
                            }
                        }
                        else if (onTime == null && _entities.Switch.BedroomNightlight.IsOn())
                        {
                            onTime = DateTime.Now;
                        }
                    }
                });
        }
    }
}
