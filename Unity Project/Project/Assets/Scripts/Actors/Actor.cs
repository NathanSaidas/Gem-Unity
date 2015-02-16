// -- System
using System;
using System.Collections;
using System.Collections.Generic;

// -- Unity
using UnityEngine;

// -- Gem
using Gem.Extensions;

#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added Actor Class
 * February, 14, 2015 - Nathan Hanlan - Made InvokeActorEvent virtual
 * February, 14, 2015 - Nathan Hanlan - Removed OnSelectActor and OnDeselectActor event handlers.
 */
#endregion

namespace Gem
{
    public class Actor : MonoBehaviour , IActor
    {
        /// <summary>
        /// A mask to denote an actors identifier eg. Giant Ethereal actor or a mechanical structure
        /// </summary>
        [SerializeField]
        [EnumFlags]
        private ActorIdentifier m_ActorIdentifier = (ActorIdentifier)0;

        // -- Actor Initialization
        /// <summary>
        /// If the Actor is Initialized, InitializeActor will not invoke OnInitializeActor
        /// This ensures that an actor may only be initialized once.
        /// </summary>
        private bool m_IsInitialized = false;
        /// <summary>
        /// If the Actor is finalized,  FinalizeActor will not invoke OnFinalizeActor
        /// </summary>
        private bool m_IsFinalized = false;
        
        // -- Object State
        /// <summary>
        /// This is used with the ActorManager to properly destroy an actor.
        /// </summary>
        private bool m_IsActorAlive = true;

        // -- Object Info
        /// <summary>
        /// The key for the actor. This is used with ActorManager for storing actors inside a Dictionary. (Key,Value)
        /// </summary>
        [SerializeField]
        private string m_ActorKey = string.Empty;

        /// <summary>
        /// Properties of the actor.
        /// </summary>
        private List<ActorProperty> m_ActorProperties = new List<ActorProperty>();


        #region IACTOR
        /// <summary>
        /// Retrieves the name of the actor.
        /// </summary>
        /// <returns>The name of the Object the actor is attached to.</returns>
        public string GetActorName()
        {
            return gameObject.name;
        }

        /// <summary>
        /// Retrieves the type of actor this is.
        /// </summary>
        /// <returns>Returns the type of actor this is.</returns>
        public ActorIdentifier GetActorIdentifier()
        {
            return m_ActorIdentifier;
        }

        /// <summary>
        /// Retrieves a reference to the gameobject this actor can use.
        /// </summary>
        /// <returns></returns>
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        /// <summary>
        /// Retrieves a reference to the state of the object.
        /// </summary>
        /// <returns></returns>
        public bool IsObjectAlive()
        {
            return m_IsActorAlive;
        }

        #endregion


        #region ACTOR INITIALIZATION AND DESTRUCTION
        /// <summary>
        /// Tells the actor to put itself up for destruction within the actor manager.
        /// It also invokes FinalizeActor
        /// </summary>
        private void Destroy()
        {
            if(m_IsActorAlive)
            {
                FinalizeActor();
                ActorManager.Destroy(this);
            }
            m_IsActorAlive = false;
        }

        /// <summary>
        /// A static method to destroy actors.
        /// </summary>
        /// <param name="aActor">The actor to destroy</param>
        public static void Destroy(Actor aActor)
        {
            if(aActor != null)
            {
                aActor.Destroy();
            }
        }

        /// <summary>
        /// Uses GameObjectExtensions to clone the gameobject. This will then search for the actor and 
        /// do some internal initialization.
        ///     - Generate a Unique Object Key
        ///     - Registers with the ActorManager using the Generated Object Key
        /// </summary>
        /// <param name="aGameObject">The gameobject to clone</param>
        /// <returns>Returns null if there was no actor on the root gameobject.</returns>
        public static Actor Instantiate(GameObject aGameObject)
        {
            return Instantiate(aGameObject, Vector3.zero, Quaternion.identity);
        }
        
        /// <summary>
        /// Uses GameObjectExtensions to clone the gameobject. This will then search for the actor and 
        /// do some internal initialization.
        ///     - Generate a Unique Object Key
        ///     - Registers with the ActorManager using the Generated Object Key
        /// </summary>
        /// <param name="aGameObject">The gameobject to clone</param>
        /// <param name="aPosition">The position of the gameobject</param>
        /// <param name="aRotation">The rotation of the gameobject</param>
        /// <returns>Returns null if there was no actor on the root gameobject.</returns>
        public static Actor Instantiate(GameObject aGameObject, Vector3 aPosition, Quaternion aRotation)
        {
            GameObject gameObject = GameObjectExtensions.InstantiateActor(aGameObject,aPosition,aRotation);
            if (gameObject != null)
            {
                Actor actor = gameObject.GetComponent<Actor>();
                if (actor != null)
                {
                    actor.m_ActorKey = Guid.NewGuid().ToString();
                }
                return actor;
            }
            return null;
        }

