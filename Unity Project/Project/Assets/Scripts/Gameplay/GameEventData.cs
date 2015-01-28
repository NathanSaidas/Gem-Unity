#region CHANGE LOG
/* December, 11, 2014 - Nathan Hanlan - Added class GameEventData
 * 
 */
#endregion
namespace Gem
{
    public class GameEventData
    {
        private float m_Time = 0.0f;
        private GameEventType m_Type = GameEventType.None;
        private GameEventID m_ID = GameEventID.None;
        private object m_Sender = null;
        private object m_Triggering = null;
        private object[] m_ObjectParams = null;

        public GameEventData()
        {

        }
        public GameEventData(float aTime, GameEventType aType, GameEventID aID, object aSender, object aTriggering, object[] aObjectParams)
        {
            m_Time = aTime;
            m_Type = aType;
            m_ID = aID;
            m_Sender = aSender;
            m_Triggering = aTriggering;
            m_ObjectParams = aObjectParams;
        }


        public T SenderAs<T>() where T : class
        {
            return m_Sender as T;
        }
        public T TriggeringAs<T>() where T : class
        {
            return m_Triggering as T;
        }


        public float time
        {
            get { return m_Time; }
        }
        public GameEventType type
        {
            get { return m_Type; }
        }
        public GameEventID id
        {
            get { return m_ID; }
        }
        public object sender
        {
            get { return m_Sender; }
        }
        public object triggering
        {
            get { return m_Triggering; }
        }
        public object[] objectParams
        {
            get { return m_ObjectParams; }
        }
    }
}