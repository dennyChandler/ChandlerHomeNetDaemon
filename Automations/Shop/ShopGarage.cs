using ChandlerHome.Helpers.Notification_Messages;

namespace ChandlerHome.Automations.Shop;

[NetDaemonApp(Id = "Shop Garage Door")]
internal class ShopGarage
{
    TimeSpan warningTimeTimespan = TimeSpan.FromHours(3);
    DateTime? openTime = null;
    public ShopGarage(IHaContext ha)
    {
        var entities = new Entities(ha);
        var services = new Services(ha);



        entities.Cover.BasementGarageDoor.StateChanges().Where(e => e.New.State.Equals("open", StringComparison.OrdinalIgnoreCase))
            .Subscribe(x =>
            {
                openTime = DateTime.Now;
            });

        if (entities.BinarySensor.ShopWaterSensorMoisture.IsOn() && openTime == null)
        {
            openTime = DateTime.Now;
        }

        Observable.Interval(TimeSpan.FromHours(1))
        .Subscribe(_ =>
            {
                if (openTime != null)
                {
                    // Check if it's time to turn off the lights
                    if (entities.Cover.BasementGarageDoor.State.Equals("open", StringComparison.OrdinalIgnoreCase)
                    && DateTime.Now - openTime >= warningTimeTimespan)
                    {
                        NotifyOfGarageBeingOpenForOverThreeHours(entities, services);
                        openTime = null;
                    }
                }
                else if (entities.Cover.BasementGarageDoor.State.Equals("open", StringComparison.OrdinalIgnoreCase) && openTime == null)
                {
                    openTime = DateTime.Now;
                }
            });
    }

    private void NotifyOfGarageBeingOpenForOverThreeHours(Entities entities, Services services)
    {
        services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters
        {
            Title = "BASEMENT GARAGE OPEN!",
            Message = "Bsement garage has been open 3 hours."
        });

        services.Notify.MobileAppBrittanysPhone(new NotifyMobileAppBrittanysPhoneParameters
        {
            Title = "BASEMENT GARAGE OPEN!",
            Message = GetRandomGarageOpenNotification()
        });
    }

    private string GetRandomGarageOpenNotification()
    {
        var rand = new Random();
        return FlirtyNotifications.GarageNotifications[rand.Next(0, FlirtyNotifications.GarageNotifications.Count - 1)];
    }
}
