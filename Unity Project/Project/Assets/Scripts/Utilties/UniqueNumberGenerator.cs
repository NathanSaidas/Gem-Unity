using UnityEngine;
using System.Collections.Generic;

#region CHANGE LOG
/* November, 7, 2014 - Nathan Hanlan - Added and implemented class.
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// Generates a unique number in an ascending sequence. 
    /// Its possible to restore values back to the unique number generator for reuse.
    /// </summary>
    public class UniqueNumberGenerator
    {
        /// <summary>
        /// A list of ints that are currently available.
        /// </summary>
        private List<int> m_FreeList = new List<int>();
        /// <summary>
        /// A list of reserved ints that cannot be taken
        /// </summary>
        private List<int> m_Reserved = new List<int>();
        /// <summary>
        /// The next integer to choose from.
        /// </summary>
        private int m_NextNumber = 0;
        /// <summary>
        /// The Largest Number generated
        /// </summary>
        private int m_LargestNumber = -1;
        
        public UniqueNumberGenerator()
        {

        }
        /// <summary>
        /// Starts the unique number generator at the specified number
        /// </summary>
        /// <param name="aStartNumber">The number to start at.</param>
        public UniqueNumberGenerator(int aStartNumber)
        {
            m_NextNumber = aStartNumber;
        }
        /// <summary>
        /// Querys the next available integer.
        /// </summary>
        /// <returns>A unique integer</returns>
        public int Get()
        {
        	//Get a unique number off the free list, cross reference check it with the reserved list
			int uniqueNumber = -1;
        	while(m_FreeList.Count > 0)
        	{
        		uniqueNumber = m_FreeList[0];
        		m_FreeList.RemoveAt(0);
        		if(!m_Reserved.Contains(uniqueNumber))
        		{
        			if(uniqueNumber > m_LargestNumber)
        			{
        				m_LargestNumber = uniqueNumber;
        			}
        			return uniqueNumber;
        		}
        	}
        	//Get a unique number by incrementing the next number value, cross reference check it with the reserved list.
        	uniqueNumber = m_NextNumber;
        	m_NextNumber ++;
        	while(m_Reserved.Contains(uniqueNumber))
        	{
        		uniqueNumber = m_NextNumber;
        		m_NextNumber++;
        	}
        	if(uniqueNumber > m_LargestNumber)
        	{
        		m_LargestNumber = uniqueNumber;
        	}
            return uniqueNumber;
        }
        /// <summary>
        /// Reserve the specified aNumber.
        /// </summary>
        /// <param name="aNumber">The number to reserve.</param>
		public bool Reserve(int aNumber)
		{	
			if(aNumber < 0)
			{
				return false;
			}
			//Check the reserved list
			if(m_Reserved.Contains(aNumber))
			{
				return false;
			}
			//Check the free list
			if(aNumber < m_NextNumber)
			{
				if(m_FreeList.Contains(aNumber))
				{
					m_FreeList.Remove(aNumber);
				}
				else
				{
					return false;
				}
			}
			if(aNumber > m_LargestNumber)
			{
				m_LargestNumber = aNumber;
			}
			m_Reserved.Add(aNumber);
			return true;
		}
        
        /// <summary>
        /// Restores a value back into the free list.
        /// </summary>
        /// <param name="aNumber">The integer to free.</param>
        public void Free(int aNumber)
        {
        	if(aNumber == m_LargestNumber)
        	{
        		m_LargestNumber = 0;
        	}
        	if(m_Reserved.Contains(aNumber))
        	{
        		m_Reserved.Remove(aNumber);
        	}
        	if(!(aNumber >= m_NextNumber))
        	{
        		m_FreeList.Add(aNumber);
        	}
        	
        	List<int>.Enumerator iter = m_Reserved.GetEnumerator();
        	while(iter.MoveNext())
        	{
        		if(iter.Current > m_LargestNumber)
        		{
        			m_LargestNumber = iter.Current;
        		}
        	}
        	
        	if(m_NextNumber > m_LargestNumber && !m_FreeList.Contains(m_NextNumber))
        	{
        		m_LargestNumber = m_NextNumber;
        	}
        }
        /// <summary>
        /// Resets the generator at index 0
        /// </summary>
        public void Reset()
        {
            Reset(0);
        }
        /// <summary>
        /// Resets the generator at the specified index
        /// </summary>
        /// <param name="aStartInt">The index to start out at.</param>
        public void Reset(int aStartInt)
        {
            m_NextNumber = 0;
            m_LargestNumber = 0;
            m_FreeList.Clear();
            m_Reserved.Clear();
        }

        /// <summary>
        /// Returns true if the number is on the free list
        /// </summary>
        /// <param name="aNumber"></param>
        /// <returns></returns>
        public bool IsFree(int aNumber)
        {
            return m_FreeList.Contains(aNumber);
        }
        /// <summary>
        /// Returns the next unique number not on the free list.
        /// </summary>
        public int next
        {
            get { return m_NextNumber; }
        }
		public int largestNumber
		{	
			get{return m_LargestNumber;}
		}
    }
}