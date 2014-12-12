using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added Actor Class
 *
 */
#endregion

namespace Gem
{
    public class Actor : MonoBehaviour
    {
        /// <summary>
        /// ID within the Gem World
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private string m_UniqueID = string.Empty;
        /// <summary>
        /// Determines whether or not the this actor will be spawned within the world by the world.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private bool m_LoadedOnWorldLoad = false;
        /// <summary>
        /// The name of the actor within the gem world
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private string m_ActorName = string.Empty;
        /// <summary>
        /// A identifier mask to identify the unit within the gem world.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private ActorIdentifier m_Identifier = ActorIdentifier.None;
        /// <summary>
        /// A reference to the player currently controlling this actor.
        /// </summary>
        [HideInInspector]
        [SerializeField]  
        private PlayerController m_PlayerController = null;
        /// <summary>
        /// The target player of this actor. This is useful for placing actors in the world while editting the game.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private Player m_Player = Player.Environment;
        
        

        /// <summary>
        /// Call this method when any actor has been instantiated in the world.
        /// </summary>
        protected void InitializeActor()
        {
            World.Register(this);
            if(!m_LoadedOnWorldLoad || m_UniqueID.Length == 0)
            {
                GenerateID();
            }
        }
        /// <summary>
        /// Call this method when any actor is being removed from the world.
        /// </summary>
        protected void FinalizeActor()
        {
            World.Unregister(this);
        }


        public virtual void OnWorldReset()
        {

        }
        public virtual void OnWorldPause()
        {

        }
        public virtual void OnWorldUnpause()
        {

        }
        public virtual void OnWorldLoaded()
        {

        }
        public virtual void OnWorldUnload()
        {

        }
        public virtual void OnWorldSave(int aPass)
        {

        }
        public virtual void OnWorldLoad(int aPass)
        {

        }
        /// <summary>
        /// Enables the Actor
        /// </summary>
        public void Enable()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// Disables the Actor
        /// </summary>
        public void Disable()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Generates the unique Actor ID
        /// </summary>
        public void GenerateID()
        {
            m_UniqueID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Accessor to the position of the actor in the world.
        /// </summary>
        public Vector3 position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        /// <summary>
        /// Accessor to the actors rotation in the world.
        /// </summary>
        public Quaternion rotation
        {
            get { return transform.rotation; }
            set { transform.rotation = value; }
        }
        /// <summary>
        /// Accessor to the actors scale in the world.
        /// </summary>
        public Vector3 scale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }
        /// <summary>
        /// Accessor to the unique ID of the actor.
        /// </summary>
        public string uniqueID
        {
            get { return m_UniqueID; }
        }
        /// <summary>
        /// Accessor to the actors name
        /// </summary>
        public string actorName
        {
            get { return m_ActorName; }
            set { m_ActorName = value; }
        }
        /// <summary>
        /// Accessor to the actors identifier
        /// </summary>
        public ActorIdentifier identifier
        {
            get { return m_Identifier;}
            set { m_Identifier = value; }
        }
        /// <summary>
        /// The layer of which the actor appear
        /// </summary>
        public int layer
        {
            get { return gameObject.layer; }
            set { gameObject.layer = value; }
        }
        /// <summary>
        /// The player who owns this Actor
        /// </summary>
        public PlayerController playerController
        {
            get { return m_PlayerController; }
            set { m_PlayerController = value; }
        }
        public Player player
        {
            get { return m_PlayerController == null ? m_Player : m_PlayerController.player; }
        }
        /// <summary>
        /// Whether or not the actor is enabled. (The gameobject )
        /// </summary>
        public bool isEnabled
        {
            get { return gameObject.activeSelf; }
        }

    }
}