using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// -- Gem
using Gem.Coroutines;

namespace Gem
{
    /// <summary>
    /// This class is designed based on the singleton pattern and thus should have an object in the scene to instantiate it.
    /// This class keeps a Queue collection of Garbage items and destroys them at the end of each frame.
    /// Only IActors may be placed on the Queue.
    /// </summary>
    public class ActorManager : MonoBehaviour
    {
        #region SINGLETON
        private static ActorManager s_Instance = null;
        private static ActorManager instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        private static void CreateInstance()
        {
            GameObject persistent = GameObject.Find(Constants.GAME_OBJECT_PERSISTENT);
            if (persistent == null)
            {
                if (Application.isPlaying)
                {
                    persistent = new GameObject(Constants.GAME_OBJECT_ACTOR_MANAGER);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if (persistent != null)
            {
                s_Instance = persistent.GetComponent<ActorManager>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<ActorManager>();
                }
            }
        }
        private static bool SetInstance(ActorManager aInstance)
        {
            if (s_Instance == null)
            {
                s_Instance = aInstance;
                return true;
            }
            return false;
        }
        private static void DestroyInstance(ActorManager aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion


        private Queue<IActor> m_GarbageQueue = new Queue<IActor>();
        private Dictionary<string, Actor> m_Actors = new Dictionary<string, Actor>();

        public int m_QueueSize = 0;

        private void Awake()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                DebugUtils.MultipleInstances<ActorManager>();
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            DestroyInstance(this);
        }


        private void Update()
        {
            m_QueueSize = m_GarbageQueue.Count;
            StartCoroutine(Collect());
        }

        private IEnumerator Collect()
        {
            yield return new WaitForEndOfFrame();
            while(m_GarbageQueue.Count > 0)
            {
                IActor actor = m_GarbageQueue.Dequeue();
                if(actor != null)
                {
                    GameObject goActor = actor.GetGameObject();
                    if(goActor != null)
                    {
                        Debug.Log("Destroying: " + goActor);
                        Destroy(goActor);
                    }
                    else
                    {
                        Debug.Log("Missing GameObject");
                    }
                }
            }
        }

        /// <summary>
        /// Destroys an Actor. Invokes FinalizeActor.
        /// The destroyed actor gets added to the garbage queue and will be handled at the end of the frame.
        /// </summary>
        /// <param name="aActor">The actor to destroy.</param>
        public static void Destroy(IActor aActor)
        {
            bool isNull = aActor == null;


            if(!isNull && aActor.IsObjectAlive())
            {
                ActorManager manager = ActorManager.instance;
                if(manager != null)
                {
                    manager.m_GarbageQueue.Enqueue(aActor);
                }
            }
            else
            {
                Debug.Log("Bad Object");
            }
        }

        public static void Destroy(Actor aActor)
        {
            bool isNull = aActor == null;


            if (!isNull && aActor.IsObjectAlive())
            {
                ActorManager manager = ActorManager.instance;
                if (manager != null)
                {
                    manager.m_GarbageQueue.Enqueue(aActor);
                }
            }
            else
            {
                Debug.Log("Bad Object");
            }
        }

        /// <summary>
        /// Registers a Actor into the Actor Manager. 
        /// An actor with an invalid key, (null or empty) cannot be registered
        /// An actor whos key already exists is not recommended and will create an error. But it is allowed.
        /// </summary>
        /// <param name="aActor">The Actor to register</param>
        public static void RegisterActor(Actor aActor)
        {
            if(aActor != null)
            {
                ///Check String
                if(string.IsNullOrEmpty(aActor.actorKey))
                {
                    DebugUtils.LogError(DebugConstants.GetError(ErrorCode.ACTOR_INVALID_KEY) + aActor.GetActorName(), LogVerbosity.LevelThree);
                    return;
                }
                ActorManager manager = ActorManager.instance;
                if(manager != null)
                {
                    ///Check Key Exist
                    if(manager.m_Actors.ContainsKey(aActor.actorKey))
                    {
                        DebugUtils.LogError(DebugConstants.GetError(ErrorCode.ACTOR_MULTIPLE_SAME_KEY) + aActor.actorKey);
                    }
                    manager.m_Actors.Add(aActor.actorKey, aActor);
                }
            }
        }

        /// <summary>
        /// Unregisters a Actor from the Actor Manager.
        /// </summary>
        /// <param name="aActor">The actor to Unregister.</param>
        public static void UnregisterActor(Actor aActor)
        {
            if(aActor != null)
            {
                ///Check String
                if(string.IsNullOrEmpty(aActor.actorKey))
                {
                    DebugUtils.LogError(DebugConstants.GetError(ErrorCode.ACTOR_INVALID_KEY) + aActor.GetActorName(), LogVerbosity.LevelThree);
                    return;
                }

                ActorManager manager = ActorManager.instance;
                if(manager != null)
                {
                    manager.m_Actors.Remove(aActor.actorKey);
                }
            }
        }

        

    }

}

