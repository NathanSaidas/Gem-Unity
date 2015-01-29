using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Gem
{
    public class UIMatchMaking : MonoBehaviour
    {
        [SerializeField]
        private Color[] m_ColorOptions = new Color[Constants.MAX_PLAYERS];

        [SerializeField]
        private UIPlayerSlot[] m_PlayerSlots = new UIPlayerSlot[Constants.MAX_PLAYERS];
    }

}

