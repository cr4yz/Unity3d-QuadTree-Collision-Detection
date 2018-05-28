using UnityEngine;

namespace Peril.Physics
{
    public interface IQuadTreeBody
    {
        Vector2 Position { get; }
        bool QuadTreeIgnore { get; }
    }
}

