using ChandlerHome.Helpers.Notification_Messages;
using System.Threading.Tasks;

namespace ChandlerHome.Automations.LaundryRoom;

[NetDaemonApp(Id = "Dryer")]
internal class Dryer
{
    private bool notified;
    private bool dryerIsRunning;
    public Dryer(IHaContext ha)
    {
        var entities = new Entities(ha);

        entities.BinarySensor.DryerVibration.StateChanges().Where(e => e.New.IsOn())
            .SubscribeAsync(async x =>
            {
                dryerIsRunning = true;
                notified = false;
                await MonitorDryer(entities, ha);
            });
    }

    private async Task MonitorDryer(Entities entities, IHaContext ha)
    {
        while (dryerIsRunning)
        {
            await Task.Delay(TimeSpan.FromMinutes(1));

            if (entities.Sensor.WashingMachineSwitchElectricConsumptionA.State < 1 && !notified)
            {
                var services = new Services(ha);
                services.Notify.MobileAppBrittanysPhone(new NotifyMobileAppBrittanysPhoneParameters
                {
                    Title = "Washer is done!",
                    Message = GetRandomFlirtyDryerNotification(),
                    Target = ""
                });
                services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters
                {
                    Title = "Dryer is done!",
                    Message = "",
                    Target = ""
                });

                dryerIsRunning = false;
            }
        }
    }

    private string GetRandomFlirtyDryerNotification()
    {
        var rand = new Random();
        return FlirtyNotifications.DryerNotifications[rand.Next(0, FlirtyNotifications.DryerNotifications.Count - 1)];
    }
}
