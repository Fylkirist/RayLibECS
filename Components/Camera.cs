using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using RayLibECS.Entities;

namespace RayLibECS.Components
{
    internal class Camera3:Component
    {
        public Camera3D CameraPosition;
        public CameraMode CameraMode;
        public Camera3(Entity owner) : base(owner)
        {
            CameraPosition = new Camera3D();
            CameraMode = CameraMode.CAMERA_FREE;
        }
    }
}
