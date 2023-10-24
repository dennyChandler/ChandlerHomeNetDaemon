using ChandlerHome.Helpers.Notification_Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.LaundryRoom
{
    [NetDaemonApp(Id = "Washing Machine")]
    internal class WashingMachine
    {
        private bool notified;
        private bool washingMachineRunning;
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

        private void MonitorWashingMachine(Entities entities, IHaContext ha)
        {
            while (washingMachineRunning)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));

                if (entities.Sensor.WashingMachineSwitchElectricConsumptionA.State < 1 && !notified)
                {
                    notified = true;
                    var services = new Services(ha);
                    var notification = GetRandomFlirtyLaundryNotification();
                    entities.InputText.LastLaundryNotification.SetValue(notification);
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

                    washingMachineRunning = false;
                }
            }
        }

        private string GetRandomFlirtyLaundryNotification()
        {
            var rand = new Random();
            return FlirtyNotifications.LaundryNotifications[rand.Next(0, FlirtyNotifications.LaundryNotifications.Count - 1)];
        }
    }
}
