using ChandlerHome.Automations;

namespace ChandlerHome.Automations.FrontOfHouse.Switches;

[NetDaemonApp(Id = "Front of House Z-Wave Switch")]
internal class HolidayDecorationSwitch : Home
{
    public HolidayDecorationSwitch(IHaContext ha, IScheduler scheduler) : base(ha)
    {
        _entities ??= new Entities(ha);

        scheduler.ScheduleCron("0 15 * * MON,TUE,WED,THU,FRI", () => _entities.Switch.Plug.TurnOn());

        scheduler.ScheduleCron("0 22 * * MON,TUE,WED,THU,FRI", () => _entities.Switch.Plug.TurnOff());

        scheduler.ScheduleCron("0 7 * * MON,TUE,WED,THU,FRI", () => _entities.Switch.Plug.TurnOn());

        scheduler.ScheduleCron("0 9 * * MON,TUE,WED,THU,FRI", () => _entities.Switch.Plug.TurnOff());

        scheduler.ScheduleCron("0 8 * * SAT,SUN", () => _entities.Switch.Plug.TurnOn()); //turn on 8AM SAT/SUN

        scheduler.ScheduleCron("0 22 * * SAT,SUN", () => _entities.Switch.Plug.TurnOff());
    }
}
