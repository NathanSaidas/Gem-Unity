// -- System
using System;
using System.Collections.Generic;
// -- Unity
using UnityEngine;

#region CHANGE LOG
/*  February 12 2015 - Nathan Hanlan - Added CoroutineEx file / class
 * 
 */
#endregion

namespace Gem
{
    namespace Coroutines
    {
        public abstract class CoroutineYield
        {
            public abstract YieldInstruction Yield();
            public abstract void PostYield();
            public abstract bool IsDone();
        }
    }
}
