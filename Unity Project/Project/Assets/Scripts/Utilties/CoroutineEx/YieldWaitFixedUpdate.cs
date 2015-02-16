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
        public class YieldWaitFixedUpdate : CoroutineYield
        {
            private int m_Frames = 0;
            private int m_YieldCount = 0;

            public YieldWaitFixedUpdate(int aFrames)
            {
                m_Frames = aFrames;
            }

            public override YieldInstruction Yield()
            {
                return new WaitForFixedUpdate();
            }
            public override void PostYield()
            {
                m_YieldCount++;
            }
            public override bool IsDone()
            {
                return m_YieldCount >= m_Frames;
            }
        }
    }
}
