using System.Xml;

namespace ChandlerHome.apps.HassModel.Bedroom
{
    [NetDaemonApp(Id = "Bedroom Tub Light")]
    public class TubLight : Bedroom
    {
        public TubLight(IHaContext ha) : base(ha)
        {
            if (_entities == null)
                _entities = new Entities(ha);

            var tubLight = _entities.Light.MasterBathroomTubLight;
            var sunState = _entities.Sun.Sun;
            var dennysPhoneCharging = _entities.BinarySensor.DennysPhoneIsCharging;

            sunState
            .StateChanges()
                .Where(e => e.New?.State?.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) ?? false && (tubLight.State?.Equals("off", StringComparison.OrdinalIgnoreCase) ?? true))
                .Subscribe(x =>
                {
                    if (tubLight.IsOff() && (dennysPhoneCharging.State?.Equals("off") ?? true))
                        TurnLightOn(tubLight, 100, 3, doesOverrideMatter: false);
                });

            dennysPhoneCharging.StateChanges().Where(e => e.New?.IsOff() ?? false)
                .Subscribe(x =>
                {
                    if (tubLight.IsOff() && (sunState.State?.Equals("above_horizon", StringComparison.OrdinalIgnoreCase) ?? false))
                        TurnLightOn(tubLight, 100, 3, doesOverrideMatter: false);
                });

            sunState
            .StateChanges()
                .Where(e => e.New?.State?.Equals("below_horizon", StringComparison.OrdinalIgnoreCase) ?? false)
                 .Subscribe(x =>
                {
                    if (tubLight.IsOn())
                        TurnLightOff(tubLight, 300, false);
                });

            _entities.InputBoolean.JointInBed.StateChanges().Where(e => e.New?.State?.Equals("on") ?? false)
                .Subscribe(x =>
                {
                    TurnLightOn(tubLight, 100, 3, "purple", false);
                });

            _entities.InputBoolean.JointInBed.StateChanges().Where(e => e.New?.State?.Equals("off") ?? false)
                .Subscribe(x =>
                {
                    if (_entities.Sensor.WeatherflowBrightness.State > 2500)
                        TurnLightOn(tubLight, 100, 3, colorName: "white", doesOverrideMatter: false);
                    else
                        TurnLightOff(tubLight, 10, false);
                });

        }
    }
}
