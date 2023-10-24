using ChandlerHome.apps.HassModel.Livingroom.Lighting.FireplaceRoutines;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Livingroom.Lighting
{
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
}
