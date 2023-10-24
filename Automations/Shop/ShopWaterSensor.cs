namespace ChandlerHome.Automations.Shop;

[NetDaemonApp(Id = "Shop Water Sensor")]
internal class ShopWaterSensor
{
    public ShopWaterSensor(IHaContext ha)
    {
        var entities = new Entities(ha);
        var services = new Services(ha);

        entities.BinarySensor.ShopWaterSensorMoisture.StateChanges().Where(e => e.New.IsOn())
            .Subscribe(x =>
            {
                NotifyOfWaterInShop(entities, services);
            });

        if (entities.BinarySensor.ShopWaterSensorMoisture.IsOn())
        {
            NotifyOfWaterInShop(entities, services);
        }
    }

    private void NotifyOfWaterInShop(Entities entities, Services services)
    {
        services.Notify.MobileAppDennysPhone(new NotifyMobileAppDennysPhoneParameters
        {
            Title = "WATER IN THE BASEMENT!",
            Message = "There is water in the basement, check it ASAP!"
        });

        services.Notify.MobileAppBrittanysPhone(new NotifyMobileAppBrittanysPhoneParameters
        {
            Title = "WATER IN THE BASEMENT!",
            Message = "There is water in the basement, check it ASAP!"
        });

        entities.Light.BedroomLights.TurnOn(transition: 0, colorName: "red", effect: "flash");
    }
}
