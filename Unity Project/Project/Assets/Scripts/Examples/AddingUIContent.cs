using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Gem;

namespace Gem.Examples
{
    public class AddingUIContent : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Prefab = null;
        [SerializeField]
        private UIContentScaler m_ContentScale = null;
        [SerializeField]
        private KeyCode m_AddKey = KeyCode.Alpha1;


        private void Update()
        {
            if(Input.GetKeyDown(m_AddKey) && m_Prefab != null && m_ContentScale != null)
            {
                GameObject uiGameObject = Instantiate(m_Prefab) as GameObject;
                RectTransform rtTransform = uiGameObject.GetComponent<RectTransform>();
                m_ContentScale.AddItem(rtTransform);
                UIGameLobbyInfo lobbyInfo = uiGameObject.GetComponent<UIGameLobbyInfo>();
                lobbyInfo.gameButton.onClick.AddListener(() => OnClick(lobbyInfo.gameButton));
            }
        }

        private void OnClick(Button aButton)
        {
            Debug.Log("Button Pressed");
        }
    }

}



