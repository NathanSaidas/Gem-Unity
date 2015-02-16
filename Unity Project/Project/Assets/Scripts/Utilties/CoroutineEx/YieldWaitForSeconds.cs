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
        public class YieldWaitForSeconds : CoroutineYield
        {
            private float m_Seconds = 0.0f;
            private bool m_IsDone = false;

            public YieldWaitForSeconds(float aSeconds)
            {
                m_Seconds = aSeconds;
            }

            public override YieldInstruction Yield()
            {
                return new WaitForSeconds(m_Seconds);
            }

            public override void PostYield()
            {
                m_IsDone = true;
            }

            public override bool IsDone()
            {
                return m_IsDone;
            }
        }
    }
}
