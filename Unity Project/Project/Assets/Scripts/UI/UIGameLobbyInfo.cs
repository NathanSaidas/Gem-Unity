using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGameLobbyInfo : MonoBehaviour 
{
    [SerializeField]
    private Text m_GameName = null;
    [SerializeField]
    private Text m_GameComment = null;
    [SerializeField]
    private Button m_GameButton = null;

    public Text gameName
    {
        get { return m_GameName; }
    }
    public Text gameComment
    {
        get { return m_GameComment; }
    }
    public Button gameButton
    {
        get { return m_GameButton; }
    }
}
