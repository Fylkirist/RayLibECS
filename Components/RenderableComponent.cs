using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayLibECS.Components;

public class RenderableComponent:Component
{
    public RenderingModes RenderingMode;
    public int Z;

    public RenderableComponent()
    {
        RenderingMode = RenderingModes.TwoD;
        Z = 0;
    }
}