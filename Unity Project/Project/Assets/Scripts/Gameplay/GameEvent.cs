using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#region CHANGE LOG
/* December, 11, 2014 - Nathan Hanlan - Added & implemented class GameEvent
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// The class used to transfer game events globally.
    /// </summary>
    public class GameEvent : MonoBehaviour
    {
        #region SINGLETON
        private static GameEvent s_Instance = null;
        private static GameEvent instance
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
                    persistent = new GameObject(Constants.GAME_OBJECT_PERSISTENT);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if (persistent != null)
            {
                s_Instance = persistent.GetComponent<GameEvent>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<GameEvent>();
                }
            }
        }
        private static bool SetInstance(GameEvent aInstance)
        {
            if(s_Instance == null)
            {
                s_Instance = aInstance;
                return true;
            }
            return false;
        }
        private static void DestroyInstance(GameEvent aInstance)
        {
            if(s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        /// <summary>
        /// A queue of events to processed
        /// </summary>
        private Queue<GameEventData> m_GameEvents = null;
        private WaitForEndOfFrame m_Wait = new WaitForEndOfFrame();

        #region RECEIVERS

        /// <summary>
        /// Receivers for game events.
        /// </summary>
        private HashSet<IGameEventReceiver> m_GameReceivers = new HashSet<IGameEventReceiver>();
        /// <summary>
        /// Receivers for actor events
        /// </summary>
        private HashSet<IGameEventReceiver> m_ActorReceivers = new HashSet<IGameEventReceiver>();
        /// <summary>
        /// Receivers for unit events
        /// </summary>
        private HashSet<IGameEventReceiver> m_UnitReceivers = new HashSet<IGameEventReceiver>();
        /// <summary>
        /// Receivers for interactive events
        /// </summary>
        private HashSet<IGameEventReceiver> m_InteractiveReceivers = new HashSet<IGameEventReceiver>();
        /// <summary>
        /// Receivers for item events
        /// </summary>
        private HashSet<IGameEventReceiver> m_ItemReceievers = new HashSet<IGameEventReceiver>();
        /// <summary>
        /// Receievers for network events
        /// </summary>
        private HashSet<IGameEventReceiver> m_NetworkReceivers = new HashSet<IGameEventReceiver>();

        #endregion

        private void Start()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
            StartCoroutine(ProcessRoutine());
        }
        private void Destroy()
        {
            StopCoroutine(ProcessRoutine());
            DestroyInstance(this);
        }
        /// <summary>
        /// Invokes a game event into the system.
        /// </summary>
        /// <param name="aType">The type of event to invoke.</param>
        /// <param name="aID">The specific ID to invoke</param>
        /// <param name="aSender">The invoker of the event</param>
        /// <param name="aTriggering">The triggering element of the event</param>
        public static void Invoke(GameEventType aType, GameEventID aID, object aSender, object aTriggering)
        {
            if (instance != null)
            {
                instance.m_GameEvents.Enqueue(new GameEventData(Time.time, aType, aID, aSender, aTriggering, null));
            }
        }
        /// <summary>
        /// Invokes a game event into the system.
        /// </summary>
        /// <param name="aType">The type of event to invoke.</param>
        /// <param name="aID">The specific ID to invoke</param>
        /// <param name="aSender">The invoker of the event</param>
        /// <param name="aTriggering">The triggering element of the event</param>
        /// <param name="aParams">A list of extra objects to be passed in </param>
        public static void Invoke(GameEventType aType, GameEventID aID, object aSender, object aTriggering, params object[] aParams)
        {
            if(instance != null)
            {
                instance.m_GameEvents.Enqueue(new GameEventData(Time.time, aType, aID, aSender, aTriggering, aParams));
            }
        }
        /// <summary>
        /// Register for an event based on type.
        /// </summary>
        /// <param name="aReceiver"></param>
        /// <param name="aType"></param>
        public static void Register(IGameEventReceiver aReceiver, GameEventType aType)
        {
            if(instance == null || aReceiver == null)
            {
                return;
            }
            switch(aType)
            {
                case GameEventType.Game:
                    instance.m_GameReceivers.Add(aReceiver);
                    break;
                case GameEventType.Actor:
                    instance.m_ActorReceivers.Add(aReceiver);
                    break;
                case GameEventType.Unit:
                    instance.m_UnitReceivers.Add(aReceiver);
                    break;
                case GameEventType.Interactive:
                    instance.m_InteractiveReceivers.Add(aReceiver);
                    break;
                case GameEventType.Item:
                    instance.m_ItemReceievers.Add(aReceiver);
                    break;
                case GameEventType.Network:
                    instance.m_NetworkReceivers.Add(aReceiver);
                    break;
            }
        }
        /// <summary>
        /// This will remove all instances of the object from the event receiver list.
        /// </summary>
        /// <param name="aReceiver"></param>
        /// <param name="aType"></param>
        public static void Unregister(IGameEventReceiver aReceiver, GameEventType aType)
        {
            if (instance == null || aReceiver == null)
            {
                return;
            }
            switch (aType)
            {
                case GameEventType.Game:
                    instance.m_GameReceivers.RemoveWhere(Element => Element == aReceiver);
                    break;
                case GameEventType.Actor:
                    instance.m_ActorReceivers.RemoveWhere(Element => Element == aReceiver);
                    break;
                case GameEventType.Unit:
                    instance.m_UnitReceivers.RemoveWhere(Element => Element == aReceiver);
                    break;
                case GameEventType.Interactive:
                    instance.m_InteractiveReceivers.RemoveWhere(Element => Element == aReceiver);
                    break;
                case GameEventType.Item:
                    instance.m_ItemReceievers.RemoveWhere(Element => Element == aReceiver);
                    break;
                case GameEventType.Network:
                    instance.m_NetworkReceivers.RemoveWhere(Element => Element == aReceiver);
                    break;
            }
        }
        /// <summary>
        /// The coroutine used to process events
        /// </summary>
        /// <returns></returns>
        private IEnumerator ProcessRoutine()
        {
            while(true)
            {
                while(m_GameEvents.Count > 0)
                {
                    Process(m_GameEvents.Dequeue());
                }
                yield return m_Wait;
            }
        }

        /// <summary>
        /// Processes a game event.
        /// </summary>
        /// <param name="aEvent">The event to process</param>
        private void Process(GameEventData aEvent)
        {
            if(aEvent != null)
            {
                switch(aEvent.type)
                {
                    case GameEventType.Game:
                        {
                            IEnumerator<IGameEventReceiver> receievers = m_GameReceivers.GetEnumerator();
                            while(receievers.MoveNext())
                            {
                                if(receievers.Current != null)
                                {
                                    receievers.Current.OnReceiveEvent(aEvent);
                                }
                            }
                        }
                        break;
                    case GameEventType.Actor:
                        {
                            IEnumerator<IGameEventReceiver> receievers = m_ActorReceivers.GetEnumerator();
                            while (receievers.MoveNext())
                            {
                                if (receievers.Current != null)
                                {
                                    receievers.Current.OnReceiveEvent(aEvent);
                                }
                            }
                        }
                        break;
                    case GameEventType.Unit:
                        {
                            IEnumerator<IGameEventReceiver> receievers = m_UnitReceivers.GetEnumerator();
                            while (receievers.MoveNext())
                            {
                                if (receievers.Current != null)
                                {
                                    receievers.Current.OnReceiveEvent(aEvent);
                                }
                            }
                        }
                        break;
                    case GameEventType.Interactive:
                        {
                            IEnumerator<IGameEventReceiver> receievers = m_InteractiveReceivers.GetEnumerator();
                            while (receievers.MoveNext())
                            {
                                if (receievers.Current != null)
                                {
                                    receievers.Current.OnReceiveEvent(aEvent);
                                }
                            }
                        }
                        break;
                    case GameEventType.Item:
                        {
                            IEnumerator<IGameEventReceiver> receievers = m_ItemReceievers.GetEnumerator();
                            while (receievers.MoveNext())
                            {
                                if (receievers.Current != null)
                                {
                                    receievers.Current.OnReceiveEvent(aEvent);
                                }
                            }
                        }
                        break;
                    case GameEventType.Network:
                        {
                            IEnumerator<IGameEventReceiver> receievers = m_NetworkReceivers.GetEnumerator();
                            while (receievers.MoveNext())
                            {
                                if (receievers.Current != null)
                                {
                                    receievers.Current.OnReceiveEvent(aEvent);
                                }
                            }
                        }
                        break;
                    default:

                        break;
                }
            }
        }

    }
}