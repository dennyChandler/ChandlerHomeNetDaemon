using ChandlerHome.apps.HassModel.Kitchen.Lighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Nursery.Lighting
{
    [NetDaemonApp(Id = "Nursery Fan Light")]
    internal class NurseryFanLights : Nursery
    {
        private TimeSpan motionTimeout = TimeSpan.FromMinutes(10);
        DateTime? offTime;
        public NurseryFanLights(IHaContext ha, IScheduler scheduler) : base(ha, scheduler)
        {
            _entities ??= new Entities(ha);

            _entities.BinarySensor.NurseryMotion.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    TurnOn(_entities.Light.NurseryFanLights, 100, 10);
                });

            _entities.BinarySensor.NurseryDoor.StateChanges().Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    TurnOff(_entities.Light.NurseryFanLights, 600);
                });

            _entities.BinarySensor.NurseryMotion.StateChanges().Where(e => e.New.IsOff())
                .Subscribe(x =>
                {
                    offTime = DateTime.Now;
                });

            Observable.Interval(TimeSpan.FromMinutes(1))
                .Subscribe(_ =>
                {
                    if (offTime != null)
                    {
                        // Check if it's time to turn off the lights
                        if (_entities.Light.NurseryFanLights.IsOn()
                        && DateTime.Now - offTime >= motionTimeout
                        && _entities.BinarySensor.NurseryMotion.IsOff())
                        {
                            TurnOff(_entities.Light.NurseryFanLights, transition: 60);
                            offTime = null;
                        }
                    }
                    else if (offTime == null && _entities.BinarySensor.NurseryMotion.IsOff() && _entities.Light.NurseryFanLights.IsOn())
                    {
                        offTime = DateTime.Now;
                    }
                });
        }
    }
}
