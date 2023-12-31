﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components;

internal class Camera3 : Component
{
    public Camera3D CameraPosition;
    public CameraMode CameraMode;
    public double FieldOfView;
    public Camera3(Entity owner) : base(owner)
    {
        CameraPosition = new Camera3D();
        CameraMode = CameraMode.CAMERA_FREE;
    }

    public Camera3()
    {
        CameraMode = CameraMode.CAMERA_FREE;
        CameraPosition = new Camera3D();
        FieldOfView = 0;
    }
}
