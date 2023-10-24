using ChandlerHome.Helpers.Sloganizer;
using System.Net.Http;
using System.Threading;

namespace ChandlerHome.apps.HassModel.FrontOfHouse;

[NetDaemonApp(Id = "Mailbox")]
internal class Mail
{
    public Mail(IHaContext ha)
    {
        var sloganizerOptions = new SloganizerOptions(new HttpClient(), "http://www.sloganizer.net");
        var entities = new Entities(ha);

        entities.BinarySensor.Mailbox.StateChanges().Where(e => e.New.IsOn()) //mailbox opens
            .SubscribeAsync(async x =>
            {
                var services = new Services(ha);
                var sloganizer = new Sloganizer(sloganizerOptions);
                var message = await sloganizer.GetSlogan("Mail");
                services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters
                {
                    Message = $"{message}",
                    Title = $"You've Got Mail!"
                });

                if (entities.Light.FrontOfHouseLights.IsOff())
                {
                    entities.Light.FrontOfHouseLights.TurnOn();
                    Thread.Sleep(250);
                    entities.Light.FrontOfHouseLights.TurnOff();
                    Thread.Sleep(500);
                    entities.Light.FrontOfHouseLights.TurnOn();
                    Thread.Sleep(250);
                    entities.Light.FrontOfHouseLights.TurnOff();
                }
                else
                {
                    entities.Light.FrontOfHouseLights.TurnOn();
                    Thread.Sleep(250);
                    entities.Light.FrontOfHouseLights.TurnOff();
                    Thread.Sleep(500);
                    entities.Light.FrontOfHouseLights.TurnOn();
                    Thread.Sleep(250);
                }

                return;
            });
    }
}
