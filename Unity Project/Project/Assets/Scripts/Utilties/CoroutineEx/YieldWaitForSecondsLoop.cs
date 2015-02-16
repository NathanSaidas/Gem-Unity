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
        public class YieldWaitForSecondsLoop : CoroutineYield
        {
            private float m_Seconds = 0.0f;
            private bool m_IsDone = false;
            private int m_LoopCount = 0;
            private int m_CurrentLoopCount = 0;

            public YieldWaitForSecondsLoop(float aSeconds, int aLoopCount)
            {
                m_Seconds = aSeconds;
                m_LoopCount = aLoopCount;
            }

            public override YieldInstruction Yield()
            {
                return new WaitForSeconds(m_Seconds);
            }

            public override void PostYield()
            {
                m_CurrentLoopCount++;
                if(m_CurrentLoopCount >= m_LoopCount)
                {
                    m_IsDone = true;
                }
            }

            public override bool IsDone()
            {
                return m_IsDone;
            }
        }
    }
}