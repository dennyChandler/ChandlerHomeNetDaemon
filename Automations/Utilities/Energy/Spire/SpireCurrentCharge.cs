using ChandlerHome.Helpers.SpirePriceTracker;

namespace ChandlerHome.Automations.Utilities.Energy.Spire;

[NetDaemonApp(Id = "Spire Current Price Tracker")]
internal class SpireCurrentCharge
{
    public SpireCurrentCharge(IHaContext ha)
    {
        double gasUsage;
        var entities = new Entities(ha);

        Observable.Interval(TimeSpan.FromMinutes(15))
            .Subscribe(_ =>
                {
                    var currentGasUsage = entities.Sensor.MonthlyGasUsed.State;
                    if (currentGasUsage == null)
                    {
                        gasUsage = 5;
                    }
                    else
                        gasUsage = currentGasUsage.HasValue ? currentGasUsage.Value : 5;

                    var usage = (int)gasUsage;
                    var energyPrice = SpirePriceTracker.GetCurrentPricePerCcf(usage);
                    entities.InputNumber.CurrentGasPrice.SetValue(new InputNumberSetValueParameters { Value = (double)energyPrice });
                });
    }
}
