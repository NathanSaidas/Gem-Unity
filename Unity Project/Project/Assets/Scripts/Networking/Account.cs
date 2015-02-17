using UnityEngine;
using System;

[Serializable]
public class Account  
{
    [SerializeField]
    private string m_Username = string.Empty;
    [SerializeField]
    private string m_Password = string.Empty;

    public string username
    {
        get { return m_Username; }
        set { m_Username = value; }
    }

    public string password
    {
        get { return m_Password; }
        set { m_Password = value; }
    }
}
