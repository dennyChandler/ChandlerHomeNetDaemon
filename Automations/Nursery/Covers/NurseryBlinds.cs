using System.Threading;

namespace ChandlerHome.Automations.Nursery.Covers;

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
        var napTime = _entities.Schedule.OctaviasRest;
        bool notificationSent = false;

        napTime.StateChanges().Where(e => e.New?.IsOn() ?? false)
            .Subscribe(x =>
            {
                blindsCloseToggle.TurnOn();
            });

        napTime.StateChanges().Where(e => e.New?.IsOff() ?? false)
            .Subscribe(x =>
            {
                timeToOpen = true;
            });

        bedtime.StateChanges().Where(e => e.New?.IsOff() ?? false && blindsEntity.Attributes.CurrentPosition.Value < 50)
            .Subscribe(x =>
            {
                timeToOpen = true;
            });

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
                if (_entities.BinarySensor.NurseryWindow.IsOn())
                {
                    if (!notificationSent)
                    {

                        var services = new Services(ha);
                        services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters
                        {
                            Message = "Nursery window is open and the blinds want to close!",
                            Title = "Close your nursery window!"
                        });
                        notificationSent = true;
                        return;
                    }
                    return;
                }
                CloseBlinds(blindsEntity, 20);
                blindsCloseToggle.TurnOff();
                notificationSent = false;
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
               if (_entities.InputBoolean.NurseryBlindsShut.IsOn())
               {
                   _entities.InputBoolean.NurseryBlindsShut.TurnOff();
                   Thread.Sleep(300);
                   _entities.InputBoolean.NurseryBlindsShut.TurnOn();
               }


           });
    }
}
