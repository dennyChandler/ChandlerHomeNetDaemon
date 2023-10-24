namespace ChandlerHome.Automations.Office.Lighting;

[NetDaemonApp(Id = "Dennys Office Fan Lights")]
public class FanLights : Office
{
    public FanLights(IHaContext ha) : base(ha)
    {
        if (_entities == null)
            _entities = new Entities(ha);

        var officeMotion = _entities.BinarySensor.DennyOfficeMotion;
        var fanLights = _entities.Light.DennysOfficeLights;
        var gamingPcSessionState = _entities.Sensor.RocinanteSessionstate;
        var plexState = _entities.MediaPlayer.PlexPlexForWindowsRocinante;

        var plexSessionStates = new List<string> { "playing", "running" };

        officeMotion.StateChanges().Where(e => e.New?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false && fanLights.IsOff())
            .Subscribe(x =>
            {
                if (plexSessionStates.Contains(_entities.MediaPlayer.PlexPlexForWindowsRocinante.State?.ToString() ?? ""))
                    TurnOn(fanLights, 25, 3);
                else if (_entities.Sensor.TempAndHumidityAirTemperature.State > 83)
                    TurnOn(fanLights, 30, 3);
                else
                    TurnOn(fanLights, 100, 3);
            });

        officeMotion.StateChanges().Where(e => e.New?.State?.Equals("off", StringComparison.OrdinalIgnoreCase) ?? false && fanLights.IsOn())
            .Subscribe(x =>
            {
                if (!gamingPcSessionState.State?.Equals("InUse", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    TurnOff(fanLights, 3);
                }
            });

        gamingPcSessionState.StateChanges().Where(e => e.New?.State?.Equals("InUse", StringComparison.OrdinalIgnoreCase) ?? false && fanLights.IsOff())
            .Subscribe(x =>
            {
                if (_entities.Sensor.TempAndHumidityAirTemperature.State > 83)
                    TurnOn(fanLights, 30, 3);
                else
                    TurnOn(fanLights, 100, 3);
            });

        plexState.StateChanges().Where(e => plexSessionStates.Contains(e.New?.State?.ToString() ?? ""))
            .Subscribe(x =>
            {
                TurnOn(fanLights, 25, 3);
            });

        plexState.StateChanges().Where(e => !plexSessionStates.Contains(e.New?.State?.ToString() ?? ""))
            .Subscribe(x =>
            {
                if (_entities.Sensor.TempAndHumidityAirTemperature.State > 83)
                    TurnOn(fanLights, 30, 3);
                else
                    TurnOn(fanLights, 100, 3);
            });

        _entities.InputBoolean.MuteDennysPc.StateChanges().Where(e => e.New?.State?.Equals("on", StringComparison.OrdinalIgnoreCase) ?? false)
            .Subscribe(x =>
            {
                while (_entities.Sensor.RocinanteVolume.State > 0)
                {
                    _entities.Switch.RocinanteStepVolumeDown.TurnOn();
                    System.Threading.Thread.Sleep(5000);
                }
            });



    }
}
