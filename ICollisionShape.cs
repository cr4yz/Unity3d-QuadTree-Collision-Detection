using UnityEngine;

namespace Peril.Physics
{
    public interface ICollisionShape
    {
        Vector3 Center { get; set; }
        Vector3 Extents { get; }
        Vector3 Min { get; }
        Vector3 Max { get; }
        CollisionResult TestCollision(ICollisionShape other);
    }
}
