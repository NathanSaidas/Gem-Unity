#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added struct ConsoleMessage and Enum LogLevel
 * 
 */ 
#endregion

namespace Gem
{
    /// <summary>
    /// Describes the king of log level the console should display.
    /// </summary>
    public enum LogLevel
    {
        Log,
        Warning,
        Error,
        User
    }

    /// <summary>
    /// Defines a message in the console.
    /// </summary>
    public struct ConsoleMessage
    {
        /// <summary>
        /// The contents of the message
        /// </summary>
        private string m_Message;
        /// <summary>
        /// The type of message it is.
        /// </summary>
        private LogLevel m_LogLevel;
        public ConsoleMessage(string aMessage)
        {
            m_Message = aMessage;
            m_LogLevel = LogLevel.Log;
        }
        public ConsoleMessage(string aMessage, LogLevel aLogLevel)
        {
            m_Message = aMessage;
            m_LogLevel = aLogLevel;
        }
        /// <summary>
        /// An accessor to the contents of the message
        /// </summary>
        public string message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
        /// <summary>
        /// An accessor to the type of message it is.
        /// </summary>
        public LogLevel logLevel
        {
            get { return m_LogLevel; }
            set { m_LogLevel = value; }
        }
    }
}