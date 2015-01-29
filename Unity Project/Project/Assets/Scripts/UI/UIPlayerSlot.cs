using UnityEngine;
using UnityEngine.UI;

namespace Gem
{
    public class UIPlayerSlot : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo m_Player = null;
        [SerializeField]
        private Team m_Team = new Team();
        [SerializeField]
        private int m_Index = 0;
        [SerializeField]
        private Text m_Text = null;
        [SerializeField]
        private Button m_Button = null;

        
        public PlayerInfo player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }
        public Team team
        {
            get { return m_Team; }
        }
        public int index
        {
            get { return m_Index; }
        }
        public Text text
        {
            get { return m_Text; }
        }
        public Button button
        {
            get { return m_Button; }
        }
    }

}

