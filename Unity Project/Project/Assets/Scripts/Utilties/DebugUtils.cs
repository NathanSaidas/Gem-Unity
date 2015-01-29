using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added Class
 * December, 9, 2014 - Nathan Hanlan - OnDrawConsole is now getting called by OnGUI appropriately.
 */
#endregion

namespace Gem
{

    public class DebugUtils : MonoBehaviour
    {
        #region CONSTANTS
        private const string CONSOLE_LOG = "[Log]";
        private const string CONSOLE_WARNING = "[Warning]:";
        private const string CONSOLE_ERROR = "[Error]:";
        private static readonly Color CONSOLE_LOG_COLOR = Color.white;
        private static readonly Color CONSOLE_WARNING_COLOR = Color.yellow;
        private static readonly Color CONSOLE_ERROR_COLOR = Color.red;
        #endregion

        #region SINGLETON
        /// <summary>
        /// A singleton instance of DebugUtils
        /// </summary>
        private static DebugUtils s_Instance = null;
        /// <summary>
        /// An accessor to the DebugUtils singleton. Creates an instance if there was no instance available. (PlayMode only).
        /// </summary>
        private static DebugUtils instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        /// <summary>
        /// Attempts to find a persistent game object in the scene. Does not create objects while not in play mode.
        /// </summary>
        private static void CreateInstance()
        {
            GameObject persistent = GameObject.Find(Constants.GAME_OBJECT_PERSISTENT);
            if(persistent == null)
            {
                if(Application.isPlaying)
                {
                    persistent = new GameObject(Constants.GAME_OBJECT_PERSISTENT);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if(persistent != null)
            {
                s_Instance = persistent.GetComponent<DebugUtils>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<DebugUtils>();
                }
            }
        }
        /// <summary>
        /// Claim ownership of the singleton instance.
        /// </summary>
        /// <param name="aInstance"></param>
        /// <returns></returns>
        private static bool SetInstance(DebugUtils aInstance)
        {
            if(s_Instance != null && s_Instance != aInstance)
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
        private static void DestroyInstance(DebugUtils aInstance)
        {
            if(s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        [SerializeField]
        private int m_ConsoleLogLength = 30;
        [SerializeField]
        private Rect m_ConsoleArea = new Rect(0.0f, 0.0f, 400.0f, 100.0f);
        [SerializeField]
        private KeyCode m_ShowConsoleKey = KeyCode.F3;


        private bool m_ShowConsole = false;
        private string m_ConsoleString = string.Empty;
        private Queue<ConsoleMessage> m_ConsoleMessages = new Queue<ConsoleMessage>();
        private Vector2 m_ConsoleScroll = Vector2.zero;
        private string m_RecentCommand = string.Empty;
        
        private ICommandProcessor m_Processor = null;
        // Use this for initialization
        void Start()
        {
            if(!SetInstance(this))
            {
                Utilities.Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        void OnDestroy()
        {
            DestroyInstance(this);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(m_ShowConsoleKey))
            {
                m_ShowConsole = !m_ShowConsole;
                if(m_ShowConsole == true)
                {
                    Screen.lockCursor = false;
                }
            }
        }

        void OnGUI()
        {
            if(m_ShowConsole)
            {
                OnDrawConsole();
            }
        }

        #region LOGGING
        public static void Log(ErrorCode aCode)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(DebugConstants.GetError(aCode));
#else
            instance.ConsoleLog(DebugConstants.GetError(aCode));
#endif
        }
        public static void Log(object aMessage)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(aMessage.ToString());
#else
            instance.ConsoleLog(aMessage.ToString());
#endif
        }
        public static void LogWarning(ErrorCode aCode)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(DebugConstants.GetError(aCode));
#else
            instance.ConsoleLogWarning(DebugConstants.GetError(aCode));
#endif
        }
        public static void LogWarning(object aMessage)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(aMessage.ToString());
#else
            instance.ConsoleLogWarning(aMessage.ToString());
#endif
        }
        public static void LogError(ErrorCode aCode)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(DebugConstants.GetError(aCode));
#else
            instance.ConsoleLogError(DebugConstants.GetError(aCode));
#endif
        }
        public static void LogError(object aMessage)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(aMessage.ToString());
#else
            instance.ConsoleLogError(aMessage.ToString());
#endif
        }

