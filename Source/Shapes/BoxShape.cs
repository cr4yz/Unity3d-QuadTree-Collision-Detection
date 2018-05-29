using UnityEngine;

namespace Peril.Physics
{
    public class BoxShape : ICollisionShape
    {

        ///// Constructor /////

        public BoxShape(Bounds bounds, bool twoD = true)
        {
            Bounds = bounds;
            TwoD = twoD;
        }

        ///// Fields /////

        public Bounds Bounds;
        public bool TwoD;

        ///// Properties /////

        public Vector3 Center
        {
            get { return Bounds.center; }
            set { Bounds.center = value; }
        }

        public Vector3 Extents
        {
            get { return Bounds.extents; }
            set { Bounds.extents = value; }
        }

        public Vector3 Min { get { return Center - Extents; } }
        public Vector3 Max { get { return Center + Extents; } }

        ///// Methods /////

        public CollisionResult TestCollision(ICollisionShape other)
        {
            var result = new CollisionResult();

            if(other is BoxShape)
            {
                result.Collides = BoxVsBox(this, (BoxShape)other, ref result, TwoD);
            }
            else
            {
                Debug.LogErrorFormat("Collision test not implemented: {0}-{1}", GetType(), other.GetType());
            }
            
            return result;
        }

        public static bool BoxVsBox(BoxShape a, BoxShape b, ref CollisionResult result, bool twoD)
        {
            return CollisionTest.TestAABB(a.Min, a.Max, b.Min, b.Max, ref result, twoD);
        }

    }
}
