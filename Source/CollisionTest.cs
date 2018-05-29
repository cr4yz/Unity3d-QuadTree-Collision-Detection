/**
 * 
 * Author: Jake.E
 * Purpose: Provides static methods for testing collisions and generating results.
 * 
 **/


using UnityEngine;

namespace Peril.Physics
{
    public sealed class CollisionTest
    {

        ///// Fields /////

        private static Vector3[] _aabbNormals =
        {
            // x
            new Vector3(-1, 0, 0),
            new Vector3(1, 0, 0),
            // z
            new Vector3(0, 0, -1),
            new Vector3(0, 0, 1),
            // y
            new Vector3(0, -1, 0),
            new Vector3(0, 1, 0),
        };

        private static float[] _distances = new float[6];

        ///// Public methods /////

        public static bool TestAABB(Vector3 min0, Vector3 max0, Vector3 min1, Vector3 max1, ref CollisionResult result, bool twod = true)
        {
            _distances[0] = max1[0] - min0[0];
            _distances[1] = max0[0] - min1[0];
            _distances[2] = max1[2] - min0[2];
            _distances[3] = max0[2] - min1[2];

            var iter = 4;

            // test y if 3d
            if(!twod)
            {
                _distances[4] = max1[1] - min0[1];
                _distances[5] = max0[1] - min1[1];
                iter = 6;
            }

            for (int i = 0; i < iter; i++)
            {
                if (_distances[i] < 0.0f)
                    return false;

                if ((i == 0) || _distances[i] < result.Penetration)
                {
                    result.Penetration = _distances[i];
                    result.Normal = _aabbNormals[i];
                }
            }

            return true;
        }

        /// <summary>
        /// Test if a finite line segment intersects a collider
        /// </summary>
        /// <param name="collider">Collider to test</param>
        /// <param name="start">Start point of line</param>
        /// <param name="end">End point of line</param>
        /// <returns>True if line intersects collider</returns>
        public static bool SegmentIntersects(ICollisionShape collider, Vector3 start, Vector3 end)
        {
            Vector3 d = (end - start) * 0.5f;
            Vector3 e = (collider.Max - collider.Min) * 0.5f;
            Vector3 c = start + d - (collider.Min + collider.Max) * 0.5f;

            Vector3 ad = new Vector3(Mathf.Abs(d.x), Mathf.Abs(d.y), Mathf.Abs(d.z));

            if (Mathf.Abs(c.x) > e.x + ad.x)
                return false;
            if (Mathf.Abs(c.y) > e.y + ad.y)
                return false;
            if (Mathf.Abs(c.z) > e.z + ad.z)
                return false;

            if (Mathf.Abs(d.y * c.z - d.z * c.y) > e.y * ad.z + e.z * ad.y + Mathf.Epsilon)
                return false;
            if (Mathf.Abs(d.z * c.x - d.x * c.z) > e.z * ad.x + e.x * ad.z + Mathf.Epsilon)
                return false;
            if (Mathf.Abs(d.x * c.y - d.y * c.x) > e.x * ad.y + e.y * ad.x + Mathf.Epsilon)
                return false;

            return true;
        }

    }
}

