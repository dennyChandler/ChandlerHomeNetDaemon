using ChandlerHome.Helpers.Sloganizer;
using System.Net.Http;
using System.Threading;

namespace ChandlerHome.apps.HassModel.Utilities.Climate;

[NetDaemonApp(Id = "Climate Automations")]
internal class ClimateAutomations : Home
{
    Logger<ClimateAutomations>? logger;
    DateTime? lastTurnOffTime;
    public ClimateAutomations(IHaContext ha, ILogger<ClimateAutomations> _logger, IScheduler scheduler) : base(ha)
    {
        var entities = new Entities(ha);
        logger = (Logger<ClimateAutomations>?)_logger;


        entities.BinarySensor.BackDoorsOpen.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                if (!IsVacationModeOn())
                {
                    if (_entities.InputBoolean.JointInBed.IsOff())
                    {
                        logger.LogTrace($"Starting open door check.");
                        StartDoorOpenCheck(entities, new Services(ha));
                    }
                }
            });
        entities.Switch.AtticFanHigh.StateChanges().Where(x => x.New.IsOff()).Subscribe(x =>
        {
            lastTurnOffTime = DateTime.Now;
        });

        entities.BinarySensor.AtticFanRequiredOpenings.StateChanges().Where(e => e.New.IsOff())
            .Subscribe(x =>
            {
                if (!IsVacationModeOn())
                {
                    if (_entities.Switch.AtticFanHigh.IsOn())
                        _entities.Switch.AtticFanHigh.TurnOff();
                    if (_entities.Switch.AtticFanLow.IsOn())
                        _entities.Switch.AtticFanLow.TurnOff();
                }
            });

        entities.Sensor.WeatherstationTemperature.StateChanges().Where(x => x.New.State < entities.Sensor.LivingRoomTemperature.State && AtticFanRequirementsMet(entities))
            .Subscribe(x =>
            {
                var services = new Services(ha);
                var sloganizerOptions = new SloganizerOptions(new HttpClient(), "http://www.sloganizer.net");
                var sloganizer = new Sloganizer(sloganizerOptions);

                if (!IsVacationModeOn())
                    if (entities.Climate.LivingRoom.EntityState.State.Equals("heat_cool", StringComparison.OrdinalIgnoreCase) || entities.Climate.LivingRoom.EntityState.State.Equals("cool", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTime.Now.Hour > 9 && DateTime.Now.Hour < 20 && entities.InputBoolean.TempNotificationSent.IsOff())
                        {
                            var message = sloganizer.GetSlogan("Saving Money");
                            services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters
                            {
                                Title = $"{message.Result}",
                                Message = "You should turn off the A/C and open windows to save money <3"
                            });
                            entities.InputBoolean.TempNotificationSent.TurnOn();
                        }
                    }
                    else if (entities.Climate.LivingRoom.IsOff())
                    {
                        if (!IsVacationModeOn())
                            RunAtticFanCheck(entities, services, sloganizer);
                    }
            });

        entities.InputBoolean.TempNotificationSent.StateChanges().WhenStateIsFor(s => s?.State == "on", TimeSpan.FromHours(3), scheduler)
            .Subscribe(x =>
            {
                entities.InputBoolean.TempNotificationSent.TurnOff();
            });

        entities.Sensor.WeatherstationTemperature.StateChanges().Where(x => x.New.State > entities.Sensor.LivingRoomTemperature.State)
           .Subscribe(x =>
           {
               entities.InputBoolean.TempNotificationSent.TurnOff();
           });

        entities.BinarySensor.HomeBinarySensorsIsRaining.StateChanges().Where(x => x.New.IsOn() && (entities.Switch.AtticFanHigh.IsOn() || entities.Switch.AtticFanLow.IsOn()))
         .Subscribe(x =>
         {
             var services = new Services(ha);
             var sloganizerOptions = new SloganizerOptions(new HttpClient(), "http://www.sloganizer.net");
             var sloganizer = new Sloganizer(sloganizerOptions);

             var message = sloganizer.GetSlogan("Rain");
             services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters
             {
                 Title = $"{message.Result}",
                 Message = "Attic fan off due to rain, consider shutting windows."
             });

             if (entities.Switch.AtticFanHigh.IsOn())
                 entities.Switch.AtticFanHigh.TurnOff();
             else
                 entities.Switch.AtticFanLow.TurnOff();
         });

        Observable.Interval(TimeSpan.FromMinutes(5))
            .Subscribe(_ =>
            {
                if (entities.BinarySensor.AtticFanRequiredOpenings.IsOn() && !IsVacationModeOn())
                {
                    var services = new Services(ha);
                    var sloganizerOptions = new SloganizerOptions(new HttpClient(), "http://www.sloganizer.net");
                    var sloganizer = new Sloganizer(sloganizerOptions);

                    RunAtticFanCheck(entities, services, sloganizer);
                }
            });
    }

    private bool AtticFanRequirementsMet(Entities entities)
    {
        if (entities.Switch.AtticFanHigh.IsOff() && entities.Switch.AtticFanLow.IsOff() && entities.InputBoolean.AtticFanOverride.IsOff())
        {
            if (entities.Sensor.WeatherflowHumidity.State <= entities.InputNumber.MaxOutdoorHumidity.State
            && entities.Sensor.WeatherflowTemperature.State <= entities.InputNumber.MaxOutdoorTemperature.State
            && entities.Sensor.WeatherflowTemperature.State >= entities.InputNumber.MinOutdoorTemperature.State)
                return true;
        }
        return false;
    }
    private void StartDoorOpenCheck(Entities entities, Services services)
    {
        Thread.Sleep(TimeSpan.FromMinutes(5));
        if (_entities.InputBoolean.JointInBed.IsOn())
            return;

        if (entities.BinarySensor.BackDoorsOpen.IsOn()
                && entities.Sensor.WeatherstationTemperature.State > 60
                && entities.Sensor.WeatherstationTemperature.State < 90)
        {
            logger.LogInformation($"turning off AC: {entities.BinarySensor.BackDoorsOpen.IsOn()}, {entities.Sensor.WeatherstationTemperature.State}");
            var sloganizer = new Sloganizer(new SloganizerOptions(new System.Net.Http.HttpClient(), "http://www.sloganizer.net"));

            entities.Climate.LivingRoom.TurnOff();
            var slogan = sloganizer.GetSlogan("The Outdoors");

            services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters()
            {
                Title = "AC turned off!",
                Message = $"{slogan.Result}"
            });

            RunAtticFanCheck(entities, services, sloganizer);
        }
        else if (entities.BinarySensor.BackDoorsOpen.IsOn()
        && (entities.Sensor.WeatherstationTemperature.State < 60
        || entities.Sensor.WeatherstationTemperature.State > 90))
        {
            Thread.Sleep(TimeSpan.FromMinutes(5));

            if (entities.BinarySensor.BackDoorsOpen.IsOn()
                    && entities.Sensor.WeatherflowTemperature.State > 60
                    && entities.Sensor.WeatherflowTemperature.State < 90)
            {
                logger.LogInformation($"turning off AC: {entities.BinarySensor.BackDoorsOpen.IsOn()}, {entities.Sensor.WeatherflowTemperature.State}");
                var sloganizer = new Sloganizer(new SloganizerOptions(new System.Net.Http.HttpClient(), "http://www.sloganizer.net"));

                entities.Climate.LivingRoom.TurnOff();
                var slogan = sloganizer.GetSlogan("The Outdoors");

                services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters()
                {
                    Title = "AC turned off!",
                    Message = $"{slogan.Result}"
                });

                RunAtticFanCheck(entities, services, sloganizer);
            }
            else if (entities.BinarySensor.BackDoorsOpen.IsOn()
            && (entities.Sensor.WeatherflowTemperature.State < 60
            || entities.Sensor.WeatherflowTemperature.State > 90))
            {
                services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters()
                {
                    Title = "Back Doors Open!",
                    Message = "It's either too hot or too cold and the back doors have been open over 5 minutes!"
                });
                logger.LogInformation($"not turning AC off: {entities.BinarySensor.BackDoorsOpen.IsOn()}, {entities.Sensor.WeatherflowTemperature.State}");
            }
        }
    }

    private void RunAtticFanCheck(Entities entities, Services services, Sloganizer sloganizer)
    {
        if (entities.Switch.AtticFanHigh.IsOff() && entities.Switch.AtticFanLow.IsOff() && entities.InputBoolean.AtticFanOverride.IsOff())
        {
            if (lastTurnOffTime != null)
            {
                if (lastTurnOffTime - DateTime.Now > TimeSpan.FromHours(3))
                {
                    if (entities.Sensor.WeatherflowHumidity.State <= entities.InputNumber.MaxOutdoorHumidity.State
                && entities.Sensor.WeatherflowTemperature.State <= entities.InputNumber.MaxOutdoorTemperature.State
                && entities.Sensor.WeatherflowTemperature.State >= entities.InputNumber.MinOutdoorTemperature.State)
                    {
                        entities.Switch.AtticFanHigh.TurnOn();
                        var fanSlogan = sloganizer.GetSlogan("Attic Fan");
                        entities.InputBoolean.TempNotificationSent.TurnOff();

                        services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters()
                        {
                            Title = "Attic Fan On!",
                            Message = $"{fanSlogan.Result}"
                        });
                    }
                }
            }
            else
            {
                if (entities.Sensor.WeatherflowHumidity.State <= entities.InputNumber.MaxOutdoorHumidity.State
                    && entities.Sensor.WeatherflowTemperature.State <= entities.InputNumber.MaxOutdoorTemperature.State
                    && entities.Sensor.WeatherflowTemperature.State >= entities.InputNumber.MinOutdoorTemperature.State)
                {
                    entities.Switch.AtticFanHigh.TurnOn();
                    var fanSlogan = sloganizer.GetSlogan("Attic Fan");
                    entities.InputBoolean.TempNotificationSent.TurnOff();

                    services.Notify.FamilyPhones(new NotifyFamilyPhonesParameters()
                    {
                        Title = "Attic Fan On!",
                        Message = $"{fanSlogan.Result}"
                    });
                }
            }
        }
    }
}
