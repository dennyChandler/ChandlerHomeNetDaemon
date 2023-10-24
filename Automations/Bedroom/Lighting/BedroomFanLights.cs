using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Bedroom.Lighting;

[NetDaemonApp(Id = "Bedroom Fan Lights")]
internal class BedroomFanLights : Bedroom
{
    private TimeSpan motionTimeout = TimeSpan.FromMinutes(5);
    DateTime? offTime;
    private Logger<BedroomFanLights> logger;
    public BedroomFanLights(IHaContext ha, ILogger<BedroomFanLights> _logger) : base(ha)
    {
        _entities ??= new Entities(ha);
        logger = (Logger<BedroomFanLights>)_logger;
        var bedroomMotion = _entities.BinarySensor.BedroomMotion;
        var dennysPhoneCharging = _entities.BinarySensor.Pixel8aIsCharging;
        var brittanysPhoneCharging = _entities.BinarySensor.BrittanysPhoneIsCharging;
        var fanLights = _entities.Light.BedroomLights;
        var nightRoutineRun = _entities.InputBoolean.NightRoutineRun;
        var weekdays = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
        var weekend = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };

        bedroomMotion.StateChanges().Where(e => e.New.IsOn() && DateTime.Now.Hour > 20)
            .Subscribe(x =>
            {
                if (!nightRoutineRun.IsOn())
                {
                    if (!IsVacationModeOn())
                    {
                        if (!IsBedroomOverrideOn())
                        {
                            TurnOn(fanLights, 70, 3, kelvin: 2500);
                            if (_entities.MediaPlayer.RokuTv.IsOff())
                            {
                                _entities.MediaPlayer.RokuTv.TurnOn();
                                _entities.MediaPlayer.RokuTv.SelectSource(new MediaPlayerSelectSourceParameters { Source = "Plex - Free Movies & TV" });
                            }

                            _entities.InputBoolean.NightRoutineRun.TurnOn();
                        }
                    }
                }
            });

        bedroomMotion.StateChanges().Where(e => e.New.IsOn() && weekdays.Contains(DateTime.Now.DayOfWeek) && DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 9)
           .SubscribeAsync(async x =>
           {
               if ((dennysPhoneCharging.IsOn() && _entities.Person.DennyChandler.State.Equals("home", StringComparison.OrdinalIgnoreCase)) || (brittanysPhoneCharging.IsOn() && _entities.Person.BrittanyChandler.State.Equals("home", StringComparison.OrdinalIgnoreCase) && DateTime.Now.Hour < 7))
               {
                   if (!IsVacationModeOn())
                   {
                       if (!IsBedroomOverrideOn())
                       {
                           TurnLightOn(fanLights, 5, 3, kelvin: 2500);
                           await Task.Delay(TimeSpan.FromMinutes(3));
                           TurnLightOff(fanLights, 3);
                       }
                   }
               }
               else if (dennysPhoneCharging.IsOff() && brittanysPhoneCharging.IsOff())
               {
                   if (!IsVacationModeOn())
                   {
                       if (!IsBedroomOverrideOn())
                       {
                           TurnLightOn(fanLights, 100, 3);
                       }
                   }
               }
           });

        bedroomMotion.StateChanges().Where(e => e.New.IsOn() && weekend.Contains(DateTime.Now.DayOfWeek) && DateTime.Now.Hour >= 7 && DateTime.Now.Hour < 10)
           .SubscribeAsync(async x =>
           {
               if ((dennysPhoneCharging.IsOn() && _entities.Person.DennyChandler.State.Equals("home", StringComparison.OrdinalIgnoreCase)) || (brittanysPhoneCharging.IsOn() && _entities.Person.BrittanyChandler.State.Equals("home", StringComparison.OrdinalIgnoreCase) && DateTime.Now.Hour < 7))
               {
                   if (!IsVacationModeOn())
                   {
                       if (!IsBedroomOverrideOn())
                       {
                           TurnLightOn(fanLights, 5, 3, kelvin: 2500);
                           await Task.Delay(TimeSpan.FromMinutes(3));
                           TurnLightOff(fanLights, 3);
                       }
                   }
               }
               else if (dennysPhoneCharging.IsOff() && brittanysPhoneCharging.IsOff())
               {
                   if (!IsVacationModeOn())
                   {
                       if (!IsBedroomOverrideOn())
                       {
                           TurnLightOn(fanLights, 100, 3);
                       }
                   }
               }
           });

        bedroomMotion.StateChanges().Where(e => e.New.IsOn()
        && DateTime.Now.Hour >= 9
        && DateTime.Now.Hour <= 20)
           .Subscribe(x =>
           {
               TurnLightOn(fanLights, 100, 3, kelvin: 2500);
           });

        bedroomMotion.StateChanges().Where(e => e.New.IsOff() && DateTime.Now.Hour < 20 && DateTime.Now.Hour > 7)
            .Subscribe(x =>
            {
                if (_entities.MediaPlayer.RokuTv.IsOff())
                {
                    TurnLightOff(fanLights, 3);
                }
            });


        Observable.Interval(TimeSpan.FromMinutes(1))
        .Subscribe(_ =>
            {
                if (offTime != null)
                {
                    var timeDiff = DateTime.Now - offTime;
                    // Check if it's time to turn off the lights
                    if (_entities.Light.BedroomLights.IsOn()
                    && timeDiff >= motionTimeout
                    && _entities.BinarySensor.BedroomMotion.IsOff()
                    && IsOutsideTimeRange(DateTime.Now, logger))
                    {
                        TurnLightOff(_entities.Light.BedroomLights, transition: 60);
                        offTime = null;
                        logger.LogDebug($"FanLightsOff: Time Difference: {timeDiff.ToString()}, State: {_entities.BinarySensor.BedroomMotion.State}, DateTIme.Now: {DateTime.Now}");
                    }
                }
                else if (offTime == null && _entities.BinarySensor.BedroomMotion.IsOff() && _entities.Light.BedroomLights.IsOn() && IsOutsideTimeRange(DateTime.Now, logger))
                {
                    offTime = DateTime.Now;
                }
                if (nightRoutineRun.IsOn())
                    if (DateTime.Now.Hour < 18)
                        nightRoutineRun.TurnOff();
            });
    }

    private bool IsOutsideTimeRange(DateTime currentTime, Logger<BedroomFanLights> logger)
    {
        int hour = currentTime.Hour;
        logger.LogDebug($"IsOutsideTimeRange Hour: {hour}");
        logger.LogDebug($"{!(hour >= 20)}");
        // Check if the time is not between 8 PM and midnight
        return !(hour >= 20);
    }
}

