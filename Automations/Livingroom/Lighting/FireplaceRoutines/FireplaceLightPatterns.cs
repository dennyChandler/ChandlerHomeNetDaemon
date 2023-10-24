namespace ChandlerHome.Automations.Livingroom.Lighting.FireplaceRoutines;

internal class FireplaceLightPatterns : Livingroom
{
    public FireplaceLightPatterns(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);
    }

    public void SetFireplaceHolidaLights()
    {
        switch (DateTime.Now.Month)
        {
            case 4:
                SetFireplaceLightsToStPatricksColors(_entities);
                SetUpstairsLightsToStPatricksColors(_entities);
                break;
            case 7:
                SetFireplaceLightsToUSA(_entities);
                SetUpstairsLightsToUSA(_entities);
                break;
            case 9:
            case 10:
                SetFireplaceLightsToHalloweenColors(_entities);
                SetUpstairsLightsToHalloween(_entities);
                break;
            case 11:
            case 12:
                SetFireplaceLightsToChristmasColors(_entities);
                SetUpstairsLightsToChristmas(_entities);
                break;
            default:
                SetFireplaceLightsToRainbow(_entities);
                SetUpstairsLightsToRainbow(_entities);
                break;
        }
    }

    private void SetUpstairsLightsToStPatricksColors(Entities? entities)
    {
        if (entities != null)
            TurnOn(entities.Light.ZigbeeStickGroupsUpstairsColorLights, 100, 0, "green");
    }

    private void SetFireplaceLightsToStPatricksColors(Entities? entities)
    {
        if (entities != null)
            TurnOn(entities.Light.FireplaceLights, 100, 0, "green");
    }

    private void SetUpstairsLightsToUSA(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.UpstairsHallNorthLight, 100, 0, "red");
            TurnOn(entities.Light.TopOfStairsLightLight, 100, 0, "blue");
            TurnOn(entities.Light.UpstairsSouthHallLight, 100, 0, "white");
            TurnOn(entities.Light.MainFloorHallNorthLight, 100, 0, "red");
            TurnOn(entities.Light.MainFloorHallSouthLight, 100, 0, "blue");
        }
    }
    private void SetUpstairsLightsToHalloween(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.UpstairsHallNorthLight, 100, 0, "purple");
            TurnOn(entities.Light.TopOfStairsLightLight, 100, 0, "orange");
            TurnOn(entities.Light.UpstairsSouthHallLight, 100, 0, "green");
            TurnOn(entities.Light.MainFloorHallNorthLight, 100, 0, "purple");
            TurnOn(entities.Light.MainFloorHallSouthLight, 100, 0, "orange");
        }
    }
    private void SetUpstairsLightsToChristmas(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.UpstairsHallNorthLight, 100, 0, "red");
            TurnOn(entities.Light.TopOfStairsLightLight, 100, 0, "green");
            TurnOn(entities.Light.UpstairsSouthHallLight, 100, 0, "red");
            TurnOn(entities.Light.MainFloorHallNorthLight, 100, 0, "red");
            TurnOn(entities.Light.MainFloorHallSouthLight, 100, 0, "green");
        }
    }
    private void SetUpstairsLightsToRainbow(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.ZigbeeStickGroupsUpstairsColorLights, 100, 0, effect: "LSD");
            TurnOn(entities.Light.ZigbeeStickGroupsMainFloorHall, 100, 0, effect: "LSD");
        }
    }

    public void SetLivingRoomLightsToRain()
    {
        if (DateTime.Now.Hour > 5 && DateTime.Now.Hour < 24)
        {
            SetFireplaceLightsToRainColors(_entities);
            SetHallLightsToRain(_entities);
        }
    }

    private void SetHallLightsToRain(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.ZigbeeStickGroupsMainFloorHall, 100, 3, "dodgerblue");
        }
    }

    private void SetFireplaceLightsToRainbow(Entities? entities)
    {
        if (entities != null)
            TurnOn(entities.Light.FireplaceLights, 100, 0, effect: "LSD");
    }

    private void SetFireplaceLightsToUSA(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.Fireplace1, 100, 3, "red");
            TurnOn(entities.Light.InnerFireplaceLights, 100, 3, "white");
            TurnOn(entities.Light.Fireplace4, 100, 3, "blue");
        }
    }

    private void SetFireplaceLightsToHalloweenColors(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.OuterFireplaceLights, 100, 3, "purple");
            TurnOn(entities.Light.InnerFireplaceLights, 100, 3, "orange");
        }
    }

    private void SetFireplaceLightsToChristmasColors(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.OuterFireplaceLights, 100, 3, "red");
            TurnOn(entities.Light.InnerFireplaceLights, 100, 3, "green");
        }
    }

    private void SetFireplaceLightsToRainColors(Entities? entities)
    {
        if (entities != null)
        {
            TurnOn(entities.Light.OuterFireplaceLights, 100, 3, "deepskyblue");
            TurnOn(entities.Light.InnerFireplaceLights, 100, 3, "darkblue");
        }
    }
}
