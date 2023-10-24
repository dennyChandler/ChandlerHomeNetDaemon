namespace ChandlerHome.Automations.Bedroom.Switches
{
    [NetDaemonApp(Id = "Bedroom Fan Switch")]
    public class FanSwitch : Bedroom
    {
        public FanSwitch(IHaContext ha) : base(ha)
        {
            if (_entities == null)
                _entities = new Entities(ha);

            _entities.InputBoolean.JointInBed.StateChanges().Where(e => e.New?.State?.Equals("on") ?? false && _entities.Switch.BedroomFan.IsOn())
                .Subscribe(e =>
                {
                    _entities.Switch.BedroomFan.TurnOff();
                });
        }
    }
}
