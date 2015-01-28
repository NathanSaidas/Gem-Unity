using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added enum SaveFormat and ISaveable
 * 
 */
#endregion

namespace Gem
{
    public enum SaveFormat
    {
        PC,
        PC_PERSISTENT,
        WEB,
        WEB_PERSISTENT
    }

    public interface ISaveable
    {
        void OnSave(BinaryFormatter aFormatter, Stream aStream);
        void OnLoad(BinaryFormatter aFormatter, Stream aStream);
    }
}