        /// <summary>
        /// Uses GameObjectExtensions to clone the gameobject. This will then search for the actor and 
        /// do some internal initialization.
        ///     - Uses the provided key for the actor created
        ///     - Registers with the ActorManager using the given Object Key
        /// </summary>
        /// <param name="aGameObject">The gameobject to clone</param>
        /// <param name="aObjectKey">A unique key to give to the object. (For Networking Object Sync) </param>
        /// <returns>Returns null if there was no actor on the root gameobject.</returns>
        public static Actor Instantiate(GameObject aGameObject, string aObjectKey)
        {
            return Instantiate(aGameObject, Vector3.zero, Quaternion.identity, aObjectKey);
        }

        /// <summary>
        /// Uses GameObjectExtensions to clone the gameobject. This will then search for the actor and 
        /// do some internal initialization.
        ///     - Uses the provided key for the actor created
        ///     - Registers with the ActorManager using the given Object Key
        /// </summary>
        /// <param name="aGameObject">The gameobject to clone</param>
        /// <param name="aPosition">The position of the gameobject</param>
        /// <param name="aRotation">The rotation of the gameobject</param>
        /// <param name="aObjectKey">A unique key to give to the object. (For Networking Object Sync) </param>
        /// <returns>Returns null if there was no actor on the root gameobject.</returns>
        public static Actor Instantiate(GameObject aGameObject, Vector3 aPosition, Quaternion aRotation, string aObjectKey)
        {
            GameObject gameObject = GameObjectExtensions.InstantiateActor(aGameObject, aPosition, aRotation);
            if (gameObject != null)
            {
                Actor actor = gameObject.GetComponent<Actor>();
                if (actor != null)
                {
                    actor.m_ActorKey = aObjectKey;
                }
                return actor;
            }
            return null;
        }

        /// <summary>
        /// A one time initialization method. This will only invoke OnInitializeActor once the entire lifetime of the object.
        /// </summary>
        public void InitializeActor()
        {
            if(!m_IsInitialized)
            {
                m_IsInitialized = true;
                if(string.IsNullOrEmpty(m_ActorKey))
                {
                    m_ActorKey = Guid.NewGuid().ToString();
                }
                ActorManager.RegisterActor(this);
                OnInitializeActor();
                
            }
        }

        /// <summary>
        /// A one time finalizeation method. This will only invoke OnFinalizeActor once the entire lifetime of the object.
        /// </summary>
        public void FinalizeActor()
        {
            if(!m_IsFinalized)
            {
                m_IsFinalized = true;
                ActorManager.UnregisterActor(this);
                OnFinalizeActor();
            }
        }

        #endregion


        #region HELPERS
        /// <summary>
        /// Use to invoke an event on the actor.
        /// </summary>
        /// <param name="aEvent">The event data. (Type,Context)</param>
        public virtual void InvokeActorEvent(ActorEvent aEvent)
        {

        }

        public void AddProperty(ActorProperty aProperty)
        {
            if(aProperty != null)
            {
                m_ActorProperties.Add(aProperty);
            }
        }
        public void RemoveProperty(ActorProperty aProperty)
        {
            if(aProperty != null)
            {
                m_ActorProperties.Remove(aProperty);
            }
        }
        public void RemoveProperty(string aName)
        {
            if(!string.IsNullOrEmpty(aName))
            {
                m_ActorProperties.RemoveAll(Element => Element.name == aName);
            }
        }

        #endregion

        #region VIRTUALS

        /// <summary>
        /// Override this method to initialize the actor once in its entire lifetime.
        /// 
        /// Entry to this method comes from InitializationActor. Common ways of invoking that is through
        ///     - Unity Awake message method
        ///     - Unity Start message method
        ///     - Explicitly Initialize using Actor static Instantiate Methods
        ///     - Explicitly Initialize using GameObjectExtensions Instantiate Methods
        /// </summary>
        protected virtual void OnInitializeActor()
        {

        }
        
        /// <summary>
        /// Override this method to finalize the actor once in its entire lifetime.
        /// 
        /// Entry to this method comes from the static Destroy Method. Common ways of invoking that is through
        ///     - OnDestroy
        ///     - Explicity use of the Actor static method
        /// </summary>
        protected virtual void OnFinalizeActor()
        {

        }

        /// <summary>
        /// Overrride this method to define what kind of actor this actor is.
        /// </summary>
        /// <returns></returns>
        public virtual ActorType GetActorType()
        {
            return ActorType.Actor;
        }

        #endregion



        #region PROPERTIES
        /// <summary>
        /// Returns true if the actor was initialized with InitializeActor
        /// </summary>
        public bool isInitialized
        {
            get { return m_IsInitialized; }
        }

        /// <summary>
        /// Returns true if the actor was finalized with FinalizeActor
        /// </summary>
        public bool isFinalized
        {
            get { return m_IsFinalized; }
        }

        /// <summary>
        /// Returns true if the actor is considered alive still.
        /// This refers to the state regarding the object and the ActorManagers perspective on it.
        /// </summary>
        public bool isActorAlive
        {
            get { return m_IsActorAlive; }
        }

        /// <summary>
        /// Returns the unique key of the object within the actor manager.
        /// </summary>
        public string actorKey
        {
            get { return m_ActorKey; }
        }
        #endregion

    }
}