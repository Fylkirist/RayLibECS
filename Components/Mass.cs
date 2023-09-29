using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayLibECS.Components
{
    public class Mass : Component
    {
        public double Kgs;

        public Mass()
        {
            Kgs = 0;
        }
    }
}
