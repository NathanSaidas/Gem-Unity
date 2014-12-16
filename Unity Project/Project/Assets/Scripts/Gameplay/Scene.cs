using UnityEngine;
using System;

#region CHANGE LOG
/* December, 11, 2014 - Nathan Hanlan - Added Scene class
 * 
 */
#endregion

namespace Gem
{
    [Serializable]
    public class Scene
    {
        [SerializeField]
        private string m_Name = string.Empty;
        [SerializeField]
        private int m_Index = 0;
        [SerializeField]
        private bool m_MenuScene = false;
        [SerializeField]
        private bool m_GameScene = false;
        [SerializeField]
        private string m_FullName = string.Empty;

        public string name
        {
            get { return m_Name; }
        }
        public int index
        {
            get { return m_Index; }
        }
        public bool menuScene
        {
            get { return m_MenuScene; }
        }
        public bool gameScene
        {
            get { return m_GameScene; }
        }
        public string fullName
        {
            get { return m_FullName; }
        }
    }
}