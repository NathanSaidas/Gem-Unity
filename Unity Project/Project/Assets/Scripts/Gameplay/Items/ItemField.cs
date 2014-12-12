using System;
#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan, Added Class ItemField
 * 
 */
#endregion
namespace Gem
{
    /// <summary>
    /// Contains the data for a field of an item within a database
    /// </summary>
    [Serializable]
    public struct ItemField
    {

        /// <summary>
        /// The name of the field
        /// </summary>
        private string m_FieldName;
        /// <summary>
        /// Boxed data. Accepted Types are (Int,Float,String)
        /// </summary>
        private object m_FieldData;

        /// <summary>
        /// Basic constructor for the ItemField to initalize with a name
        /// </summary>
        /// <param name="aName">The name to set the field</param>
        public ItemField(string aName)
        {
            m_FieldName = aName;
            m_FieldData = 0;
        }
        /// <summary>
        /// Sets the data with the given int.
        /// </summary>
        public void SetInt(int aData)
        {
            m_FieldData = aData;
        }
        /// <summary>
        /// Sets the data with the given float.
        /// </summary>
        public void SetFloat(float aData)
        {
            m_FieldData = aData;
        }
        /// <summary>
        /// Sets the data with the given string.
        /// </summary>
        public void SetString(string aString)
        {
            m_FieldData = aString;
        }
        /// <summary>
        /// Attempts to get the data back as a integer.
        /// </summary>
        /// <returns>Constants.INVALID_INT if invalid type</returns>
        public int GetInt()
        {
            if(m_FieldData != null && m_FieldData.GetType() == typeof(int))
            {
                return (int)m_FieldData;
            }
            return Constants.INVALID_INT;
        }
        /// <summary>
        /// Attempts to get the data back as a float.
        /// </summary>
        /// <returns>Constants.INVALID_FLOAT if invalid type</returns>
        public float GetFloat()
        {
            if (m_FieldData != null && m_FieldData.GetType() == typeof(float))
            {
                return (float)m_FieldData;
            }
            return Constants.INVALID_FLOAT;
        }
        /// <summary>
        /// Attempts to get the data back as a string.
        /// </summary>
        /// <returns>Constants.INVALID_STRING if invalid type</returns>
        public string GetString()
        {
            if (m_FieldData != null && m_FieldData.GetType() == typeof(string))
            {
                return (string)m_FieldData;
            }
            return Constants.INVALID_STRING;
        }

        /// <summary>
        /// An override to check if ones data is equal to another.
        /// </summary>
        /// <param name="aOther">The comparing ItemField</param>
        /// <returns></returns>
        public bool Equals(ItemField aOther)
        {
            return aOther.data == data;
        }
        public bool Equals(string aName)
        {
            return name == aName;
        }
        /// <summary>
        /// An accessor for the name of the field
        /// </summary>
        public string name
        {
            get { return m_FieldName; }
            set { m_FieldName = value; }
        }
        /// <summary>
        /// A readonly accessor of the data contained within the field. Consider using GetInt/GetFloat/GetString instead.
        /// </summary>
        public object data
        {
            get { return m_FieldData; }
        }

        public static readonly ItemField empty = new ItemField("NewItemField");
           
    }
}