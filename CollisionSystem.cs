/**
 * 
 * Author: Jake.E
 * Purpose: Abstract class for collision testing algorithms.  
 *          Maintains a list of colliders and bodies
 * 
 **/

using UnityEngine;
using System.Collections.Generic;

namespace Peril.Physics
{
    public abstract class CollisionSystem
    {

        ///// Fields /////

        protected List<ICollisionBody> bodyList = new List<ICollisionBody>(MaxCollisionBodies);
        private HashSet<int> _pairs = new HashSet<int>();
        private List<int> _pairCache = new List<int>();
        private int _uniqueIndex;

        public const int MaxCollisionBodies = 10000;

        ///// Methods /////

        public abstract void DetectBodyVsBody();
        public abstract bool LineOfSight(Vector3 start, Vector3 end);

        /// <summary>
        /// Adds a body to the CollisionSystem
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual bool AddBody(ICollisionBody body)
        {
            if (!bodyList.Contains(body) && bodyList.Count < MaxCollisionBodies)
            {
                body.RefId = _uniqueIndex;
                _uniqueIndex++;
                bodyList.Add(body);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a body from the CollisionSystem
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual bool RemoveBody(ICollisionBody body)
        {
            return bodyList.Remove(body);
        }

        /// <summary>
        /// Process CollisionSystem by one step
        /// </summary>
        public virtual void Step()
        {
            DetectBodyVsBody();


            // This was implemented for CollisionSystem implementations with broad phases
            // When two colliders are paired and one of them is moved to a far away position 
            // on the same frame, they wont be tested next frame due to broad phasing, but they will still be paired.
            // This simply checks all pairs that weren't checked this frame

            foreach(var i in _pairs)
            {
                var body1 = FindCollisionBody(i / (MaxCollisionBodies + 1));
                var body2 = FindCollisionBody(i % (MaxCollisionBodies + 1));
                if (body1 == null || body2 == null)
                {
                    continue;
                }
                Test(body1, body2, false);
            }

            _pairs.Clear();

            for(int i = 0; i < _pairCache.Count; i++)
            {
                _pairs.Add(_pairCache[i]);
            }

            _pairCache.Clear();
        }

        public ICollisionBody FindCollisionBody(int refId)
        {
            for(int i = 0; i < bodyList.Count; i++)
            {
                if (bodyList[i].RefId == refId)
                    return bodyList[i];
            }
            return null;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.black;
            for(var i = 0; i < bodyList.Count; i++)
            {
                var center = bodyList[i].CollisionShape.Center;
                if (center == Vector3.zero) continue;
                center.y += 2f;
                Gizmos.DrawWireCube(center, bodyList[i].CollisionShape.Extents * 2);
            }
        }

        /// <summary>
        ///  Executes collision between two bodies
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <param name="removePair"></param>
        /// <returns></returns>
        protected bool Test(ICollisionBody body1, ICollisionBody body2, bool removePair = true)
        {
            var result = new CollisionResult();
            var paired = FindCollisionPair(body1, body2, removePair);

            if (TestCollisionShapes(body1.CollisionShape, body2.CollisionShape, ref result))
            {
                result.Type = paired ? CollisionType.Stay : CollisionType.Enter;
                CacheCollisionPair(body1, body2);
                body2.OnCollision(result, body1);
                result.Normal *= -1;
                result.First = true;
                body1.OnCollision(result, body2);
                return true;
            }
            else
            {
                if(paired)
                {
                    result.Type = CollisionType.Exit;
                    body2.OnCollision(result, body1);
                    result.Normal *= -1;
                    result.First = true;
                    body1.OnCollision(result, body2);
                    return true;
                }
            }
            return false;
        }

        private bool FindCollisionPair(ICollisionBody a, ICollisionBody b, bool remove = true)
        {
            var idx = a.RefId * (MaxCollisionBodies + 1) + b.RefId;
            if (remove) return _pairs.Remove(idx);
            else return _pairs.Contains(idx);
        }

        private void CacheCollisionPair(ICollisionBody a, ICollisionBody b)
        {
            var idx = a.RefId * (MaxCollisionBodies + 1) + b.RefId;
            _pairCache.Add(idx);
        }

        private static bool TestCollisionShapes(ICollisionShape a, ICollisionShape b, ref CollisionResult result)
        {
            result = a.TestCollision(b);
            return result.Collides;
        }

    }
}

/** 
 * 
 * Random reading material
 * https://www.lhup.edu/~dsimanek/ideas/bounce.htm
 * http://www.red3d.com/cwr/steer/Doorway.html
 * http://gamedev.stackexchange.com/questions/18436/most-efficient-aabb-vs-ray-collision-algorithms/18459#18459
 * http://gamedev.stackexchange.com/questions/47888/find-the-contact-normal-of-rectangle-collision
 * 
**/
