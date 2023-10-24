using ChandlerHome.Helpers.SunPosition;

namespace ChandlerHome.apps.HassModel.Livingroom.Covers
{
    [NetDaemonApp(Id = "Living Room Blinds")]
    internal class LivingRoomBlinds : Livingroom
    {
        private DateTime sunset;
        private DateTime sunsetUtc;
        public LivingRoomBlinds(IHaContext ha, IScheduler scheduler, ILogger<LivingRoomBlinds> logger) : base(ha)
        {            
            _entities ??= new Entities(ha);
            scheduler.ScheduleCron("0 4 * * *", () => GetSunsetTime(logger));

            var services = new Services(ha);


            _entities.InputBoolean.ItsTooDamnHot.StateChanges().Where(e => e.New.IsOn())
                .Subscribe(x =>
                {
                    if (_entities.Cover.LivingRoomBlindsCover.State.Equals("opening", StringComparison.OrdinalIgnoreCase) || _entities.Cover.BlindTilt8ce2.State.Equals("open", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                        {
                            logger.LogInformation("Too Damn Hot! Close Blinds");
                            services.Cover.CloseCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
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
                        var timeDifference = sunset - DateTime.Now;
                        if (timeDifference <= TimeSpan.FromHours(2) && timeDifference > TimeSpan.Zero)
                        {
                            if (_entities.Cover.LivingRoomBlindsCover.State.Equals("closing", StringComparison.OrdinalIgnoreCase) || _entities.Cover.BlindTilt8ce2.State.Equals("closed",StringComparison.OrdinalIgnoreCase))
                            {
                                if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                                {
                                    logger.LogInformation($"Not 2 Hours Before Sunrise, Open Blinds. Sunset: {sunset}, Now: {DateTime.Now}");
                                    services.Cover.OpenCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                                    if (_entities.Cover.BlindTilt8ce2.State.Equals("closing", StringComparison.OrdinalIgnoreCase))
                                        _entities.Cover.BlindTilt8ce2.OpenCover();
                                }
                            }
                        }
                        else if (timeDifference < TimeSpan.Zero)
                        {
                            if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                                if (_entities.Cover.LivingRoomBlindsCover.State.Equals("opening", StringComparison.OrdinalIgnoreCase) || _entities.Cover.BlindTilt8ce2.State.Equals("open", StringComparison.OrdinalIgnoreCase))
                                {
                                    logger.LogInformation($"2 hours before sunset, close blinds.  Sunset: {sunset}, Now: {DateTime.Now}");
                                    services.Cover.CloseCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                                    if (_entities.Cover.BlindTilt8ce2.State.Equals("opening", StringComparison.OrdinalIgnoreCase))
                                        _entities.Cover.BlindTilt8ce2.CloseCover();
                                }
                        }
                    }
                });

            _entities.Sensor.BlindTilt8ce2LightLevel.StateChanges().Where(e => e.New.State.HasValue && e.New.State.Value > 2)
                .Subscribe(x =>
                {
                    if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                    {
                        logger.LogInformation($"Light Level is above 2.  Light Level: {_entities.Sensor.BlindTilt8ce2LightLevel.State}");
                        services.Cover.CloseCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                        if (_entities.Cover.BlindTilt8ce2.State.Equals("opening", StringComparison.OrdinalIgnoreCase))
                            _entities.Cover.BlindTilt8ce2.CloseCover();
                    }
                });
            _entities.Sensor.BlindTilt8ce2LightLevel.StateChanges().Where(e => e.New.State.HasValue && e.New.State.Value <= 2)
                .Subscribe(x =>
                {
                    if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                    {
                        logger.LogInformation($"Light Level is 2 or less.  Light Level: {_entities.Sensor.BlindTilt8ce2LightLevel.State}");
                        services.Cover.OpenCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                        if (_entities.Cover.BlindTilt8ce2.State.Equals("closing", StringComparison.OrdinalIgnoreCase))
                            _entities.Cover.BlindTilt8ce2.OpenCover();
                    }
                });


            SetCurrentState(logger, services);
        }

        private void SetCurrentState(ILogger<LivingRoomBlinds> logger, Services services)
        {
            if (_entities.Sensor.BlindTilt8ce2LightLevel.State.HasValue)
            {
                if (_entities.InputBoolean.LivingRoomOverride.IsOff())
                {
                    if (_entities.Sensor.BlindTilt8ce2LightLevel.State.Value > 2)
                    {
                        logger.LogInformation($"INIT: Light Level is above 2.  Light Level: {_entities.Sensor.BlindTilt8ce2LightLevel.State}");
                        services.Cover.CloseCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
                        if (_entities.Cover.BlindTilt8ce2.State.Equals("opening", StringComparison.OrdinalIgnoreCase))
                            _entities.Cover.BlindTilt8ce2.CloseCover();
                    }
                    else
                    {
                        logger.LogInformation($"INIT: Light Level is 2 or less.  Light Level: {_entities.Sensor.BlindTilt8ce2LightLevel.State}");
                        services.Cover.OpenCover(ServiceTarget.FromEntities(_entities.Cover.LivingRoomBlindsCover.EntityId));
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
            logger.LogInformation($"Sunset results: {sunset}");
        }
    }
}
