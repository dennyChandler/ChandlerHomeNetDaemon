using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Bedroom.Switches
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

            _entities.InputBoolean.JointInBed.StateChanges().Where(e => e.New?.State?.Equals("off") ?? false && _entities.Switch.BedroomFan.IsOff())
                .Subscribe(e =>
                {
                    _entities.Switch.BedroomFan.TurnOn();
                });
        }
    }
}
