using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.Automations.Bedroom.Switches
{
    [NetDaemonApp(Id = "Roku TV Smart Plug")]
    internal class RokuTVSmartPlug : Bedroom
    {
        public RokuTVSmartPlug(IHaContext ha, IScheduler scheduler) : base(ha)
        {
            _entities ??= new Entities(ha);

            scheduler.ScheduleCron("0 16 * * *", () => TurnOfPlugIfTVIsntActive(_entities));

            scheduler.ScheduleCron("59 19 * * *", () => TurnOnPlug(_entities));
        }

        private void TurnOfPlugIfTVIsntActive(Entities entities)
        {
            if (!entities.MediaPlayer.RokuTv.State.Equals("Playing", StringComparison.OrdinalIgnoreCase))
            {
                if (!IsBedroomOverrideOn())
                    entities.Switch.RokuSwitch.TurnOff();
            }
        }

        private void TurnOnPlug(Entities entities)
        {
            entities.Switch.RokuSwitch.TurnOn();
        }
    }
}
