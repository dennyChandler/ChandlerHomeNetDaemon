namespace ChandlerHome.Automations.Office.Switches;

[NetDaemonApp(Id = "Dennys Office Lava Lamp")]
internal class OfficeTradfri : Office
{
    public OfficeTradfri(IHaContext ha) : base(ha)
    {
        var entities = new Entities(ha);

        entities.Sensor.RocinanteSessionstate.StateChanges().Where(e => e.New?.State?.Equals("InUse") ?? false && entities.Switch.DennysOfficeTradfriSwitch.IsOff())
            .Subscribe(e =>
            {
                if (entities.Switch.DennysOfficeTradfriSwitch.IsOff())
                    entities.Switch.DennysOfficeTradfriSwitch.TurnOn();
            });

        entities.Sensor.RocinanteSessionstate.StateChanges().Where(e => !e.New?.State?.Equals("InUse") ?? false && entities.Switch.DennysOfficeTradfriSwitch.IsOn())
            .Subscribe(e =>
            {
                if (entities.Switch.DennysOfficeTradfriSwitch.IsOn())
                    entities.Switch.DennysOfficeTradfriSwitch.TurnOff();
            });

        if (entities.Sensor.RocinanteSessionstate.State.Equals("InUse", StringComparison.Ordinal))
        {
            entities.Switch.DennysOfficeTradfriSwitch.TurnOn();
        }
        else
        {
            entities.Switch.DennysOfficeTradfriSwitch.TurnOff();
        }
    }
}
