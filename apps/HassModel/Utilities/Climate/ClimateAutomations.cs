using ChandlerHome.Helpers.Sloganizer;
using System.Threading;

namespace ChandlerHome.apps.HassModel.Utilities.Climate
{
    [NetDaemonApp(Id = "Climate Automations")]
    internal class ClimateAutomations : Home
    {
        Logger<ClimateAutomations>? logger;
        public ClimateAutomations(IHaContext ha, ILogger<ClimateAutomations> _logger) : base(ha)
        {
            var entities = new Entities(ha);
            logger = (Logger<ClimateAutomations>?)_logger;

            entities.BinarySensor.BackDoorsOpen.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    logger.LogInformation($"Starting open door check.");
                    StartDoorOpenCheck(entities, new Services(ha));                    
                });
        }

        private void StartDoorOpenCheck(Entities entities, Services services)
        {
            Thread.Sleep(TimeSpan.FromMinutes(5));

            if (entities.BinarySensor.BackDoorsOpen.IsOn()
                    && entities.Sensor.WeatherflowAirTemperature.State > 60
                    && entities.Sensor.WeatherflowAirTemperature.State < 90)
            {
                logger.LogInformation($"turning off AC: {entities.BinarySensor.BackDoorsOpen.IsOn()}, {entities.Sensor.WeatherflowAirTemperature.State}");
                var sloganizer = new Sloganizer(new SloganizerOptions(new System.Net.Http.HttpClient(), "http://www.sloganizer.net"));

                entities.Climate.LivingRoom.TurnOff();
                var slogan = sloganizer.GetSlogan("Attic Fan");
                
                services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters()
                {
                    Title = "AC turned off!",
                    Message = $"{slogan.Result}"
                });
            }
            else if (entities.BinarySensor.BackDoorsOpen.IsOn()
            && entities.Sensor.WeatherflowAirTemperature.State < 60
            && entities.Sensor.WeatherflowAirTemperature.State > 90)
            {
                services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters()
                {
                    Title = "Back Doors Open!",
                    Message = "It's either too hot or too cold and the back doors have been open over 5 minutes!"
                });
                logger.LogInformation($"not turning AC off: {entities.BinarySensor.BackDoorsOpen.IsOn()}, {entities.Sensor.WeatherflowAirTemperature.State}");
            }            
        }
    }
}
