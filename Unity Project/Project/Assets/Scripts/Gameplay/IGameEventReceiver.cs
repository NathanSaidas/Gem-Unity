#region CHANGE LOG
/* December, 11, 2014 - Nathan Hanlan - Added interface IGameEventReceiver
 * 
 */
#endregion

namespace Gem
{
    public interface IGameEventReceiver
    {
        void OnReceiveEvent(GameEventData aData);
    }
}