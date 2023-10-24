using System.Threading;
using ChandlerHome.Automations;

namespace ChandlerHome.Automations.FrontOfHouse.Lighting;

[NetDaemonApp(Id = "Entryway Chandelier")]
internal class EntrywayChandelier : Home
{
    private bool offTimerIsRunning;
    public EntrywayChandelier(IHaContext ha, IScheduler scheduler) : base(ha)
    {
        _entities ??= new Entities(ha);
        TimeSpan EntrywayOffTimer = TimeSpan.FromMinutes(15);


        _entities.BinarySensor.FrontDoor.StateChanges().Where(x => x.New.IsOn())
            .Subscribe(y =>
            {
                _entities.Light.EntrywayChandelier.TurnOn();
            });

        _entities.BinarySensor.FrontDoor.StateChanges().Where(x => x.New.IsOff())
            .Subscribe(y =>
            {
                StartOffTimer(ha);
            });

        Observable.Interval(TimeSpan.FromMinutes(30))
        .Subscribe(_ =>
        {
            if (_entities.Light.EntrywayChandelier.IsOn())
                if (_entities.BinarySensor.FrontDoor.IsOff())
                    if (!offTimerIsRunning)
                        StartOffTimer(ha);
        });
    }

    private void StartOffTimer(IHaContext ha)
    {
        offTimerIsRunning = true;
        Thread.Sleep(900000);

        _entities ??= new Entities(ha);
        _entities.Light.EntrywayChandelier.TurnOff();
    }


}
