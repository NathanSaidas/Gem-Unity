using UnityEngine;
using System.Linq;
using System.Collections.Generic;

#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added Class
 * 
 */
#endregion
namespace Gem
{
    /// <summary>
    /// Represents a inventory of items a unit has. This is not what the unit has equipped...
    /// </summary>
    public class UnitInventory : MonoBehaviour
    {
        [SerializeField]
        private List<Item> m_Items = new List<Item>();

        [SerializeField]
        private int m_MaxItemCount = 64;

        [SerializeField]
        private Unit m_Owner = null;

        private void Start()
        {
            SetMaxItems(m_MaxItemCount);
        }


        public void SetMaxItems(int aMaxItems)
        {
            m_MaxItemCount = aMaxItems;
            for(int i = 0; i < m_MaxItemCount; i++)
            {
                m_Items.Add(null);
            }
        }

        /// <summary>
        /// Returns true if any elements in the item array have the same name
        /// </summary>
        /// <param name="aItemName"></param>
        /// <returns></returns>
        public bool Exists(string aItemName)
        {
            if(m_Items == null)
            {
                return false;
            }
            return m_Items.Any(Element => Element.itemName == aItemName);
        }
        /// <summary>
        /// Returns true if the number of elements that in the array have the same name.
        /// </summary>
        /// <param name="aItemName"></param>
        /// <param name="aQuantity"></param>
        /// <returns></returns>
        public bool Exists(string aItemName, int aQuantity)
        {
            if(m_Items == null)
            {
                return false;
            }
            return aQuantity == m_Items.Count(Element => Element.itemName == aItemName);
        }
        /// <summary>
        /// Searches for a free spot on the array. (Front to back) and proceeds to to add an item.
        /// Sets the items stacks to 1
        /// </summary>
        /// <param name="aItemName"></param>
        /// <returns>Returns true if the item was succesfully added to the array.</returns>
        public bool AddItem(string aItemName)
        {
            int index = -1;
            for(int i = 0; i < m_Items.Count; i++)
            {
                if(m_Items[i] == null)
                {
                    index = i;
                    break;
                }
            }
            if(index != -1)
            {
                Item item = ItemDatabase.QueryItem(aItemName);
                m_Items[index] = item;
                if(item != null)
                {
                    item.inventory = this;
                    item.stacks = 1;
                    return true;
                }
                
            }
            return false;
        }
        /// <summary>
        /// Adds an item into the array and sets the specified stacks. 
        /// If there was overflow and the item is stackable this is called again and again until all stacks of the item have been added
        /// </summary>
        /// <param name="aItemName"></param>
        /// <param name="aQuantity"></param>
        /// <returns></returns>
        public bool AddItem(string aItemName, byte aStacks)
        {
            int index = -1;
            for(int i = 0; i < m_Items.Count; i++)
            {
                if(m_Items[i] == null)
                {
                    index = i;
                    break;
                }
            }
            if(index != -1)
            {
                Item item = ItemDatabase.QueryItem(aItemName);
                m_Items[index] = item;
                if(item != null)
                {
                    item.inventory = this;
                    item.stacks = 1;
                    byte overflow = item.AddStack(aStacks);
                    if(overflow != 0 && item.stackable == true)
                    {
                        return AddItem(aItemName, overflow);
                    }
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Finds the first item in the array
        /// </summary>
        /// <param name="aItemName"></param>
        /// <returns></returns>
        public Item GetItem(string aItemName)
        {
            if(m_Items == null)
            {
                return null;
            }
            return m_Items.FirstOrDefault<Item>(Element => Element.itemName == aItemName);
        }
        /// <summary>
        /// Finds 
        /// </summary>
        /// <param name="aItemName"></param>
        /// <returns></returns>
        public IEnumerable<Item> GetAllItem(string aItemName)
        {
            if (m_Items == null)
            {
                return null;
            }
            return m_Items.Where<Item>(Element => Element.itemName == aItemName);
        }
        /// <summary>
        /// Removes an item from the inventory. (Or removes a single stack if the item is stackable).
        /// Removes an item from the inventory when the stacks are 0
        /// </summary>
        /// <param name="aItemName"></param>
        /// <returns>The amount of stacks removed</returns>
        public byte RemoveItem(string aItemName)
        {
            for(int i = 0; i < m_Items.Count; i++)
            {
                if(m_Items[i] != null && m_Items[i].itemName == aItemName)
                {
                    if(m_Items[i].stackable)
                    {
                        byte stacksRemoved = m_Items[i].RemoveStack(1);
                        if(m_Items[i].stacks == 0)
                        {
                            m_Items[i] = null;
                        }
                        return stacksRemoved;
                    }
                    else
                    {
                        m_Items[i] = null;
                        return 1;
                    }
                }
            }
            return 0;
        }

        public bool RemoveItem(Item aItem)
        {
            for(int i = 0; i < m_Items.Count; i++)
            {
                if(m_Items[i] == aItem)
                {
                    m_Items[i] = null;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Removes an item from the inventory. (Or removes a given stack amount if the item is stackable).
        /// Removes an item from the inventory when the stacks are 0 
        /// </summary>
        /// <param name="aItemName"></param>
        /// <param name="aQuantity"></param>
        /// <returns></returns>
        public byte RemoveItem(string aItemName, byte aQuantity)
        {
            for (int i = 0; i < m_Items.Count; i++)
            {
                if (m_Items[i] != null && m_Items[i].itemName == aItemName)
                {
                    if (m_Items[i].stackable)
                    {
                        byte stacksRemoved = m_Items[i].RemoveStack(aQuantity);
                        if (m_Items[i].stacks == 0)
                        {
                            m_Items[i] = null;
                        }
                        return stacksRemoved;
                    }
                    else
                    {
                        m_Items[i] = null;
                        return 1;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Determines if the inventory is full or not.
        /// </summary>
        public bool isFull
        {
            get { return space == 0; }
        }
        /// <summary>
        /// Determines how much space is in the inventory.
        /// </summary>
        public int space
        {
            get { return m_Items == null ? 0 : m_Items.Count(Element => Element == null); }
        }
        public int maxItemCount
        {
            get { return m_MaxItemCount; }
        }

        public Item[] rawInventory
        {
            get { return m_Items.ToArray(); }
        }
        public Unit owner
        {
            get { return m_Owner; }
        }
    }
}