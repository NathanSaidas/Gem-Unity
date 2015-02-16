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
        public class YieldNone : CoroutineYield
        {

            public override YieldInstruction Yield()
            {
                return null;
            }
            public override void PostYield()
            {

            }
            public override bool IsDone()
            {
                return true;
            }
        }
    }
}
