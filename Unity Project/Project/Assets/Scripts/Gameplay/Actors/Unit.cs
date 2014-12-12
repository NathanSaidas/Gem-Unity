using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added class Unit
 * 
 */
#endregion

namespace Gem
{

    /// <summary>
    /// Represents an actor in the world with stats and health.
    /// </summary>
    public class Unit : Actor
    {
        /// <summary>
        /// Represents the max health of the unit
        /// </summary>
        [SerializeField]
        private float m_MaxHealth = 100.0f;
        /// <summary>
        /// Represents the current health of the unit
        /// </summary>
        [SerializeField]
        private float m_CurrentHealth = 10.0f;
        /// <summary>
        /// Represents how much health the unit gains per second.
        /// </summary>
        [SerializeField]
        private float m_HealthRegen = 0.1f;
        /// <summary>
        /// Represents max amount of the resource of the unit
        /// </summary>
        [SerializeField]
        private float m_MaxResource = 100.0f;
        /// <summary>
        /// Represents the units current resource
        /// </summary>
        [SerializeField]
        private float m_CurrentResource = 10.0f;
        /// <summary>
        /// Represents the units current regen.
        /// </summary>
        [SerializeField]
        private float m_ResourceRegen = 0.1f;
        /// <summary>
        /// Defines the type of resource the unit uses.
        /// </summary>
        [SerializeField]
        private ResourceType m_ResourceType = ResourceType.Energy;
        /// <summary>
        /// A list of effects the unit is under taking.
        /// </summary>
        [SerializeField]
        private List<Effect> m_Effects = new List<Effect>();
        /// <summary>
        /// The type of unit this is classified as.
        /// </summary>
        [SerializeField]
        private UnitType m_UnitType = UnitType.None;
        /// <summary>
        /// The race the unit belongs to.
        /// </summary>
        [SerializeField]
        private RaceType m_RaceType = RaceType.Human;
        /// <summary>
        /// A spellbook that contains all the spells of the unit
        /// </summary>
        [SerializeField]
        private UnitSpellBook m_SpellBook = null;
        /// <summary>
        /// An inventory that contains all the items held on the unit.
        /// </summary>
        [SerializeField]
        private UnitInventory m_DefaultInventory = null;
        [SerializeField]
        private UnitInventory[] m_Inventories = new UnitInventory[5];

        void Awake()
        {
            InitializeActor();
        }
        void OnDestroy()
        {
            FinalizeActor();
        }

        public override void OnWorldPause()
        {
            
        }
        public override void OnWorldUnpause()
        {
            
        }

        protected virtual void Update()
        {
            if(World.isLoading || World.isPaused)
            {
                return;
            }
        }

        #region REVIVE METHODS
        /// TODO: Invoke Game Events for each of the Revive Methods.
        public virtual void Revive()
        {
            m_CurrentHealth = m_MaxHealth;
            m_CurrentResource = m_MaxResource;
        }
        public virtual void Revive(Vector3 aLocation)
        {
            m_CurrentHealth = m_MaxHealth;
            m_CurrentResource = m_MaxResource;
            position = aLocation;
        }
        public virtual void Revive(float aHealthPercent)
        {
            m_CurrentHealth = Mathf.Clamp01(aHealthPercent) * m_MaxHealth;
            m_CurrentResource = m_MaxResource;
        }
        public virtual void Revive(float aHealthPercent, float aResourcePercent)
        {
            m_CurrentHealth = Mathf.Clamp01(aHealthPercent) * m_MaxHealth;
            m_CurrentResource = Mathf.Clamp01(aResourcePercent) * m_MaxResource;
        }
        public virtual void Revive(float aHealthPercent, Vector3 aLocation)
        {
            m_CurrentHealth = Mathf.Clamp01(aHealthPercent) * m_MaxHealth;
            m_CurrentResource = m_MaxResource;
            position = aLocation;
        }
        public virtual void Revive(float aHealthPercent, float aResourcePercent, Vector3 aLocation)
        {
            m_CurrentHealth = Mathf.Clamp01(aHealthPercent) * m_MaxHealth;
            m_CurrentResource = Mathf.Clamp01(aResourcePercent) * m_MaxResource;
            position = aLocation;
        }
        #endregion
        #region KILL METHODS
        /// TODO: Invoke Game Events for the following Kill Methods
        public virtual void Kill()
        {
            m_CurrentHealth = 0.0f;
            m_CurrentResource = 0.0f;
        }
        public virtual void Kill(Actor aSource)
        {
            m_CurrentHealth = 0.0f;
            m_CurrentResource = 0.0f;
        }
        #endregion

        public bool HasItem(string aItemName)
        {
            if(m_DefaultInventory != null)
            {
                if (m_DefaultInventory.Exists(aItemName))
                {
                    return true;
                }
            }
            for(int i = 0; i < m_Inventories.Length; i++)
            {
                if(m_Inventories[i] != null && m_Inventories[i].Exists(aItemName))
                {
                    return true;
                }
            }
            return false;
        }

        public Item GetItem(string aItemName)
        {
            Item item = null;
            if(m_DefaultInventory != null)
            {
                item = m_DefaultInventory.GetItem(aItemName);
            }
            if(item == null)
            {
                for(int i = 0; i < m_Inventories.Length; i++)
                {
                    if(m_Inventories[i] != null)
                    {
                        item = m_Inventories[i].GetItem(aItemName);
                        if(item != null)
                        {
                            return item;
                        }
                    }
                }
            }
            return item;
        }

        public void AddItem(string aItemName)
        {
            if(m_DefaultInventory != null)
            {
                if(m_DefaultInventory.AddItem(aItemName))
                {
                    return;
                }
            }
            for(int i = 0; i < m_Inventories.Length; i++)
            {

            }
        }
        public void RemoveItem(string aItemName)
        {

        }

        public void UseItem(string aItemName)
        {
            ///Find the item. Then proceed to use it.
            Item item = null;
            if(m_DefaultInventory != null)
            {
                item = m_DefaultInventory.GetItem(aItemName);
                if(item != null)
                {
                    UseItem(m_DefaultInventory, item);
                    return;
                }
            }
            for(int i = 0; i < m_Inventories.Length; i++)
            {
                if(m_Inventories[i] != null)
                {
                    item = m_Inventories[i].GetItem(aItemName);
                    if(item != null)
                    {
                        UseItem(m_Inventories[i], item);
                        return;
                    }
                }
            }
        }

        private void UseItem(UnitInventory aInventory, Item aItem)
        {
            ///Find the item. Then proceed to use it.
            if (aInventory != null && aItem != null)
            {
                ///Check the destroy on use field and see if its true or false.
                Item item = aItem;
                if (item.consumeable)
                {
                    if(item.stackable == true)
                    {
                        item.RemoveStack(1);
                        if(item.stacks == 0)
                        {
                            aInventory.RemoveItem(item);
                        }
                    }
                    else
                    {
                        aInventory.RemoveItem(item);
                    }
                }
            }
        }

        public void CanUseItem(Item aItem)
        {

        }
        

        //bool IsFriend(Unit aUnit)
        //{
        //    return aUnit != null && aUnit.pla
        //}

        bool isAlive
        {
            get { return m_CurrentHealth > 0.0f; }
        }

    }
}