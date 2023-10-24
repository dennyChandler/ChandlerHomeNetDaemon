using System.Threading;

namespace ChandlerHome.Automations.Livingroom.Lighting;

[NetDaemonApp(Id = "Candle Experiment")]
internal class Candle : Livingroom
{
    private const double TwoPi = Math.PI * 2;
    public Candle(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);
        var lamp = _entities.Light.LivingRoomLamp;
        var rand = new Random();

        _entities.InputBoolean.CandlesOn.StateChanges().Where(e => e.New.IsOn())
        .Subscribe(x =>
        {
            while (_entities.InputBoolean.CandlesOn.IsOn())
            {
                var brightness = lamp.Attributes.Brightness ?? 255;
                var brightnessMin = Math.Max(brightness - 10, 0);
                var brightnessMax = Math.Min(brightness + 10, 255);

                var delayMin = 250;  // Default minimum delay (0.25 seconds)
                var delayMax = 750;  // Default maximum delay (0.75 seconds)
                var kelvinMin = 2000;  // Minimum color temperature (warm)
                var kelvinMax = 4000;  // Maximum color temperature (cool)

                var elapsedTime = DateTime.Now.Second + DateTime.Now.Millisecond / 1000.0;
                var brightnessVariation = Math.Sin(elapsedTime * TwoPi / 5.0) * 8.0; // Adjust the factor for the desired flicker effect

                var kelvin = rand.Next(kelvinMin, kelvinMax + 1);
                brightness = (int)(brightness + brightnessVariation);
                brightness = Math.Max(brightnessMin, Math.Min(brightnessMax, brightness));

                var delay = TimeSpan.FromMilliseconds(rand.Next(delayMin, delayMax + 1));

                lamp.TurnOn(brightness: (long)brightness, transition: (long)delay.TotalSeconds, kelvin: kelvin);

                Thread.Sleep(delay);
            }
        });
    }
}
