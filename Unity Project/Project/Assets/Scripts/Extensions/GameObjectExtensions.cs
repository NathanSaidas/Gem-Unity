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
            public static GameObject InstantiateActor(GameObject aObject)
            {
                return InstantiateActor(aObject, Vector3.zero, Quaternion.identity);
            }
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


