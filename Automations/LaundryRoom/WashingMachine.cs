using ChandlerHome.Helpers.Notification_Messages;
using ChandlerHome.Models.ResponseModels;
using Serilog;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace ChandlerHome.Automations.LaundryRoom;

[NetDaemonApp(Id = "Washing Machine")]
internal class WashingMachine
{
    private bool notified;
    private bool washingMachineRunning;
    private NsfwResponse nsfw;
    private const string nightApiKey = @"EOgwS8fRhX-My9vOHb4dqfUNq54J0db-onewUioItC";
    public WashingMachine(IHaContext ha)
    {
        var entities = new Entities(ha);

        entities.Sensor.WashingMachineSwitchElectricConsumptionA.StateChanges().Where(e => e.New.State > 1)
            .Subscribe(x =>
            {
                washingMachineRunning = true;
                notified = false;
                MonitorWashingMachine(entities, ha);
            });
    }

    private async Task MonitorWashingMachine(Entities entities, IHaContext ha)
    {
        while (washingMachineRunning)
        {
            Thread.Sleep(TimeSpan.FromMinutes(1));

            if (entities.Sensor.WashingMachineSwitchElectricConsumptionA.State < .1 && !notified)
            {
                notified = true;
                var services = new Services(ha);
                var notification = GetRandomFlirtyLaundryNotification();
                entities.InputText.LastLaundryNotification.SetValue(notification);
                dynamic nsfwInfo = new ExpandoObject();
                nsfwInfo.image = nsfw.content.url;
                services.Notify.MobileAppBrittanysPhone(new NotifyMobileAppBrittanysPhoneParameters
                {
                    Title = "Washer is done!",
                    Message = notification,
                    Target = ""
                });
                services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters
                {
                    Title = "Washer is done!",
                    Message = "Get the clothes done!",
                    Target = ""
                });
                ColorHallwayNotification(entities, "green");

            }
            washingMachineRunning = false;
        }
    }


    private static void ColorHallwayNotification(Entities entities, string colorName)
    {
        if (entities.BinarySensor.NurseryDoor.IsOn() && entities.BinarySensor.OctaviasBedtime.IsOff())
            entities.Light.ZigbeeStickGroupsUpstairsColorLights.TurnOn(effect: "blink", colorName: colorName, brightnessPct: 100);
        entities.Light.ZigbeeStickGroupsMainFloorHall.TurnOn(effect: "blink", colorName: colorName, brightnessPct: 100);
        Thread.Sleep(10000);
        if (entities.BinarySensor.NurseryDoor.IsOn() && entities.BinarySensor.OctaviasBedtime.IsOff())
            entities.Light.ZigbeeStickGroupsUpstairsColorLights.TurnOff();
        entities.Light.ZigbeeStickGroupsMainFloorHall.TurnOff();
    }

    private string GetRandomFlirtyLaundryNotification()
    {
        var rand = new Random();
        return FlirtyNotifications.LaundryNotifications[rand.Next(0, FlirtyNotifications.LaundryNotifications.Count - 1)];
    }
}
