#region CHANGE LOG
/*  February 12 2015 - Nathan Hanlan - Added file/class ActorEvent
 * 
 */
#endregion
namespace Gem
{
    public class ActorEvent
    {
        private ActorEventType m_EventType = ActorEventType.Select;
        private object m_Context = null;

        public ActorEvent(ActorEventType aEventType, object aContext)
        {
            m_EventType = aEventType;
            m_Context = aContext;
        }

        public ActorEventType eventType
        {
            get { return m_EventType; }
        }
        public object context
        {
            get { return m_Context; }
        }
    }
}