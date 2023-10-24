namespace ChandlerHome.Automations.BackOfHouse.Lighting;

[NetDaemonApp(Id = "Deck Lights")]
internal class DeckLights : BackOfHouse
{
    public DeckLights(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);

        var sun = _entities.Sun.Sun;
        var deckLights = _entities.Switch.DeckLights;

        sun.StateChanges().Where(e => e.New.State.Equals("below_horizon", StringComparison.OrdinalIgnoreCase) && deckLights.IsOff())
            .Subscribe(x =>
            {
                deckLights.TurnOn();
            });

        sun.StateChanges().Where(e => e.New.State.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) && deckLights.IsOn())
            .Subscribe(x =>
            {
                deckLights.TurnOff();
            });

        if (sun.State.Equals("above_horizon", StringComparison.OrdinalIgnoreCase))
            deckLights.TurnOff();
        else
            deckLights.TurnOn();

        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(e => e.New.IsOn() && deckLights.IsOff())
            .Subscribe(x =>
            {
                if (sun.State.Equals("above_horizon", StringComparison.OrdinalIgnoreCase))
                    deckLights.TurnOn();
            });
        _entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(e => e.New.IsOff() && deckLights.IsOn())
            .Subscribe(x =>
            {
                if (sun.State.Equals("above_horizon", StringComparison.OrdinalIgnoreCase))
                    deckLights.TurnOff();
            });

    }
}
