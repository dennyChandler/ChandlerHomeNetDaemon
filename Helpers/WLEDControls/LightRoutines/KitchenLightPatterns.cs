using ChandlerHome.apps.HassModel.Kitchen;

namespace ChandlerHome.Helpers.WLEDControls.LightRoutines
{
    public class KitchenLightPatterns : Kitchen
    {
        public KitchenLightPatterns(IHaContext ha) : base(ha)
        {
            _entities ??= new Entities(ha);
        }

        internal async void TornadoWarningLights()
        {
            var controlData = new WledControlData("Scan Dual", 100, 100, 255);
            await controlData.TurnOnKitchenWledLight();
        }

        internal async void FreezeWarningLights()
        {
            var controlData = new WledControlData("Colortwinkles", 100, 100, 255);
            await controlData.TurnOnKitchenWledLight();
        }

        internal async void RedFlagWarningLights()
        {
            var controlData = new WledControlData("Fire 2012", 20, 30, 255);
            await controlData.TurnOnKitchenWledLight();
        }

        internal async void HeatWarningLights()
        {
            var controlData = new WledControlData("Fire Flicker", 100, 100, 255);
            await controlData.TurnOnKitchenWledLight();
        }

        internal async void FloodWarningLights()
        {
            var controlData = new WledControlData("Ripple", 100, 100, 255);
            await controlData.TurnOnKitchenWledLight();
        }

        internal async void ThunderstormWarningLights()
        {
            var controlData = new WledControlData("Lightning", 100, 100, 255);
            await controlData.TurnOnKitchenWledLight();
        }

        internal async void WindWarningLights()
        {
            var controlData = new WledControlData("Stream 2", 100, 100, 255);
            await controlData.TurnOnKitchenWledLight();
        }

        internal async void WinterWarningLights()
        {
            var controlData = new WledControlData("Flow", 100, 50, 255, 35);
            await controlData.TurnOnKitchenWledLight();
        }

        internal async void RainKitchenLights()
        {
            var controlData = new WledControlData("Rain", 100, 100, 255);
            await controlData.TurnOnKitchenWledLight();
        }
    }
}

