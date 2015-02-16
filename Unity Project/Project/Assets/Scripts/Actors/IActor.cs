#region CHANGE LOG
/*  January, 28, 2015 - Nathan Hanlan - Added file IActor
 * 
 */
#endregion

using UnityEngine;

namespace Gem
{

    /// <summary>
    /// Defines the implementation required for an IActor type class.
    /// </summary>
    public interface IActor
    {
        /// <summary>
        /// Retrieves the name of the actor.
        /// </summary>
        /// <returns></returns>
        string GetActorName();
        /// <summary>
        /// Retrieves the type of actor this is.
        /// </summary>
        /// <returns></returns>
        ActorIdentifier GetActorIdentifier();
        /// <summary>
        /// Returns a reference to a game object this interface is being managed by.
        /// </summary>
        GameObject GetGameObject();
        /// <summary>
        /// Returns true if the object is considered alive.
        /// </summary>
        bool IsObjectAlive();
        /// <summary>
        /// A method to initialize the actor when being instantiated.
        /// </summary>
        void InitializeActor();
        /// <summary>
        /// A method to finalize the actor when being destroyed.
        /// </summary>
        void FinalizeActor();


    }
}