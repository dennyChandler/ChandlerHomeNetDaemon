using ChandlerHome.apps.HassModel.Kitchen;
using ChandlerHome.Helpers.WLEDControls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.Helpers.WLEDControls.LightRoutines
{
    public class LightPatterns : Kitchen
    {
        private const string WledApiUrl = "http://192.168.1.126/api/v1";
        public LightPatterns(IHaContext ha) : base(ha)
        {
            _entities ??= new Entities(ha);
        }

        internal async void TornadoWarningLights()
        {
            var controlData = new WledControlData();
            await controlData.TurnOnKitchenWledLight("Scan Dual", 255);
        }
    }
}

