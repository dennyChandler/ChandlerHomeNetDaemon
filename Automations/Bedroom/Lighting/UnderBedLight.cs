//using ChandlerHome.Helpers.WLEDControls;

//namespace ChandlerHome.apps.HassModel.Bedroom.Lighting
//{
//    [NetDaemonApp(Id = "Bedroom WLEDs")]
//    internal class UnderBedLight : Bedroom
//    {
//        public UnderBedLight(IHaContext ha) : base(ha)
//        {
//            _entities ??= new Entities(ha);

//            _entities.BinarySensor.BedroomMotion.StateChanges().Where(e => e.New.IsOn())
//                .SubscribeAsync(async x =>
//                {
//                    if (_entities.Sensor.WeatherstationIlluminance.State < 1000)
//                    {
//                        var controlData = new WledControlData("Rainbow", 100, 100, 255);

//                        await controlData.TurnOnMasterBedroomNightLight();
//                    }
//                });

//            _entities.BinarySensor.BedroomMotion.StateChanges().Where(e => e.New.IsOff())
//                .Subscribe(x =>
//                {
//                    if (_entities.Light.MasterBedroomNightlight.IsOn())
//                        _entities.Light.MasterBedroomNightlight.TurnOff(transition: 5);
//                });

//        }
//    }
//}
