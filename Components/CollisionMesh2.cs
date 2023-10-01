using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Interfaces;

namespace RayLibECS.Components;

internal class CollisionMesh2 : Component
{
    public List<IVertex> Vertices;
    public CollisionMesh2()
    {
        Vertices = new List<IVertex>();
    }
}