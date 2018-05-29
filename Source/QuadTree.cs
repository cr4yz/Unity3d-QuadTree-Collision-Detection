using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peril.Physics
{
    public class QuadTree
    {

        private static class QuadTreePool
        {

            ///// Fields /////

            private static Queue<QuadTree> _pool;
            private static int _maxPoolCount = 1024;
            private static int _defaultMaxBodiesPerNode = 6;
            private static int _defaultMaxLevel = 6;

            ///// Methods /////

            public static QuadTree GetQuadTree(Rect bounds, QuadTree parent)
            {
                if (_pool == null) Init();
                QuadTree tree = null;
                if (_pool.Count > 0)
                {
                    tree = _pool.Dequeue();
                    tree._bounds = bounds;
                    tree._parent = parent;
                    tree._maxLevel = parent._maxLevel;
                    tree._maxBodiesPerNode = parent._maxBodiesPerNode;
                    tree._curLevel = parent._curLevel + 1;
                }
                else tree = new QuadTree(bounds, parent);
                return tree;
            }

            public static void PoolQuadTree(QuadTree tree)
            {
                if (tree == null) return;
                tree.Clear();
                if (_pool.Count > _maxPoolCount) return;
                _pool.Enqueue(tree);
            }

            private static void Init()
            {
                _pool = new Queue<QuadTree>();
                for (int i = 0; i < _maxPoolCount; i++)
                {
                    _pool.Enqueue(new QuadTree(Rect.zero, _defaultMaxBodiesPerNode, _defaultMaxLevel));
                }
            }
        }

        ///// Constructors /////

        public QuadTree(Rect bounds, int maxBodiesPerNode = 6, int maxLevel = 6)
        {
            _bounds = bounds;
            _maxBodiesPerNode = maxBodiesPerNode;
            _maxLevel = maxLevel;
            _bodies = new List<IQuadTreeBody>(maxBodiesPerNode);
        }

        private QuadTree(Rect bounds, QuadTree parent)
            : this(bounds, parent._maxBodiesPerNode, parent._maxLevel)
        {
            _parent = parent;
            _curLevel = parent._curLevel + 1;
        }

        ///// Fields /////

        private QuadTree _parent;
        private Rect _bounds;
        private List<IQuadTreeBody> _bodies;
        private int _maxBodiesPerNode;
        private int _maxLevel;
        private int _curLevel;
        private QuadTree _childA;
        private QuadTree _childB;
        private QuadTree _childC;
        private QuadTree _childD;
        private List<IQuadTreeBody> _entCache;

        ///// Methods /////

        public void AddBody(IQuadTreeBody body)
        {
            if (_childA != null)
            {
                var child = GetQuadrant(body.Position);
                child.AddBody(body);
            }
            else
            {
                _bodies.Add(body);
                if (_bodies.Count > _maxBodiesPerNode && _curLevel < _maxLevel)
                {
                    Split();
                }
            }
        }

        public List<IQuadTreeBody> GetBodies(Vector3 point, float radius)
        {
            var p = new Vector2(point.x, point.z);
            return GetBodies(p, radius);
        }

        public List<IQuadTreeBody> GetBodies(Vector2 point, float radius)
        {
            if (_entCache == null) _entCache = new List<IQuadTreeBody>(64);
            else _entCache.Clear();
            GetBodies(point, radius, _entCache);
            return _entCache;
        }

        public List<IQuadTreeBody> GetBodies(Rect rect)
        {
            if (_entCache == null) _entCache = new List<IQuadTreeBody>(64);
            else _entCache.Clear();
            GetBodies(rect, _entCache);
            return _entCache;
        }

        private void GetBodies(Vector2 point, float radius, List<IQuadTreeBody> bods)
        {
            //no children
            if (_childA == null)
            {
                for (int i = 0; i < _bodies.Count; i++)
                    bods.Add(_bodies[i]);
            }
            else
            {
                if (_childA.ContainsCircle(point, radius))
                    _childA.GetBodies(point, radius, bods);
                if (_childB.ContainsCircle(point, radius))
                    _childB.GetBodies(point, radius, bods);
                if (_childC.ContainsCircle(point, radius))
                    _childC.GetBodies(point, radius, bods);
                if (_childD.ContainsCircle(point, radius))
                    _childD.GetBodies(point, radius, bods);
            }
        }

        private void GetBodies(Rect rect, List<IQuadTreeBody> bods)
        {
            //no children
            if (_childA == null)
            {
                for (int i = 0; i < _bodies.Count; i++)
                    bods.Add(_bodies[i]);
            }
            else
            {
                if (_childA.ContainsRect(rect))
                    _childA.GetBodies(rect, bods);
                if (_childB.ContainsRect(rect))
                    _childB.GetBodies(rect, bods);
                if (_childC.ContainsRect(rect))
                    _childC.GetBodies(rect, bods);
                if (_childD.ContainsRect(rect))
                    _childD.GetBodies(rect, bods);
            }
        }

        public bool ContainsCircle(Vector2 circleCenter, float radius)
        {
            var center = _bounds.center;
            var dx = Math.Abs(circleCenter.x - center.x);
            var dy = Math.Abs(circleCenter.y - center.y);
            if (dx > (_bounds.width / 2 + radius)) { return false; }
            if (dy > (_bounds.height / 2 + radius)) { return false; }
            if (dx <= (_bounds.width / 2)) { return true; }
            if (dy <= (_bounds.height / 2)) { return true; }
            var cornerDist = Math.Pow((dx - _bounds.width / 2), 2) + Math.Pow((dy - _bounds.height / 2), 2);
            return cornerDist <= (radius * radius);
        }

        public bool ContainsRect(Rect rect)
        {
            return _bounds.Overlaps(rect);
        }

        private QuadTree GetLowestChild(Vector2 point)
        {
            var ret = this;
            while (ret != null)
            {
                var newChild = ret.GetQuadrant(point);
                if (newChild != null) ret = newChild;
                else break;
            }
            return ret;
        }

        public void Clear()
        {
            QuadTreePool.PoolQuadTree(_childA);
            QuadTreePool.PoolQuadTree(_childB);
            QuadTreePool.PoolQuadTree(_childC);
            QuadTreePool.PoolQuadTree(_childD);
            _childA = null;
            _childB = null;
            _childC = null;
            _childD = null;
            _bodies.Clear();
        }

        public void DrawGizmos()
        {
            //draw children
            if (_childA != null) _childA.DrawGizmos();
            if (_childB != null) _childB.DrawGizmos();
            if (_childC != null) _childC.DrawGizmos();
            if (_childD != null) _childD.DrawGizmos();

            //draw rect
            Gizmos.color = Color.cyan;
            var p1 = new Vector3(_bounds.position.x, 0.1f, _bounds.position.y);
            var p2 = new Vector3(p1.x + _bounds.width, 0.1f, p1.z);
            var p3 = new Vector3(p1.x + _bounds.width, 0.1f, p1.z + _bounds.height);
            var p4 = new Vector3(p1.x, 0.1f, p1.z + _bounds.height);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }

        private void Split()
        {
            var hx = _bounds.width / 2;
            var hz = _bounds.height / 2;
            var sz = new Vector2(hx, hz);

            //split a
            var aLoc = _bounds.position;
            var aRect = new Rect(aLoc, sz);
            //split b
            var bLoc = new Vector2(_bounds.position.x + hx, _bounds.position.y);
            var bRect = new Rect(bLoc, sz);
            //split c
            var cLoc = new Vector2(_bounds.position.x + hx, _bounds.position.y + hz);
            var cRect = new Rect(cLoc, sz);
            //split d
            var dLoc = new Vector2(_bounds.position.x, _bounds.position.y + hz);
            var dRect = new Rect(dLoc, sz);

            //assign QuadTrees
            _childA = QuadTreePool.GetQuadTree(aRect, this);
            _childB = QuadTreePool.GetQuadTree(bRect, this);
            _childC = QuadTreePool.GetQuadTree(cRect, this);
            _childD = QuadTreePool.GetQuadTree(dRect, this);

            for (int i = _bodies.Count - 1; i >= 0; i--)
            {
                var child = GetQuadrant(_bodies[i].Position);
                child.AddBody(_bodies[i]);
                _bodies.RemoveAt(i);
            }
        }

        private QuadTree GetQuadrant(Vector2 point)
        {
            if (_childA == null) return null;
            if (point.x > _bounds.x + _bounds.width / 2)
            {
                if (point.y > _bounds.y + _bounds.height / 2) return _childC;
                else return _childB;
            }
            else
            {
                if (point.y > _bounds.y + _bounds.height / 2) return _childD;
                return _childA;
            }
        }

    }
}