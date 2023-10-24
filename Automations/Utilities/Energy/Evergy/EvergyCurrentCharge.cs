using ChandlerHome.Helpers.EvergyPriceTracker;

namespace ChandlerHome.Automations.Utilities.Energy.Evergy;

[NetDaemonApp(Id = "Evergy Current Price Tracker")]
internal class EvergyCurrentCharge
{
    public EvergyCurrentCharge(IHaContext context)
    {
        int result;
        var entities = new Entities(context);

        Observable.Interval(TimeSpan.FromMinutes(10))
        .Subscribe(_ =>
            {
                var currentElectricVal = entities.Sensor.EvergyMonthlyEnergyUsed.State;
                if (currentElectricVal == null)
                {
                    currentElectricVal = 100;
                }
                var energyPrice = EvergyPriceDictionary.GetCurrentRate((int)currentElectricVal);
                entities.InputNumber.CurrentEnergyPrice.SetValue(new InputNumberSetValueParameters { Value = (double)energyPrice });
            });
    }
}
