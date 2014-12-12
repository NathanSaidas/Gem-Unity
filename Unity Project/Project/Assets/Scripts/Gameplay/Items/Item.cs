using UnityEngine;
using System;
using System.Linq;
using System.Runtime.CompilerServices; //For Indexer Attribute
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added Class
 * 
 */
#endregion

namespace Gem
{

    [Serializable]
    public class Item : ScriptableObject , ISaveable
    {
#if UNITY_EDITOR
        [Tooltip("The name of the item within the database and world.")]
#endif
        [SerializeField]
        private string m_ItemName = string.Empty;
#if UNITY_EDITOR
        [Tooltip("Description of the item.")]
#endif
        [SerializeField]
        private string m_Description = string.Empty;
#if UNITY_EDITOR
        [Tooltip("A unique ID of the item within the data base. (This will get assigned as the item is loaded into the database)")]
#endif
        [SerializeField]
        private string m_UniqueID = string.Empty;
#if UNITY_EDITOR
        [Tooltip("The type of item this is.")]
#endif
        [SerializeField]
        private ItemFlags m_ItemFlags = ItemFlags.None;
#if UNITY_EDITOR
        [Tooltip("The slot this item belongs in.")]
#endif
        [SerializeField]
        private ItemSlot m_ItemSlot = ItemSlot.Torso;
#if UNITY_EDITOR
        [Tooltip("How much a vendor sells for the item.")]
#endif
        [SerializeField]
        private int m_PurchaseCost = 0;
#if UNITY_EDITOR
        [Tooltip("How much the item sells to a vendor for")]
#endif
        [SerializeField]
        private int m_SellCost = 0;
#if UNITY_EDITOR
        [Tooltip("How many stacks the item currently has.")]
#endif
        [SerializeField]
        private byte m_Stacks = 0;
#if UNITY_EDITOR
        [Tooltip("The maximum stacks an item can have.")]
#endif
        [SerializeField]
        private byte m_MaxStacks = 1;
#if UNITY_EDITOR
        [Tooltip("The quality of the item")]
#endif
        [SerializeField]
        private ItemQuality m_ItemQuality = ItemQuality.Common;
#if UNITY_EDITOR
        [Tooltip("How much durability the item has.")]
#endif
        [SerializeField]
        private short m_Durability = 100;
#if UNITY_EDITOR
        [Tooltip("How much durability the item could possibly have.")]
#endif
        [SerializeField]
        private short m_MaxDurability = 100;
        /// <summary>
        /// The inventory this item is stored in.
        /// </summary>
        [NonSerialized]
        private UnitInventory m_Inventory = null;

  
        /// <summary>
        /// Gets called to save data to a database.
        /// </summary>
        /// <param name="aFormatter"></param>
        /// <param name="aStream"></param>
        public void OnSave(BinaryFormatter aFormatter, Stream aStream)
        {
            aFormatter.Serialize(aStream, m_ItemName);
            aFormatter.Serialize(aStream, m_Description);
            aFormatter.Serialize(aStream, m_UniqueID);
            aFormatter.Serialize(aStream, (int)m_ItemFlags);
            aFormatter.Serialize(aStream, (int)m_ItemSlot);
            aFormatter.Serialize(aStream, m_PurchaseCost);
            aFormatter.Serialize(aStream, m_SellCost);
            aFormatter.Serialize(aStream, m_MaxStacks);
            aFormatter.Serialize(aStream, m_ItemQuality);
            aFormatter.Serialize(aStream, m_MaxDurability);
        }
        /// <summary>
        /// Gets called to load data from a database
        /// </summary>
        /// <param name="aFormatter"></param>
        /// <param name="aStream"></param>
        public void OnLoad(BinaryFormatter aFormatter, Stream aStream)
        {
            Debug.Log("Loading Item");
            m_ItemName = (string)aFormatter.Deserialize(aStream);
            m_Description = (string)aFormatter.Deserialize(aStream);
            m_UniqueID = (string)aFormatter.Deserialize(aStream);
            m_ItemFlags = (ItemFlags)aFormatter.Deserialize(aStream);
            m_ItemSlot = (ItemSlot)aFormatter.Deserialize(aStream);
            m_PurchaseCost = (int)aFormatter.Deserialize(aStream);
            m_SellCost = (int)aFormatter.Deserialize(aStream);
            m_MaxStacks = (byte)aFormatter.Deserialize(aStream);
            Debug.Log("Loaded " + m_ItemName + " item with id " + m_UniqueID);
            //m_ItemQuality = (ItemQuality)aFormatter.Deserialize(aStream);
            //m_MaxDurability = (short)aFormatter.Deserialize(aStream);

            
        }

