using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
#if UNITY_WEBPLAYER

#else    
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif


#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan, Added the ItemDatabase Class
 * 
 */
#endregion

namespace Gem
{


    public class ItemDatabase : MonoBehaviour
    {
        #region SINGLETON
        private static ItemDatabase s_Instance = null;
        private static ItemDatabase instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        /// <summary>
        /// Attempts to find a persistent game object in the scene. Does not create objects while not in play mode.
        /// </summary>
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
                s_Instance = persistent.GetComponent<ItemDatabase>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<ItemDatabase>();
                }
            }
        }
        /// <summary>
        /// Claim ownership of the singleton instance.
        /// </summary>
        /// <param name="aInstance"></param>
        /// <returns></returns>
        private static bool SetInstance(ItemDatabase aInstance)
        {
            if (s_Instance != null && s_Instance != aInstance)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }
        /// <summary>
        /// Remove ownership from singleton instance.
        /// </summary>
        /// <param name="aInstance"></param>
        private static void DestroyInstance(ItemDatabase aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        private const int DATA_BASE_SAVE_VERSION = 3;
        [SerializeField]
        private ItemDatabaseInfo m_DatabaseInfo = new ItemDatabaseInfo();
        /// <summary>
        /// A collection of items held within the data base.
        /// Rule: Items must have unique ID's
        /// Notes: This collection may contain multiple of the name of item.
        /// </summary>
        private HashSet<Item> m_Items = new HashSet<Item>();
        /// <summary>
        /// Determines whether or not the database is saving or loading.
        /// </summary>
        private bool m_IsSavingOrLoading = false;

        private void Start()
        {
            if(!SetInstance(this))
            {
                Utilities.Destroy(this);
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
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Load("ItemDatabase", SaveFormat.PC);
            }
        }

        /// <summary>
        /// Adds an item to the data base
        /// </summary>
        /// <param name="aItemName"></param>
        public static void NewItem(string aItemName)
        {
            ItemDatabase database = instance;
            if(database == null)
            {
                return;
            }
            Item item = Item.CreateInstance<Item>();
            item.itemName = aItemName;
            item.uniqueID = Guid.NewGuid().ToString();
            database.m_Items.Add(item);
        }
        /// <summary>
        /// Adds an item to the data base
        /// </summary>
        /// <param name="aItemName"></param>
        /// <param name="aType"></param>
        /// <param name="aSlot"></param>
        public static void NewItem(string aItemName, ItemFlags aType, ItemSlot aSlot)
        {
            ItemDatabase database = instance;
            if(database == null)
            {
                return;
            }
            Item item = Item.CreateInstance<Item>();
            item.itemName = aItemName;
            item.itemSlot = aSlot;
            item.itemType = aType;
            item.uniqueID = Guid.NewGuid().ToString();
            database.m_Items.Add(item);
        }
        /// <summary>
        /// Removes an item from the data base
        /// </summary>
        /// <param name="aUniqueID"></param>
        public static void DeleteItem(string aUniqueID)
        {
            ItemDatabase database = instance;
            if(database == null)
            {
                return;
            }
            char[] bytes = aUniqueID.ToCharArray();
            if(bytes == null || bytes.Length != 16)
            {
                return;
            }
            int result = database.m_Items.RemoveWhere(Element => Element.uniqueID == aUniqueID);
            if(result > 1)
            {
                DebugUtils.LogWarning(ErrorCode.ITEM_MULTIPLE_REMOVED_INSTANCES);
            }
        }


        /// <summary>
        /// A method for uploading data from the database
        /// </summary>
        /// <param name="aURL"></param>
        /// <param name="aForm"></param>
        /// <returns></returns>
        private IEnumerator<WWW> UploadDatabase(string aURL, WWWForm aForm)
        {
            WWW upload = new WWW(aURL, aForm);
            yield return upload;
            m_IsSavingOrLoading = false;
            if (!String.IsNullOrEmpty(upload.error))
            {
                DebugUtils.LogError(DebugConstants.GetError(ErrorCode.ITEM_UPLOAD_FAILED) + " " + upload.error);
            }
        }
        /// <summary>
        /// A method for downloading data to the database
        /// </summary>
        /// <param name="aURL"></param>
        /// <returns></returns>
        private IEnumerator<WWW> DownloadDatabase(string aURL)
        {
            WWW download = new WWW(aURL);
            yield return download;
            if (!String.IsNullOrEmpty(download.error))
            {
                DebugUtils.LogError(DebugConstants.GetError(ErrorCode.ITEM_UPLOAD_FAILED) + " " + download.error);
            }
            else
            {
                int itemCount = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(download.bytes);
                int saveVersion = (int)formatter.Deserialize(stream);
                if (saveVersion != DATA_BASE_SAVE_VERSION)
                {
                    DebugUtils.LogError(ErrorCode.ITEM_INVALID_FILE_VERSION);
                    stream.Close();
                }
                else
                {
                    //m_DatabaseInfo = (ItemDatabaseInfo)formatter.Deserialize(stream);
                    m_DatabaseInfo.OnLoad(formatter, stream);
                    itemCount = (int)formatter.Deserialize(stream);
                    for (int i = 0; i < itemCount; i++)
                    {
                        Item item = Item.CreateInstance<Item>();
                        item.OnLoad(formatter, stream);
                        m_Items.Add(item);
                    }
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// Saves the database to the filename/url using the given format.
        /// </summary>
        /// <param name="aFilename"></param>
        /// <param name="aFormat"></param>
        public static void Save(string aFilename, SaveFormat aFormat)
        {
            if(instance != null)
            {
                instance.SaveDatabase(aFilename, aFormat);
            }
        }
        /// <summary>
        /// Loads the database from the filename/url using the given format.
        /// </summary>
        /// <param name="aFilename"></param>
        /// <param name="aFormat"></param>
        public static void Load(string aFilename, SaveFormat aFormat)
        {
            if(instance != null)
            {
                instance.LoadDatabase(aFilename, aFormat);
            }
        }


        /// <summary>
        /// A method for saving the data base
        /// </summary>
        /// <param name="aFilename"></param>
        /// <param name="aFormat"></param>
        private void SaveDatabase(string aFilename, SaveFormat aFormat)
        {
            if(m_IsSavingOrLoading)
            {
                DebugUtils.LogWarning(ErrorCode.ITEM_CANNOT_SAVE_LOAD);
                return;
            }

            string filepath = GetSavePath(aFilename, aFormat);
            if(filepath == Constants.INVALID_STRING)
            {
                DebugUtils.LogError(DebugConstants.GetError(ErrorCode.ITEM_INVALID_FILE_PATH) + " path = " + filepath);
                return;
            }
            ///Clean up hashset
            m_Items.RemoveWhere(Element => Element == null);
#if UNITY_WEBPLAYER

#else
            if (aFormat == SaveFormat.PC || aFormat == SaveFormat.PC_PERSISTENT)
            {
                ///Put the database in a lock. Currently saving
                m_IsSavingOrLoading = true;
                ///Create the formatter / file neceassry for the operation
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream file = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                ///Serialize the header.
                formatter.Serialize(file, DATA_BASE_SAVE_VERSION);
                //formatter.Serialize(file, m_DatabaseInfo);
                m_DatabaseInfo.OnSave(formatter, file);
                formatter.Serialize(file, m_Items.Count);
                ///Serialize all the items.
                IEnumerator<Item> items = m_Items.GetEnumerator();
                while (items.MoveNext())
                {
                    Item item = items.Current;
                    item.OnSave(formatter, file);
                }
                file.Close();
                ///Release the database lock.
                m_IsSavingOrLoading = false;
            }
            else if(aFormat == SaveFormat.WEB || aFormat == SaveFormat.WEB_PERSISTENT)
            {
                m_IsSavingOrLoading = true;
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();

                formatter.Serialize(stream, DATA_BASE_SAVE_VERSION);
                //formatter.Serialize(stream, m_DatabaseInfo);
                m_DatabaseInfo.OnSave(formatter, stream);
                formatter.Serialize(stream, m_Items.Count);

                IEnumerator<Item> items = m_Items.GetEnumerator();
                while (items.MoveNext())
                {
                    Item item = items.Current;
                    item.OnSave(formatter, stream);
                }
                WWWForm form = new WWWForm();
                form.AddBinaryData("ItemDatabase", stream.ToArray());
                StartCoroutine(UploadDatabase(filepath, form));
                stream.Close();
            }
#endif
        }
        /// <summary>
        /// A method for loading a database
        /// </summary>
        /// <param name="aFilename"></param>
        /// <param name="aFormat"></param>
        public void LoadDatabase(string aFilename, SaveFormat aFormat)
        {
            if(m_IsSavingOrLoading)
            {
                DebugUtils.LogWarning(ErrorCode.ITEM_CANNOT_SAVE_LOAD);
                return;
            }
            string filepath = GetSavePath(aFilename, aFormat);
            if (filepath == Constants.INVALID_STRING)
            {
                DebugUtils.LogError(ErrorCode.ITEM_INVALID_FILE_PATH);
                return;
            }
            if(!File.Exists(filepath))
            {
                DebugUtils.LogError(ErrorCode.ITEM_INVALID_FILE_PATH);
                return;
            }
            m_Items.Clear();
#if UNITY_WEBPLAYER
            
#else
            if (aFormat == SaveFormat.PC || aFormat == SaveFormat.PC_PERSISTENT)
            {
                Debug.Log("Loading file " + filepath);
                int itemCount = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                int saveVersion = (int)formatter.Deserialize(stream);
                if (saveVersion != DATA_BASE_SAVE_VERSION)
                {
                    DebugUtils.LogError(ErrorCode.ITEM_INVALID_FILE_VERSION);
                    stream.Close();
                    return;
                }
                //m_DatabaseInfo = (ItemDatabaseInfo)formatter.Deserialize(stream);
                m_DatabaseInfo.OnLoad(formatter, stream);
                itemCount = (int)formatter.Deserialize(stream);

                Debug.Log("Loading " + itemCount + " items");
                for (int i = 0; i < itemCount; i++)
                {
                    Item item = Item.CreateInstance<Item>();
                    item.OnLoad(formatter, stream);
                    m_Items.Add(item);
                }
                stream.Close();
            }
            else if (aFormat == SaveFormat.WEB || aFormat == SaveFormat.WEB_PERSISTENT)
            {
                StartCoroutine(DownloadDatabase(filepath));
            }
#endif
        }
        /// <summary>
        /// A helper method to get a save file path.
        /// </summary>
        /// <param name="aFilePath"></param>
        /// <param name="aFormat"></param>
        /// <returns></returns>
        private string GetSavePath(string aFilePath, SaveFormat aFormat)
        {
#if UNITY_WEBPLAYER

#else
            switch(aFormat)
            {
                case SaveFormat.PC:
                    return Application.dataPath + "/" + aFilePath + ".GemGDB";
                case SaveFormat.PC_PERSISTENT:
                    return Application.persistentDataPath + "/" + aFilePath + ".GemGDB";
                case SaveFormat.WEB:
                    return "file:///" + Application.dataPath + "/ItemDatabase.GemGDB";
                case SaveFormat.WEB_PERSISTENT:
                    return "file:///" + Application.persistentDataPath + "/ItemDatabase.GemGDB";
            }
            return Constants.INVALID_STRING;
        }
#endif
        /// <summary>
        /// Retrieves an copy of an item from the database
        /// </summary>
        /// <param name="aItemName"></param>
        /// <returns></returns>
        public static Item QueryItem(string aItemName)
        {
            ItemDatabase database = instance;
            if(database == null)
            {
                return null;
            }
            Item item = database.m_Items.FirstOrDefault<Item>(Element => Element.itemName == aItemName);
            if(item != null)
            {
                return Instantiate(item) as Item;
            }
            return null;
        }
        /// <summary>
        /// Retrieves an instance or a copy of an item from the database
        /// </summary>
        /// <param name="aItemName"></param>
        /// <param name="aInstance"></param>
        /// <returns></returns>
        public static Item QueryItem(string aItemName, bool aInstance)
        {
            ItemDatabase database = instance;
            if(database == null)
            {
                return null;
            }
            if(aInstance)
            {
                return QueryItem(aItemName);
            }
            return database.m_Items.FirstOrDefault<Item>(Element => Element.itemName == aItemName);
        }
        /// <summary>
        /// Retrieves a collection of instances of all the items with the given name
        /// </summary>
        /// <param name="aItemName"></param>
        /// <returns></returns>
        public static IEnumerable<Item> QueryItems(string aItemName)
        {
            if(instance == null)
            {
                return null;
            }
            return instance.m_Items.Where<Item>(Element => Element.itemName == aItemName);
        }
        /// <summary>
        /// Retrieves a copy of item from the database
        /// </summary>
        /// <param name="aID"></param>
        /// <returns></returns>
        public static Item QueryItem(Guid aID)
        {
            if(instance == null)
            {
                return null;
            }
            string id = aID.ToString();
            Item item = instance.m_Items.FirstOrDefault<Item>(Element => Element.uniqueID == id);
            if(item != null)
            {
                return Instantiate(item) as Item;
            }
            return null;
        }
        /// <summary>
        /// Retrieves an instance or a copy of an item from the database.
        /// </summary>
        /// <param name="aID"></param>
        /// <param name="aInstance"></param>
        /// <returns></returns>
        public static Item QueryItem(Guid aID, bool aInstance)
        {
            ItemDatabase database = instance;
            if(database == null)
            {
                return null;
            }
            if(aInstance)
            {
                return QueryItem(aID);
            }
            string id = aID.ToString();
            return database.m_Items.FirstOrDefault<Item>(Element => Element.uniqueID == id);
        }

        /// <summary>
        /// Returns whether or not the database is saving or loading. Returns false with no instance
        /// </summary>
        public static bool isSavingOrLoading
        {
            get { return instance != null ? instance.m_IsSavingOrLoading : false; }
        }

    }


    /// <summary>
    /// All of the information about the data base. Changes to this should reflect the version number
    /// </summary>
    [Serializable]
    public class ItemDatabaseInfo : ISaveable
    {
        [SerializeField]
        private string m_DatabaseName = string.Empty;

        public void OnSave(BinaryFormatter aFormatter, Stream aStream)
        {
            aFormatter.Serialize(aStream, m_DatabaseName);
        }

        public void OnLoad(BinaryFormatter aFormatter, Stream aStream)
        {
            Debug.Log("Loading Info");
            m_DatabaseName = (string)aFormatter.Deserialize(aStream);
        }

        public string databaseName
        {
            get { return m_DatabaseName; }
            set { m_DatabaseName = value; }
        }
    }

}