        public static void MultipleInstances<T>()
        {
            LogError(DebugConstants.GetError(ErrorCode.SINGLETON_MULTIPLE_INSTANCE) + typeof(T).Name);
        }
        #endregion

        void OnDrawConsole()
        {
            m_ConsoleArea.x = 0.0f;
            m_ConsoleArea.y = Screen.height - m_ConsoleArea.height;
            GUILayout.BeginArea(m_ConsoleArea);
            m_ConsoleScroll = GUILayout.BeginScrollView(m_ConsoleScroll);
            IEnumerator<ConsoleMessage> messageIter = m_ConsoleMessages.GetEnumerator();
            while(messageIter.MoveNext())
            {
                ConsoleMessage msg = messageIter.Current;
                if(msg.message == null || msg.message.Length == 0)
                {
                    continue;
                }
                switch (msg.logLevel)
                {
                    case LogLevel.Error:
                        GUI.contentColor = CONSOLE_ERROR_COLOR;
                        GUILayout.Label(CONSOLE_ERROR + msg.message);
                        break;
                    case LogLevel.Warning:
                        GUI.contentColor = CONSOLE_WARNING_COLOR;
                        GUILayout.Label(CONSOLE_WARNING + msg.message);
                        break;
                    case LogLevel.Log:
                        GUI.contentColor = CONSOLE_LOG_COLOR;
                        GUILayout.Label(CONSOLE_LOG + msg.message);
                        break;
                    case LogLevel.User:
                        GUI.contentColor = CONSOLE_LOG_COLOR;
                        GUILayout.Label(msg.message);
                        break;
                }
            }
            GUI.contentColor = Color.white;
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();

            m_ConsoleString = GUILayout.TextField(m_ConsoleString, GUILayout.Width(m_ConsoleArea.width * 0.60f));
            bool enterClicked = false;
            bool showHideConsolePressed = false;

            if (Event.current != null && Event.current.isKey)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        enterClicked = true;
                        break;
                    case KeyCode.F3:
                        showHideConsolePressed = true;
                        break;
                    case KeyCode.F1:
                        {
                            if (m_ConsoleString.Length == 0)
                            {
                                m_ConsoleString = m_RecentCommand;
                            }
                        }
                        break;

                }
            }

            bool send = (enterClicked || GUILayout.Button(Strings.SEND)) && m_ConsoleString.Length != 0;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (showHideConsolePressed == true)
            {
                return;
            }

            if(send)
            {
                AddMessage(m_ConsoleString);

                List<string> lowerWords = Utilities.ParseToWords(m_ConsoleString, true);
                List<string> words = Utilities.ParseToWords(m_ConsoleString, false);

                if(words.Count == 0)
                {
                    m_ConsoleString = string.Empty;
                    return;
                }
                if(m_Processor != null)
                {
                    m_Processor.Process(words,lowerWords);
                }

                
                m_RecentCommand = m_ConsoleString;
                m_ConsoleString = string.Empty;
            }
        }

        #region CONSOLE
        private void ConsoleLog(object aMessage)
        {
            if(aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString()));
            }
        }
        private void ConsoleLogWarning(object aMessage)
        {
            if (aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString(),LogLevel.Warning));
            }
        }
        private void ConsoleLogError(object aMessage)
        {
            if (aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString(),LogLevel.Error));
            }
        }

        private void ConsoleClear()
        {
            m_ConsoleMessages.Clear();
        }

        private void AddMessage(ConsoleMessage aMessage)
        {
            if(m_ConsoleMessages.Count == m_ConsoleLogLength)
            {
                m_ConsoleMessages.Dequeue();
            }
            m_ConsoleMessages.Enqueue(aMessage);
        }
        private void AddMessage(string aMessage)
        {
            if(aMessage.Length != 0)
            {
                AddMessage(new ConsoleMessage(aMessage, LogLevel.User));
            }
        }
        #endregion


        public int consoleLogLength
        {
            get { return m_ConsoleLogLength; }
            set { m_ConsoleLogLength = value; }
        }
        public Rect consoleArea
        {
            get { return m_ConsoleArea; }
            set { m_ConsoleArea = value; }
        }
        public KeyCode showConsoleKey
        {
            get { return m_ShowConsoleKey; }
            set { m_ShowConsoleKey = value; }
        }
        public string consoleMessage
        {
            get { return m_ConsoleString; }
        }
        public ICommandProcessor processor
        {
            get { return m_Processor; }
            set { m_Processor = value; }
        }


    }
}