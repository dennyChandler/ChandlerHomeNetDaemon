namespace ChandlerHome.apps.HassModel.Office.Covers;

[NetDaemonApp(Id = "Dennys Office Blinds")]
internal class OfficeBlinds : Office
{
    private bool blindsShouldClose;
    private bool blindsShouldOpen;
    public OfficeBlinds(IHaContext ha) : base(ha)
    {
        if (_entities == null)
            _entities = new Entities(ha);

        var blindsOpenToggle = _entities.InputBoolean.DennysOfficeBlindsOpen;
        var blindsCloseToggle = _entities.InputBoolean.DennysOfficeBlindsShut;
        var blindsHalfToggle = _entities.InputBoolean.OfficeBlindsHalf;
        var blindsEntity = _entities.Cover.DennysOfficeBlindsCover;
        var sun = _entities.Sun.Sun;

        sun.StateChanges().Where(e => e.New?.State?.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) ?? false
        && _entities.Sensor.WeatherstationTemperature.State < 85)
            .Subscribe(x =>
            {
                if (blindsEntity.Attributes.CurrentPosition.HasValue
                && blindsEntity.Attributes.CurrentPosition.Value < 50
                && _entities.InputBoolean.DennysOfficeOverride.IsOff()
                && _entities.InputBoolean.ItsTooDamnHot.IsOff())
                    blindsOpenToggle.TurnOn();
                else
                    blindsShouldOpen = true;
            });

        sun.StateChanges().Where(e => e.New?.State?.Equals("below_horizon", StringComparison.OrdinalIgnoreCase) ?? false)
            .Subscribe(x =>
            {
                if (blindsEntity.Attributes.CurrentPosition.HasValue
                && blindsEntity.Attributes.CurrentPosition.Value > 50
                && _entities.InputBoolean.DennysOfficeOverride.IsOff()
                && _entities.BinarySensor.DennyOfficeWindow.IsOff())
                    blindsCloseToggle.TurnOn();
                else
                    blindsShouldClose = true;

            });

        if (sun.State.Equals("below_horizon", StringComparison.OrdinalIgnoreCase)
            && blindsEntity.Attributes.CurrentPosition.Value > 50
            && _entities.BinarySensor.DennyOfficeWindow.IsOff()
            && _entities.InputBoolean.DennysOfficeOverride.IsOff())
        {
            blindsCloseToggle.TurnOn();
        }

        blindsCloseToggle.StateChanges().Where(e => e.New?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false)
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.DennyOfficeWindow.IsOn())
                {
                    var services = new Services(ha);
                    services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters
                    {
                        Message = "Your office window is open and the blinds want to close!",
                        Title = "Close your office window!"
                    });
                    return;
                }
                CloseBlinds(blindsEntity, 27);
                blindsEntity.SetCoverPosition(new CoverSetCoverPositionParameters { Position = 27 });
                blindsCloseToggle.TurnOff();
            });

        blindsOpenToggle.StateChanges().Where(e => e.New?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false)
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.DennyOfficeWindow.IsOff() && _entities.InputBoolean.DennysOfficeOverride.IsOff() && _entities.InputBoolean.ItsTooDamnHot.IsOff())
                {
                    blindsEntity.SetCoverPosition(new CoverSetCoverPositionParameters { Position = 97 });
                }

                blindsOpenToggle.TurnOff();
            });

        blindsHalfToggle.StateChanges().Where(e => e.New?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false)
            .Subscribe(x =>
            {
                if (_entities.BinarySensor.DennyOfficeWindow.IsOff())
                {
                    blindsEntity.SetCoverPosition(new CoverSetCoverPositionParameters { Position = 68 });
                    blindsHalfToggle.TurnOff();
                }
            });

        Observable.Interval(TimeSpan.FromMinutes(1))
           .Subscribe(_ =>
           {
               if (blindsShouldClose && sun.State.Equals("below_horizon", StringComparison.OrdinalIgnoreCase))
               {
                   if (_entities.BinarySensor.DennyOfficeWindow.IsOff())
                       blindsCloseToggle.TurnOn();

               }
               else if (blindsShouldClose)
                   blindsShouldClose = false;
               if (blindsShouldOpen && sun.State.Equals("above_horizon", StringComparison.OrdinalIgnoreCase))
               {
                   if (_entities.BinarySensor.DennyOfficeWindow.IsOff())
                       blindsOpenToggle.TurnOn();
                   else blindsShouldOpen = false;
               }
               else if (blindsShouldOpen)
                   blindsShouldOpen = false;
           });
    }


}
