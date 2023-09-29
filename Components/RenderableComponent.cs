using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayLibECS.Components;

internal class RenderableComponent:Component
{
    public RenderingModes RenderingMode;

    public RenderableComponent() : base()
    {
        RenderingMode = RenderingModes.TwoD;
    }
}