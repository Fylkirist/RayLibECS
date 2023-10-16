using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace RayLibECS.Components;

internal class ColouredMesh2:RenderableComponent
{
    public GeometryMesh2 Mesh;
    public List<Color> Colours;

    public ColouredMesh2()
    {
        Mesh = new GeometryMesh2();
        Colours = new List<Color>();
    }
}