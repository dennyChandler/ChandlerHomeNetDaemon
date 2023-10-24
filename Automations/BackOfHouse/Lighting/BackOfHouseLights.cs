namespace ChandlerHome.Automations.BackOfHouse.Lighting;

[NetDaemonApp(Id = "Back Of House Lights")]
internal class BackOfHouseLights : BackOfHouse
{
    DateTime? LightOnTime;
    TimeSpan lightTimeout = TimeSpan.FromMinutes(5);
    public BackOfHouseLights(IHaContext ha, IScheduler scheduler) : base(ha)
    {
        _entities ??= new Entities(ha);

        scheduler.ScheduleCron("0 0 * * *", () => _entities.Light.BackDeckLights.TurnOff());

        var sun = _entities.Sun.Sun;
        var bedroomLight = _entities.Light.KitchenDeckLight;
        var kitchenLight = _entities.Light.BedroomDeckLight;
        var backOfHouseLights = _entities.Light.BackDeckLights;

        _ = sun.StateChanges().Where(e => e.New.State.Equals("below_horizon", StringComparison.OrdinalIgnoreCase))
            .Subscribe(x =>
            {
                TurnOnBackDeckLights(bedroomLight, kitchenLight);
            });


        _ = _entities.BinarySensor.BedroomGlassDoor.StateChanges().Where(e => e.New.IsOn() && (_entities.Sun.Sun.State.Equals("below_horizon", StringComparison.OrdinalIgnoreCase) || _entities.Sensor.WeatherstationIlluminance.State < 2000) && bedroomLight.IsOff()) //door opens
            .Subscribe(x =>
            {
                bedroomLight.TurnOn(colorName: "white", brightnessPct: 100);
            });
        _ = _entities.BinarySensor.KitchenGlassDoor.StateChanges().Where(e => e.New.IsOn() && (_entities.Sun.Sun.State.Equals("below_horizon", StringComparison.OrdinalIgnoreCase) || _entities.Sensor.WeatherstationIlluminance.State < 2000) && kitchenLight.IsOff()) //door opens
            .Subscribe(x =>
            {
                kitchenLight.TurnOn(colorName: "white", brightnessPct: 100);
            });

        _entities.Light.BackDeckLights.StateChanges().Where(e => e.New.IsOn() && DateTime.Now.Hour < 20)
            .Subscribe(x =>
            {
                LightOnTime = DateTime.Now;
            });

        Observable.Interval(TimeSpan.FromMinutes(1))
        .Subscribe(_ =>
        {
            if (DateTime.Now.Hour < 17 && LightOnTime != null)
            {
                // Check if it's time to turn off the lights
                if (backOfHouseLights.IsOn()
                && DateTime.Now - LightOnTime >= lightTimeout)
                {
                    if (DateTime.Now.Hour < 17)
                    {
                        TurnOff(backOfHouseLights, transition: 60);
                        LightOnTime = null;
                    }
                    else if (_entities.Sensor.WeatherstationIlluminance.State > 2000)
                    {
                        TurnOff(backOfHouseLights, transition: 60);
                        LightOnTime = null;
                    }
                }
            }
            else if (LightOnTime == null && backOfHouseLights.IsOn()
                && DateTime.Now.Hour < 17)
            {
                LightOnTime = DateTime.Now;
            }
        });
    }

    private void TurnOnBackDeckLights(LightEntity bedroomLight, LightEntity kitchenLight)
    {
        switch (DateTime.Now.Month)
        {
            case 10:
                bedroomLight.TurnOn(colorName: "purple", brightnessPct: 100);
                kitchenLight.TurnOn(colorName: "orange", brightnessPct: 100);
                break;
            case 12:
                bedroomLight.TurnOn(colorName: "red", brightnessPct: 100);
                kitchenLight.TurnOn(colorName: "green", brightnessPct: 100);
                break;
            default:
                bedroomLight.TurnOn(colorName: "white", brightnessPct: 100);
                kitchenLight.TurnOn(colorName: "white", brightnessPct: 100);
                break;
        }
    }
}
