using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public readonly struct VfxPlayArgs
    {
        public readonly Vector3 position;
        public readonly Quaternion rotation;
        public readonly Transform parent;
        public readonly float scale;
        public readonly Color? tint;

        public VfxPlayArgs(Vector3 position, Quaternion rotation, Transform parent = null, float scale = 1f, Color? tint = null)
        {
            this.position = position;
            this.rotation = rotation;
            this.parent = parent;

            this.scale = scale;
            this.tint = tint;
        }
    }
}