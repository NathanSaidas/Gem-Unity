using UnityEngine;
using System.Collections;

#region CHANGE LOG
/* November, 7, 2014 - Nathan Hanlan - Added Class
 * 
 */ 
#endregion

namespace Gem
{
    /// <summary>
    /// Represents the data of the active player.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {



        public PlayerIndex player
        {
            get { return PlayerIndex.None; }
        }
    }
}