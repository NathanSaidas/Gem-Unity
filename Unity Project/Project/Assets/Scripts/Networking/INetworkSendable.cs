#region CHANGE LOG
/*  January, 29, 2015 - Nathan Hanlan - Added file/interface INetworkSendable
 * 
 */
#endregion

using UnityEngine;

using System.IO;
using System.Runtime.Serialization;

namespace Gem
{
    public interface INetworkSendable
    {
        void OnSend(Stream aStream, IFormatter aFormatter);
        bool OnReceive(Stream aStream, IFormatter aFormatter);
    }
}
