namespace ChandlerHome.Automations.Office.Switches;

[NetDaemonApp(Id = "Dennys Office Light Switch")]
public class Lightswitch : Office
{
    public Lightswitch(IHaContext ha) : base(ha)
    {
        if (_entities == null)
            _entities = new Entities(ha);

        //SetupSwitchLightsOnEffect(_entities, ha);
        //SetupDeadboltUnlockedNotification(_entities, ha);
        //SetupDeadboltLockedNotification(_entities);
    }

    private void SetupSwitchLightsOnEffect(Entities entities, IHaContext ha)
    {
        entities.Light.DennysOfficeLights.StateChanges().Where(e => e.New?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false)
            .Subscribe(x =>
            {
                var services = new Services(ha);
                services.Script.InovelliLed(entityId: "light.dennys_office_switch", model: "dimmer", color: "purple", effect: "breath", duration: "5");
            });
    }

    private void SetupDeadboltUnlockedNotification(Entities entities, IHaContext ha)
    {
        entities.Lock.Deadbolt.StateChanges().Where(e => e.New?.State?.Equals("unlocked", StringComparison.OrdinalIgnoreCase) ?? false)
           .Subscribe(e =>
           {
               var services = new Services(ha);
               services.Script.InovelliLed(entityId: "light.dennys_office_switch", model: "dimmer", color: "green", effect: "breath", duration: "5");
           });
    }

    private void SetupDeadboltLockedNotification(Entities entities)
    {
        var lightswitch = entities.Light.DennysOfficeSwitch;

        entities.Lock.Deadbolt.StateChanges().Where(e => e.New?.State?.Equals("locked", StringComparison.OrdinalIgnoreCase) ?? false)
           .Subscribe(e =>
           {
               TurnOn(lightswitch, 100, 0, "red");
           });
    }
}
