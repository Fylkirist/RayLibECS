﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace RayLibECS.Components;

internal struct ColouredMesh2
{
    public GeometryMesh2 Mesh;
    public List<Color> Colours;
    public RenderingModes RenderingMode;
    public int Z;

    public ColouredMesh2()
    {
        Z = 0;
        RenderingMode = RenderingModes.TwoD;
        Mesh = new GeometryMesh2();
        Colours = new List<Color>();
    }
}