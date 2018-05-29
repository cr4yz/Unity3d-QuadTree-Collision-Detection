/**
 * 
 * Author: Jake.E
 * Purpose: Implements a brute-force algorithm for testing collisions
 * 
 **/

using UnityEngine;

namespace Peril.Physics
{
    /// <summary>
    /// Brute force collision detection algorithm.
    /// </summary>
    public class CollisionSystemBrute : CollisionSystem
    {

        ///// Methods /////

        public override void DetectBodyVsBody()
        {
            int count = bodyList.Count;
            for (var i = 0; i < count; i++)
            {
                if (bodyList[i].Sleeping)
                    continue;

                for (var j = i + 1; j < count; j++)
                {
                    if (bodyList[j].Sleeping)
                        continue;

                    Test(bodyList[i], bodyList[j]);
                }
            }
        }

        public override bool LineOfSight(Vector3 start, Vector3 end)
        {
            for (var i = 0; i < bodyList.Count; i++)
            {
                if (CollisionTest.SegmentIntersects(bodyList[i].CollisionShape, start, end))
                    return false;
            }
            return true;
        }

    }
}

