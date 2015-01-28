using UnityEngine;
using System.Collections.Generic;

namespace Gem
{
    public class MonoGameEventHandler : MonoBehaviour , IGameEventReceiver
    {
        private List<GameEventID> m_RegisteredIDs = new List<GameEventID>();
        private GameEventData m_GameEvent = null;
        #region COUNTERS
        /// Keeps track of how often an event type has been registered.
        private int m_GameEvents = 0;
        private int m_ActorEvents = 0;
        private int m_UnitEvents = 0;
        private int m_InteractiveEvents = 0;
        private int m_ItemEvents = 0;
        private int m_NetworkEvents = 0;
        #endregion

        /// <summary>
        /// Register a game event with the game event system.
        /// </summary>
        /// <param name="aGameEvent"></param>
        protected void Register(GameEventID aGameEvent)
        {
            if (m_RegisteredIDs.Contains(aGameEvent))
            {
                return;
            }
            //TODO: Implement GameEvents
            switch(aGameEvent)
            {

            }

        }

        /// <summary>
        /// Unregister a game event with the game event system.
        /// </summary>
        /// <param name="aGameEvent"></param>
        protected void Unregister(GameEventID aGameEvent)
        {
            if (!m_RegisteredIDs.Contains(aGameEvent))
            {
                return;
            }
            //TODO: Implement GameEvents
            switch(aGameEvent)
            {

            }
        }

        /// <summary>
        /// Override this to get a call from the event.
        /// Use eventData to get further info about the event.
        /// </summary>
        /// <param name="aEvent"></param>
        protected virtual void OnGameEvent(GameEventID aEvent)
        {

        }
        
        public void OnReceiveEvent(GameEventData aData)
        {
            m_GameEvent = aData;
            OnGameEvent(aData.id);
            m_GameEvent = null;
        }


        public GameEventData eventData
        {
            get { return m_GameEvent; }
        }
    }
}
