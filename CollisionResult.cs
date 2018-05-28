using UnityEngine;

namespace Peril.Physics
{
    public struct CollisionResult
    {
        public bool Collides;
        public Vector3 Normal;
        public float Penetration;
        public bool IsGeometry;
        public CollisionType Type;
        public bool First;
    }
}

