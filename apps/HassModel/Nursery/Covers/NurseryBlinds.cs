using ChandlerHome.apps.HassModel.Kitchen.Lighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Nursery.Covers
{
    [NetDaemonApp(Id = "Nursery Blinds")]
    internal class NurseryBlinds : Nursery
    {
        bool timeToOpen = false;
        public NurseryBlinds(IHaContext ha, IScheduler scheduler) : base(ha, scheduler)
        {
            if (_entities == null)
                _entities = new Entities(ha);

            var blindsOpenToggle = _entities.InputBoolean.NurseryBlindsOpen;
            var blindsCloseToggle = _entities.InputBoolean.NurseryBlindsShut;
            var blindsEntity = _entities.Cover.NurseryBlindsCover;
            var sun = _entities.Sun.Sun;
            var bedtime = _entities.BinarySensor.OctaviasBedtime;

            sun.StateChanges().Where(e => e.New?.State?.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) ?? false
            && _entities.Sensor.WeatherflowAirTemperature.State < 85)
                .Subscribe(x =>
                {
                    timeToOpen = true;
                });

            if (sun.State.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) && blindsEntity.Attributes.CurrentPosition.Value < 50)
            {
                timeToOpen = true;
            }

            bedtime.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    if (blindsEntity.Attributes.CurrentPosition.HasValue
                    && blindsEntity.Attributes.CurrentPosition.Value > 50
                    && _entities.InputBoolean.NurseryOverride.IsOff()
                    && _entities.BinarySensor.NurseryWindow.IsOff())
                        blindsCloseToggle.TurnOn();

                });

            blindsCloseToggle.StateChanges().Where(e => e.New?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false)
                .Subscribe(x =>
                {
                    if (_entities.BinarySensor.DennyOfficeWindow.IsOn())
                    {
                        var services = new Services(ha);
                        services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters
                        {
                            Message = "Nursery window is open and the blinds want to close!",
                            Title = "Close your nursery window!"
                        });
                        return;
                    }
                    CloseBlinds(blindsEntity, 27);
                    blindsCloseToggle.TurnOff();
                });

            blindsOpenToggle.StateChanges().Where(e => e.New?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false)
                .Subscribe(x =>
                {

                    OpenBlinds(blindsEntity, 97);
                    blindsOpenToggle.TurnOff();
                });

            Observable.Interval(TimeSpan.FromMinutes(5))
               .Subscribe(_ =>
               {
                   if (timeToOpen)
                   {
                       if (blindsEntity.Attributes.CurrentPosition.Value < 50 && _entities.BinarySensor.NurseryDoor.IsOn())
                       {
                           if (!NurseryOverride() && !IsItTooDamnHot())
                           {
                               blindsOpenToggle.TurnOn();
                               timeToOpen = false;
                           }
                       }
                       else if (blindsEntity.Attributes.CurrentPosition.Value > 50)
                           timeToOpen = false;

                   }
               });
        }
    }
}
