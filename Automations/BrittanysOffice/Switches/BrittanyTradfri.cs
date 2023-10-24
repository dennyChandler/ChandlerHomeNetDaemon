namespace ChandlerHome.Automations.BrittanysOffice.Switches;

[NetDaemonApp(Id = "Brittany Lava Lamp")]
internal class BrittanyTradfri : BrittanysOffice
{
    private TimeSpan motionTimeout = TimeSpan.FromHours(1);
    DateTime? offTime;
    public BrittanyTradfri(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);
        var listOfWeekdays = new List<string> { "monday", "tuesday", "wednesday", "thursday", "friday" };

        _entities.BinarySensor.BrittanysOfficeMotion.StateChanges().Where(e => e.New.IsOn() && listOfWeekdays.Contains(DateTime.Now.DayOfWeek.ToString().ToLower()) && DateTime.Now.Hour < 17)
            .Subscribe(x =>
            {
                _entities.Switch.BrittanysOfficeTradfriSwitch.TurnOn();
            });

        Observable.Interval(TimeSpan.FromMinutes(30))
            .Subscribe(_ =>
            {
                if (offTime != null)
                {
                    // Check if it's time to turn off the lights
                    if (_entities.Switch.BrittanysOfficeTradfriSwitch.IsOn()
                    && DateTime.Now - offTime >= motionTimeout
                    && _entities.BinarySensor.BrittanysOfficeMotion.IsOff())
                    {
                        _entities.Switch.BrittanysOfficeTradfriSwitch.TurnOff();
                        offTime = null;
                    }
                }
                else if (offTime == null && _entities.BinarySensor.BrittanysOfficeMotion.IsOff() && _entities.Switch.BrittanysOfficeTradfriSwitch.IsOn())
                {
                    offTime = DateTime.Now;
                }
            });
    }
}
