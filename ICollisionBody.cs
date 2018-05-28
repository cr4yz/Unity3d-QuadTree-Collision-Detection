
namespace Peril.Physics
{
    public interface ICollisionBody
    {

        int RefId { get; set; }

        /// <summary>
        /// Skip this body when testing for collisions
        /// </summary>
        bool Sleeping { get; }

        /// <summary>
        /// 
        /// </summary>
        ICollisionShape CollisionShape { get; }

        /// <summary>
        /// Called each frame of collision
        /// </summary>
        /// <param name="result"></param>
        /// <param name="menace"></param>
        void OnCollision(CollisionResult result, ICollisionBody other);

    }
}

