using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChandlerHome.apps.HassModel.BackOfHouse
{
    internal class BackOfHouse : Home
    {
        public BackOfHouse(IHaContext ha) : base(ha)
        {
            _entities ??= new Entities(ha);


        }
    }
}
