namespace ChandlerHome.Automations.Bedroom.Lighting;

[NetDaemonApp(Id = "Bedroom Tub Light")]
public class TubLight : Bedroom
{
    public TubLight(IHaContext ha) : base(ha)
    {
        if (_entities == null)
            _entities = new Entities(ha);

        var tubLight = _entities.Light.MasterBathroomTubLight;
        var sunState = _entities.Sun.Sun;

        sunState
        .StateChanges()
            .Where(e => e.New?.State?.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) ?? false && (tubLight.State?.Equals("off", StringComparison.OrdinalIgnoreCase) ?? true))
            .Subscribe(x =>
            {
                if (tubLight.IsOff() && (_entities.BinarySensor.Pixel8aIsCharging.State?.Equals("off") ?? true))
                    TurnOn(tubLight, doesOverrideMatter: false);
            });

        _entities.BinarySensor.Pixel8aIsCharging.StateChanges().Where(e => e.New?.IsOff() ?? false)
            .Subscribe(x =>
            {
                if (tubLight.IsOff() && (_entities.Sun.Sun.State?.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) ?? false))
                    TurnOn(tubLight, doesOverrideMatter: false);
            });

        sunState
        .StateChanges()
            .Where(e => e.New?.State?.Equals("below_horizon", StringComparison.OrdinalIgnoreCase) ?? false)
             .Subscribe(x =>
            {
                if (tubLight.IsOn() && _entities.InputBoolean.JointInBed.IsOff())
                    TurnLightOff(tubLight, 300, false);
            });

        _entities.InputBoolean.JointInBed.StateChanges().Where(e => e.New?.State?.Equals("on") ?? false)
            .Subscribe(x =>
            {
                TurnLightOn(tubLight, 100, 3, "purple", false);
            });

        _entities.InputBoolean.JointInBed.StateChanges().Where(e => e.New?.State?.Equals("off") ?? false)
            .Subscribe(x =>
            {
                if (_entities.Sun.Sun.State?.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) ?? false)
                    TurnOn(tubLight, doesOverrideMatter: false);
                else
                    TurnLightOff(tubLight, 10, false);
            });
    }
}
