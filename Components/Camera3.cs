using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal struct Camera3
{
    public Camera3D CameraPosition;
    public CameraMode CameraMode;
    public int Owner;
    public Camera3(int owner)
    {
        Owner = owner;
        CameraPosition = new Camera3D();
        CameraMode = CameraMode.CAMERA_FREE;
    }

    public Camera3()
    {
        CameraMode = CameraMode.CAMERA_FREE;
        CameraPosition = new Camera3D();
    }
}