#region CHANGE LOG
/*  January, 28, 2015 - Nathan Hanlan - Added file GameObjectExtensions
 * 
 */
#endregion

using UnityEngine;
using System.Collections.Generic;


namespace Gem
{
    namespace Extensions
    {
        public static class GameObjectExtensions
        {
            /// <summary>
            /// Spawns a gameobject at position (0,0,0) and rotation (0,0,0)
            /// Searches for Sibling IActors of the GameObject and the Children IActors of the GameObject
            /// and invokes InitializeActor
            /// </summary>
            /// <param name="aObject">The object to clone.</param>
            /// <returns>Returns the cloned gameobject</returns>
            public static GameObject InstantiateActor(GameObject aObject)
            {
                return InstantiateActor(aObject, Vector3.zero, Quaternion.identity);
            }
            
            /// <summary>
            /// Spawns a gameobject at the given position and rotation.
            /// Searches for Sibling IActors of the GameObject and the Children IActors of the GameObject
            /// and invokes InitializeActor
            /// </summary>
            /// <param name="aObject">The object to clone.</param>
            /// <param name="aPosition">The position to place the object.</param>
            /// <param name="aRotation">The rotation to place the object.</param>
            /// <returns>Returns the cloned gameobject.</returns>
            public static GameObject InstantiateActor(GameObject aObject, Vector3 aPosition, Quaternion aRotation)
            {
                GameObject gameobject = GameObject.Instantiate(aObject, aPosition, aRotation) as GameObject;

                IActor[] actors = gameobject.GetComponents(typeof(IActor)) as IActor[];

                if(actors != null)
                {
                    foreach(IActor actor in actors)
                    {
                        actor.InitializeActor();
                    }
                }

                actors = gameobject.GetComponentsInChildren(typeof(IActor)) as IActor[];

                if(actors != null)
                {
                    foreach(IActor actor in actors)
                    {
                        actor.InitializeActor();
                    }
                }

                return gameobject;
            }
        }
    }
}


