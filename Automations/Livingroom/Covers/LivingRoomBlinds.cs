using ChandlerHome.Helpers.SunPosition;

namespace ChandlerHome.Automations.Livingroom.Covers;

[NetDaemonApp(Id = "Living Room Blinds")]
internal class LivingRoomBlinds : Livingroom
{
    private DateTime sunset;
    private DateTime sunsetUtc;
    private bool sunsetShut;
    private DateTime sunsetMinusTwo;
    private DateTime? brightnessOverFifty;
    private DateTime? nullTime;
    public LivingRoomBlinds(IHaContext ha, IScheduler scheduler, ILogger<LivingRoomBlinds> logger) : base(ha)
    {
        _entities ??= new Entities(ha);
        scheduler.ScheduleCron("0 4 * * *", () => GetSunsetTime(logger));

        var services = new Services(ha);

        //Services.OpenCover/CloseCover reversed because this zigbee wand flipped directions at some point.

        _entities.InputBoolean.ItsTooDamnHot.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                if (_entities.Cover.LivingRoomBlindsCover.State.Equals("opening", StringComparison.OrdinalIgnoreCase) || _entities.Cover.BlindTilt8ce2.State.Equals("open", StringComparison.OrdinalIgnoreCase))
                {
                    if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                    {
                        logger.LogInformation("Too Damn Hot! Close Blinds");
                        services.Cover.OpenCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                        if (_entities.Cover.BlindTilt8ce2.State.Equals("opening", StringComparison.OrdinalIgnoreCase))
                            _entities.Cover.BlindTilt8ce2.CloseCover();
                    }
                }
            });

        Observable.Interval(TimeSpan.FromHours(1))
        .Subscribe(_ =>
            {
                if (!IsItTooDamnHot())
                {
                    if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                    {
                        if (DateTime.Now < sunsetMinusTwo || DateTime.Now > sunset.AddHours(-1))
                        {
                            if (_entities.Cover.LivingRoomBlindsCover.State.Equals("closing", StringComparison.OrdinalIgnoreCase) || _entities.Cover.BlindTilt8ce2.State.Equals("closed", StringComparison.OrdinalIgnoreCase) && sunsetShut)
                            {
                                if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                                {
                                    sunsetShut = false;
                                    logger.LogDebug($"Not 2 Hours Before Sunrise, Open Blinds. Sunset: {sunset}, Now: {DateTime.Now}");

                                    services.Cover.CloseCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                                    if (_entities.Cover.BlindTilt8ce2.State.Equals("closing", StringComparison.OrdinalIgnoreCase))
                                        _entities.Cover.BlindTilt8ce2.OpenCover();
                                }
                            }
                        }
                        else if (sunsetMinusTwo - DateTime.Now < TimeSpan.Zero)
                        {
                            if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                                if (_entities.Cover.LivingRoomBlindsCover.State.Equals("opening", StringComparison.OrdinalIgnoreCase) || _entities.Cover.BlindTilt8ce2.State.Equals("open", StringComparison.OrdinalIgnoreCase) && !sunsetShut)
                                {
                                    sunsetShut = true;
                                    logger.LogInformation($"2 hours before sunset, close blinds.  Sunset: {sunset}, Now: {DateTime.Now}");
                                    services.Cover.OpenCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                                    if (_entities.Cover.BlindTilt8ce2.State.Equals("opening", StringComparison.OrdinalIgnoreCase))
                                        _entities.Cover.BlindTilt8ce2.CloseCover();
                                }
                        }
                    }
                }
            });

        Observable.Interval(TimeSpan.FromMinutes(10))
            .Subscribe(_ =>
            {
                if (!IsItTooDamnHot())
                {
                    if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                    {
                        if (brightnessOverFifty != null)
                        {
                            var timeDiff = brightnessOverFifty - DateTime.Now;
                            if (Math.Abs(timeDiff.Value.TotalMinutes) > 10) //Brightness over 50 for 10 minutes
                            {
                                services.Cover.OpenCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                                if (_entities.Cover.BlindTilt8ce2.State.Equals("opening", StringComparison.OrdinalIgnoreCase))
                                    _entities.Cover.BlindTilt8ce2.CloseCover();
                            }
                        }
                        if (brightnessOverFifty == null)
                        {
                            if (nullTime == null)
                                nullTime = DateTime.Now;

                            var timeDiff = nullTime.Value - DateTime.Now;
                            if (Math.Abs(timeDiff.TotalMinutes) > 10)
                            {
                                services.Cover.CloseCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                                if (_entities.Cover.BlindTilt8ce2.State.Equals("closing", StringComparison.OrdinalIgnoreCase))
                                    _entities.Cover.BlindTilt8ce2.OpenCover();
                            }
                        }
                    }
                }
            });

        _entities.Sensor.LumiLumiSensorMotionAq2Illuminance.StateChanges().Where(e => e.New.State.HasValue && e.New.State.Value >= 50)
            .Subscribe(x =>
            {
                if (brightnessOverFifty == null)
                    brightnessOverFifty = DateTime.Now;
            });
        _entities.Sensor.LumiLumiSensorMotionAq2Illuminance.StateChanges().Where(e => e.New.State.HasValue && e.New.State.Value < 50)
            .Subscribe(x =>
            {
                brightnessOverFifty = null;
            });


        SetCurrentState(logger, services);
    }

    private void SetCurrentState(ILogger<LivingRoomBlinds> logger, Services services)
    {
        if (_entities.Sensor.LumiLumiSensorMotionAq2Illuminance.State.HasValue)
        {
            if (_entities.InputBoolean.LivingRoomOverride.IsOff())
            {
                if (_entities.Sensor.LumiLumiSensorMotionAq2Illuminance.State.Value >= 50)
                {
                    logger.LogInformation($"INIT: Light Level is at or above 50.  Light Level: {_entities.Sensor.LumiLumiSensorMotionAq2Illuminance.State}");
                    services.Cover.OpenCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                    if (_entities.Cover.BlindTilt8ce2.State.Equals("opening", StringComparison.OrdinalIgnoreCase))
                        _entities.Cover.BlindTilt8ce2.CloseCover();
                }
                else
                {
                    logger.LogInformation($"INIT: Light Level is less than 50.  Light Level: {_entities.Sensor.LumiLumiSensorMotionAq2Illuminance.State}");
                    services.Cover.CloseCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                    if (_entities.Cover.BlindTilt8ce2.State.Equals("closing", StringComparison.OrdinalIgnoreCase))
                        _entities.Cover.BlindTilt8ce2.OpenCover();
                }
            }
        }

        if (sunset.Equals(new DateTime(1, 1, 1)))
            GetSunsetTime(logger);
    }

    private void GetSunsetTime(ILogger<LivingRoomBlinds> logger)
    {
        var sunriseSunset = new SunriseSunset(new Logger<SunriseSunset>(new LoggerFactory()));
        var response = sunriseSunset.GetSunriseSunsetInfo();
        sunsetUtc = DateTime.SpecifyKind(DateTime.Parse(response.Result.results.sunset), DateTimeKind.Utc);
        sunset = sunsetUtc.ToLocalTime();
        sunsetMinusTwo = sunsetUtc.AddHours(-2).ToLocalTime();
        logger.LogInformation($"Sunset results: {sunset}");
    }
}
