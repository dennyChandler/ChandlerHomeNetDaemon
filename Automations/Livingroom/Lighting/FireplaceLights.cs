using ChandlerHome.Automations.Livingroom.Lighting.FireplaceRoutines;

namespace ChandlerHome.Automations.Livingroom.Lighting;

[NetDaemonApp(Id = "Living Room Lights")]
public class FireplaceLights : Livingroom
{
    public FireplaceLights(IHaContext ha) : base(ha)
    {
        _entities ??= new Entities(ha);

        _entities.InputBoolean.HolidayLights.StateChanges().Where(e => e.New.IsOn()).
            Subscribe(x =>
            {
                var holidayLightPatterns = new FireplaceLightPatterns(ha);
                holidayLightPatterns.SetFireplaceHolidaLights();
            });
    }
}
