namespace ChandlerHome.Automations.Kitchen.Lighting;

[NetDaemonApp(Id = "Kitchen Hanging Lights")]
internal class KitchenHangingLights : Kitchen
{
    public KitchenHangingLights(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);

        var kitchenLights = _entities.Light.KitchenHangingLights;

        _entities.BinarySensor.KitchenMotion.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                if (DateTime.Now.Hour > 17 || DateTime.Now.Hour < 4)
                    kitchenLights.TurnOn(transition: 2, brightnessPct: 75, colorTemp: 454);
                else
                    kitchenLights.TurnOn(transition: 2, brightnessPct: 100, colorTemp: 222);
            });

        _entities.BinarySensor.KitchenMotion.StateChanges().Where(e => e.New.IsOff())
           .Subscribe(x =>
           {
               TurnOff(kitchenLights, 10, false);
           });
    }
}