        /// <summary>
        /// Adds the stacks to the item.
        /// </summary>
        /// <param name="aStacks"></param>
        /// <returns>Returns the amount of items that overflowed.</returns>
        public byte AddStack(byte aStacks)
        {
            if(stackable)
            {
                if(aStacks + m_Stacks <= m_MaxStacks)
                {
                    m_Stacks += aStacks;
                    return 0;
                }
                else
                {
                    int overflow = (int)aStacks + (int)m_Stacks - (int)m_MaxStacks;
                    m_Stacks = m_MaxStacks;
                    return (byte)overflow;
                }
            }
            return 0;
        }
        /// <summary>
        /// Adds the stacks to the item. Does not add stacks if the given stacks overflow.
        /// </summary>
        /// <param name="aStacks"></param>
        /// <param name="aForce"></param>
        /// <returns>Returns the stacks that overflowed if forced.</returns>
        public byte AddStack(byte aStacks, bool aForce)
        {
            if (stackable)
            {
                if (aStacks + m_Stacks <= m_MaxStacks)
                {
                    m_Stacks += aStacks;
                    return 0;
                }
                else if(aForce == true)
                {
                    int overflow = (int)aStacks + (int)m_Stacks - (int)m_MaxStacks;
                    m_Stacks = m_MaxStacks;
                    return (byte)overflow;
                }
            }
            return 0;
        }
        /// <summary>
        /// Removes the stacks from the item.
        /// </summary>
        /// <param name="aStacks"></param>
        /// <returns>The amount of stacks removed.</returns>
        public byte RemoveStack(byte aStacks)
        {
            if (stackable)
            {
                if (aStacks - m_Stacks >= 0)
                {
                    m_Stacks -= aStacks;
                    return aStacks;
                }
            }
            return 0;
        }

        /// <summary>
        /// Accessor to the name of the item
        /// </summary>
        public string itemName
        {
            get { return m_ItemName; }
            set { m_ItemName = value; name = value; }
        }
        public string description
        {
            get { return m_Description;}
            set { m_Description = value; }
        }
        /// <summary>
        /// Accessor to the unique id of the item
        /// </summary>
        public string uniqueID
        {
            get { return m_UniqueID; }
            set { m_UniqueID = value; }
        }
        /// <summary>
        /// Accessor to the type of item this is.
        /// </summary>
        public ItemFlags itemType
        {
            get { return m_ItemFlags; }
            set { m_ItemFlags = value; }
        }
        /// <summary>
        /// Accessor to the slot this item belongs to
        /// </summary>
        public ItemSlot itemSlot
        {
            get { return m_ItemSlot; }
            set { m_ItemSlot = value; }
        }
        /// <summary>
        /// Accessor to the cost of the item.
        /// </summary>
        public int purchaseCost
        {
            get { return m_PurchaseCost; }
            set { m_PurchaseCost = value; }
        }
        public int sellCost
        {
            get { return m_SellCost; }
            set { m_SellCost = value; }
        }
        public byte stacks
        {
            get { return m_Stacks; }
            set { m_Stacks = value; }
        }
        public byte maxStacks
        {
            get { return m_MaxStacks; }
            set { m_MaxStacks = value; }
        }
        public bool questItem
        {
            get { return (m_ItemFlags & ItemFlags.Quest) == ItemFlags.Quest; }
            set { if (value == true) { m_ItemFlags |= ItemFlags.Quest; } else { m_ItemFlags &= ~ItemFlags.Quest; } }
        }
        public bool stackable
        {
            get { return (m_ItemFlags & ItemFlags.Stackable) == ItemFlags.Stackable; }
            set { if (value == true) { m_ItemFlags |= ItemFlags.Stackable; } else { m_ItemFlags &= ~ItemFlags.Stackable; } }
        }
        public bool useable
        {
            get { return (m_ItemFlags & ItemFlags.Useable) == ItemFlags.Useable; }
            set { if (value == true) { m_ItemFlags |= ItemFlags.Useable; } else { m_ItemFlags &= ~ItemFlags.Useable; } }
        }
        public bool equippable
        {
            get { return (m_ItemFlags & ItemFlags.Equippable) == ItemFlags.Equippable; }
            set { if (value == true) { m_ItemFlags |= ItemFlags.Equippable; } else { m_ItemFlags &= ~ItemFlags.Equippable; } }
        }
        public bool consumeable
        {
            get { return (m_ItemFlags & ItemFlags.Consumeable) == ItemFlags.Consumeable; }
            set { if (value == true) { m_ItemFlags |= ItemFlags.Consumeable; } else { m_ItemFlags &= ~ItemFlags.Consumeable; } }
        }
        public ItemQuality itemQuality
        {
            get { return m_ItemQuality; }
            set { m_ItemQuality = value; }
        }
        public short durability
        {
            get { return m_Durability; }
            set { m_Durability = value; }
        }
        public short maxDurability
        {
            get { return m_MaxDurability;}
            set { m_MaxDurability = value; }
        }

        /// <summary>
        /// Accessor to the owner of the item within game.
        /// </summary>
        public Unit owner
        {
            get { return m_Inventory != null ? m_Inventory.owner : null; }
        }
        public UnitInventory inventory
        {
            get { return m_Inventory; }
            set { m_Inventory = value; }
        }
    }
}