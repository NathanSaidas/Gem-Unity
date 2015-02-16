// -- System
using System;
using System.Collections.Generic;
// -- Unity
using UnityEngine;
#region CHANGE LOG
/*  December  7 2014 - Nathan Hanlan - Added Class
 *  December  9 2014 - Nathan Hanlan - OnDrawConsole is now getting called by OnGUI appropriately.
 *  January  29 2015 - Nathan Hanlan - Added Exception Logging and Singleton MultipleInstance logging.
 *  February 12 2015 - Nathan Hanlan - Added comments to everything. Added LogVerbosity support to Log methods.
 *  February 12 2015 - Nathan Hanlan - Fixed bug where the Keycode for show/display of the console was not implemented.
 *  February 12 2015 - Nathan Hanlan - Added configurable keycode for repeating the last message in the console.
 */
#endregion

namespace Gem
{
    /// <summary>
    /// This is a Debug Utils class that provides a in-game console. The console can take user commands and the game can execute based off them.
    /// The console can also be used for in game visual logging.
    /// 
    /// Debug Utils also wraps unity Debug code in compile directives as well as verbosity conditions.
    /// 
    /// How To Use:
    /// 
    ///     Attach this component to a game object. Then call the static methods from it.
    /// </summary>
    public class DebugUtils : MonoBehaviour
    {
        #region CONSTANTS
        /// <summary>
        /// The prefix put on to Console Logging
        /// </summary>
        private const string CONSOLE_LOG = "[Log]";
        /// <summary>
        /// The prefix put on to Warning Logging
        /// </summary>
        private const string CONSOLE_WARNING = "[Warning]:";
        /// <summary>
        /// The prefix put on to Error Logging
        /// </summary>
        private const string CONSOLE_ERROR = "[Error]:";
        /// <summary>
        /// The color of the text for logging.
        /// </summary>
        private static readonly Color CONSOLE_LOG_COLOR = Color.white;
        /// <summary>
        /// The color of the text for logging warnings.
        /// </summary>
        private static readonly Color CONSOLE_WARNING_COLOR = Color.yellow;
        /// <summary>
        /// The color of the text for logging errors.
        /// </summary>
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
                    persistent = new GameObject(Constants.GAME_OBJECT_DEBUG);
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
        /// <param name="aInstance">The instance attempting to claim ownership.</param>
        /// <returns>Returns false if the instance was already owned.</returns>
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
        /// <param name="aInstance">The instance trying to unclaim ownership.</param>
        private static void DestroyInstance(DebugUtils aInstance)
        {
            if(s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        //////////////////////////////////////////////////////////////////
        ///////////            Serialized Fields               ///////////
        //////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// How many messages should the console keep track of.
        /// </summary>
        [SerializeField]
        private int m_ConsoleLogLength = 30;
        /// <summary>
        /// What level of logging is available.
        /// If a debug log method is called and the verbosity specified is higher level than whats being used. That debug log will be ignored.
        /// </summary>
        [SerializeField]
        private LogVerbosity m_ConsoleLogVerbosity = LogVerbosity.LevelThree;
        /// <summary>
        /// The area the console is to be displayed.
        /// The position of the console is always locked.
        /// </summary>
        [SerializeField]
        private Rect m_ConsoleArea = new Rect(0.0f, 0.0f, 400.0f, 100.0f);
        /// <summary>
        /// The keycode to open up the console.
        /// </summary>
        [SerializeField]
        private KeyCode m_ShowConsoleKey = KeyCode.F3;
        /// <summary>
        /// The keycode to show the last message in the console.
        /// </summary>
        [SerializeField]
        private KeyCode m_LastMessageKey = KeyCode.F1;


        //////////////////////////////////////////////////////////////////
        ///////////          Non Serialized Fields             ///////////
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// Whether or not the console is being shown.
        /// </summary>
        private bool m_ShowConsole = false;
        /// <summary>
        /// The current string entered in the console.
        /// </summary>
        private string m_ConsoleString = string.Empty;
        /// <summary>
        /// A list of console messages made.
        /// </summary>
        private Queue<ConsoleMessage> m_ConsoleMessages = new Queue<ConsoleMessage>();
        /// <summary>
        /// The current scroll position of the console.
        /// </summary>
        private Vector2 m_ConsoleScroll = Vector2.zero;
        /// <summary>
        /// The most recent command logged in the console.
        /// </summary>
        private string m_RecentCommand = string.Empty;
        
        /// <summary>
        /// A processor interface to allow external processing of console input.
        /// </summary>
        private ICommandProcessor m_Processor = null;
        /// <summary>
        /// Whether or not an event to show the console was raised.
        /// </summary>
        private bool m_ShowConsoleEventRaised = false;
        /// <summary>
        /// Use Unity's Awake message to set the instance. This will allow code-execution that depends on the DebugUtils to reference it in the Start method.
        /// </summary>
        private void Awake()
        {
            if(!SetInstance(this))
            {
                Utilities.Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// Use Unity's OnDestroy message to remove this reference from the singleton if its set.
        /// </summary>
        private void OnDestroy()
        {
            DestroyInstance(this);
        }

        /// <summary>
        /// On every update check for input from the console key to see if the console is shown.
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(m_ShowConsoleKey))
            {
                m_ShowConsoleEventRaised = true;
            }

            if(m_ShowConsoleEventRaised)
            {
                m_ShowConsole = !m_ShowConsole;
                if (m_ShowConsole == true)
                {
                    Screen.lockCursor = false;
                }
                m_ShowConsoleEventRaised = false;
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
        /// <summary>
        /// Returns the current Log Verbosity.
        /// </summary>
        /// <returns>Returns LogVerbosity.LevelOne if the singleton instance was null.</returns>
        public static LogVerbosity GetLogVerbosity()
        {
            return instance == null ? LogVerbosity.LevelOne : instance.m_ConsoleLogVerbosity;
        }
        /// <summary>
        /// Log out a regular debug message using an error code.
        /// The error code uses a lookup table to get the proper error message associated with the string.
        /// LogVerbosity is Level One by default.
        /// </summary>
        /// <param name="aCode">The error code to look up.</param>
        public static void Log(ErrorCode aCode)
        {
            Log(DebugConstants.GetError(aCode));
        }
        /// <summary>
        /// Logs out a regular debug message using an error code.
        /// The error code uses a lookup table to get the proper error message associated with the string.
        /// </summary>
        /// <param name="aCode">The error code to look up.</param>
        /// <param name="aVerbsoity">What level of debugging should this log be.</param>
        public static void Log(ErrorCode aCode, LogVerbosity aVerbsoity)
        {
            Log(DebugConstants.GetError(aCode), aVerbsoity);
        }
        /// <summary>
        /// Logs out a regular debug message using an object's ToString method
        /// LogVerbosity is Level One by default.
        /// </summary>
        /// <param name="aMessage">The message to be logged.</param>
        public static void Log(object aMessage)
        {
            Log(aMessage, LogVerbosity.LevelOne);
        }
        /// <summary>
        /// Logs out a regular debug message using an object's ToString method.
        /// </summary>
        /// <param name="aMessage">The message to be logged.</param>
        /// <param name="aVerbosity">The verbosity to log it at.</param>
        public static void Log(object aMessage, LogVerbosity aVerbosity)
        {
            if(aVerbosity > GetLogVerbosity())
            {
                return;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.Log(aMessage.ToString());
#else
            instance.ConsoleLog(aMessage.ToString());
#endif
        }
        /// <summary>
        /// Logs out a warning using the error code. 
        /// The error code is parsed using a string lookup table which contains readable information on the error.
        /// LogVerbosity is Level One by default.
        /// </summary>
        /// <param name="aCode">The error code to log.</param>
        public static void LogWarning(ErrorCode aCode)
        {
            LogWarning(DebugConstants.GetError(aCode));
        }
        /// <summary>
        /// Logs out a warning using the error code.
        /// The error code is parsed using a string lookup table which contains readable information on the error.
        /// </summary>
        /// <param name="aCode">The error code to log.</param>
        /// <param name="aVerbosity">The verbosity level to log at.</param>
        public static void LogWarning(ErrorCode aCode, LogVerbosity aVerbosity)
        {
            LogWarning(DebugConstants.GetError(aCode), aVerbosity);
        }
        /// <summary>
        /// Logs out a warning using an object's ToString method.
        /// LogVerbosity is Level One by default.
        /// </summary>
        /// <param name="aMessage">The message to log</param>
        public static void LogWarning(object aMessage)
        {
            LogWarning(aMessage, LogVerbosity.LevelOne);
        }
        /// <summary>
        /// Logs out a warning using an object's ToString method.
        /// LogVerbosity is Level One by default.
        /// </summary>
        /// <param name="aMessage">The message to log.</param>
        /// <param name="aVerbosity">The verbosity level to log at.</param>
        public static void LogWarning(object aMessage, LogVerbosity aVerbosity)
        {
            if(aVerbosity > GetLogVerbosity())
            {
                return;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(aMessage.ToString());
#else
            instance.ConsoleLogWarning(aMessage.ToString());
#endif
        }
        /// <summary>
        /// Logs out a error message using an error code.
        /// The error code is parsed using a string lookup table which contains readable information on the error.
        /// LogVerbosity is Level One by default.
        /// </summary>
        /// <param name="aCode">The code to log out</param>
        public static void LogError(ErrorCode aCode)
        {
            LogError(DebugConstants.GetError(aCode));
        }
        /// <summary>
        /// Logs out a error message using an error code.
        /// The error code is parsed using a string lookup table which contains readable information on the error.
        /// </summary>
        /// <param name="aCode">The error code to log out.</param>
        /// <param name="aVerbosity">The verbosity of the log</param>
        public static void LogError(ErrorCode aCode, LogVerbosity aVerbosity)
        {
            LogError(DebugConstants.GetError(aCode), aVerbosity);
        }
        /// <summary>
        /// Logs out an error message using an object's ToString method
        /// LogVerbosity is Level One by default.
        /// </summary>
        /// <param name="aMessage">The message to log out.</param>
        public static void LogError(object aMessage)
        {
            LogError(aMessage, LogVerbosity.LevelOne);
        }
        /// <summary>
        /// Logs out an error message using an object's ToString method
        /// </summary>
        /// <param name="aMessage">The message to log out</param>
        /// <param name="aVerbosity">The level of verbosity to log out at.</param>
        public static void LogError(object aMessage, LogVerbosity aVerbosity)
        {
            if(aVerbosity > GetLogVerbosity())
            {
                return;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(aMessage.ToString());
#else
            instance.ConsoleLogError(aMessage.ToString());
#endif
        }

        /// <summary>
        /// Logs out a Exception.
        /// </summary>
        /// <param name="aException"></param>
        public static void LogException(Exception aException)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(aException);
#endif
        }

        /// <summary>
        /// Logs out a MultipleInstance error. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void MultipleInstances<T>()
        {
            LogError(DebugConstants.GetError(ErrorCode.SINGLETON_MULTIPLE_INSTANCE) + typeof(T).Name);
        }
        #endregion

        /// <summary>
        /// Clears all the messages from the console.
        /// </summary>
        public static void ConsoleClear()
        {
            if (instance != null)
            {
                instance.m_ConsoleMessages.Clear();
            }
        }

        /// <summary>
        /// Draws the console.
        /// </summary>
        void OnDrawConsole()
        {
            ///Setup OnGUI state
            m_ConsoleArea.x = 0.0f;
            m_ConsoleArea.y = Screen.height - m_ConsoleArea.height;
            GUILayout.BeginArea(m_ConsoleArea);
            m_ConsoleScroll = GUILayout.BeginScrollView(m_ConsoleScroll);

            ///Iterate through all the console message and display them.
            IEnumerator<ConsoleMessage> messageIter = m_ConsoleMessages.GetEnumerator();
            while(messageIter.MoveNext())
            {
                ConsoleMessage msg = messageIter.Current;
                if(string.IsNullOrEmpty(msg.message))
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
            ///Reset the GUI State

            ///Request User String
            m_ConsoleString = GUILayout.TextField(m_ConsoleString, GUILayout.Width(m_ConsoleArea.width * 0.60f));
            bool enterClicked = false;
            bool showHideConsolePressed = false;

            ///Check unity events.
            if (Event.current != null && Event.current.isKey)
            {
                KeyCode currentKey = Event.current.keyCode;
                if(currentKey == KeyCode.Return || currentKey == KeyCode.KeypadEnter)
                {
                    enterClicked = true;
                }
                else if(currentKey == m_ShowConsoleKey)
                {
                    showHideConsolePressed = true;
                }
                else if(currentKey == m_LastMessageKey)
                {
                    if (m_ConsoleString.Length == 0)
                    {
                        m_ConsoleString = m_RecentCommand;
                    }
                }
            }
            ///Check if the user hit send.
            bool send = (enterClicked || GUILayout.Button(Strings.SEND)) && m_ConsoleString.Length != 0;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (showHideConsolePressed == true)
            {
                m_ShowConsoleEventRaised = true;
                return;
            }

            if(send)
            {
                ///Create a message
                AddMessage(m_ConsoleString);

                ///Parse the words
                List<string> lowerWords = Utilities.ParseToWords(m_ConsoleString, true);
                List<string> words = Utilities.ParseToWords(m_ConsoleString, false);

                if(words.Count == 0)
                {
                    m_ConsoleString = string.Empty;
                    return;
                }

                ///Tell the processor to process the words.
                if(m_Processor != null)
                {
                    m_Processor.Process(words,lowerWords);
                }

                
                m_RecentCommand = m_ConsoleString;
                m_ConsoleString = string.Empty;
            }
        }

        #region CONSOLE
        /// <summary>
        /// Log a message to the in game console using the object ToString method. Default LogLevel is Log
        /// </summary>
        /// <param name="aMessage">The message to log</param>
        private void ConsoleLog(object aMessage)
        {
            if(aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString()));
            }
        }
        /// <summary>
        /// Log a message to the in game console using the object ToString method. Default LogLevel is Warning
        /// </summary>
        /// <param name="aMessage">The message to log.</param>
        private void ConsoleLogWarning(object aMessage)
        {
            if (aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString(),LogLevel.Warning));
            }
        }

        /// <summary>
        /// Log a message to the in game console using the object ToString method.  Default LogLevel is error.
        /// </summary>
        /// <param name="aMessage">The message to log.</param>
        private void ConsoleLogError(object aMessage)
        {
            if (aMessage != null)
            {
                AddMessage(new ConsoleMessage(aMessage.ToString(),LogLevel.Error));
            }
        }
        
        /// <summary>
        /// Adds a message to the console.
        /// </summary>
        /// <param name="aMessage"></param>
        private void AddMessage(ConsoleMessage aMessage)
        {
            if(m_ConsoleMessages.Count == m_ConsoleLogLength)
            {
                m_ConsoleMessages.Dequeue();
            }
            m_ConsoleMessages.Enqueue(aMessage);
        }
        /// <summary>
        /// Adds a message to the console. Uses the LogLevel user by default.
        /// </summary>
        /// <param name="aMessage"></param>
        private void AddMessage(string aMessage)
        {
            if(aMessage.Length != 0)
            {
                AddMessage(new ConsoleMessage(aMessage, LogLevel.User));
            }
        }
        #endregion

        /// <summary>
        /// How many messages the console should keep track of.
        /// </summary>
        public int consoleLogLength
        {
            get { return m_ConsoleLogLength; }
            set { m_ConsoleLogLength = value; }
        }
        /// <summary>
        /// The width / height of the console. x,y are overwritten.
        /// </summary>
        public Rect consoleArea
        {
            get { return m_ConsoleArea; }
            set { m_ConsoleArea = value; }
        }
        /// <summary>
        /// The keycode to show/hide the console.
        /// </summary>
        public KeyCode showConsoleKey
        {
            get { return m_ShowConsoleKey; }
            set { m_ShowConsoleKey = value; }
        }
        /// <summary>
        /// The keycode to present the last message on the console.
        /// </summary>
        public KeyCode lastMessageKey
        {
            get { return m_LastMessageKey; }
            set { m_LastMessageKey = value; }
        }
        /// <summary>
        /// The current string entered in the console.
        /// </summary>
        public string consoleMessage
        {
            get { return m_ConsoleString; }
        }
        /// <summary>
        /// Get / Set the processor of the console.
        /// </summary>
        public ICommandProcessor processor
        {
            get { return m_Processor; }
            set { m_Processor = value; }
        }


    }
}