using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.Livingroom.Lighting.FireplaceRoutines
{
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
                case 7:
                    SetFireplaceLightsToUSA(_entities);
                    break;
                case 9:
                case 10:
                    SetFireplaceLightsToHalloweenColors(_entities);
                    break;
                case 12:
                    SetFireplaceLightsToChristmasColors(_entities);
                    break;
                default:
                    break;
            }
        }

                private void SetFireplaceLightsToUSA(Entities entities)
        {
            TurnOn(entities.Light.Fireplace1, 100, 3, "red");
            TurnOn(entities.Light.InnerFireplaceLights, 100, 3, "white");
            TurnOn(entities.Light.Fireplace4, 100, 3, "blue");
        }

        private void SetFireplaceLightsToHalloweenColors(Entities entities)
        {
            TurnOn(entities.Light.OuterFireplaceLights, 100, 3, "purple");
            TurnOn(entities.Light.InnerFireplaceLights, 100, 3, "orange");
        }

        private void SetFireplaceLightsToChristmasColors(Entities entities)
        {
            TurnOn(entities.Light.OuterFireplaceLights, 100, 3, "red");
            TurnOn(entities.Light.InnerFireplaceLights, 100, 3, "green");
        }
    }
}
