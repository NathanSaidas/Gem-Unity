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
        /// <summary>
        /// This is the Base Class for Coroutines to inherit from.
        /// 
        /// -- Abstract Members --
        ///     OnExecute
        ///     OnPostExecute
        ///     
        /// -- Callbacks --
        ///     OnCoroutineFinish
        ///     OnCoroutineStop
        ///     OnCoroutinePause
        ///     OnCoroutineResume
        ///     
        /// -- Methods -- (New Features Unity doesn't support by default is the following)
        ///     Start
        ///     Stop
        ///     *Pause    
        ///     *Resume
        ///     *GetExecutionTime
        ///     *GetPauseTime
        /// 
        /// How To Use:
        /// 
        ///     Start by Initializing the Extensions with the InitializeCoroutineExtensions method. 
        ///     It expect callbacks to a function that can start coroutines. (MonoBehaviours). Once initialized
        ///     a CoroutineEx class can execute its methods and work properly.
        /// 
        /// Notes:
        ///     
        ///     This was designed to work on a frame-by-frame basis and not wait for x state to execute.
        ///     GetRoutine can be overrided but keep in the mind the implementation when overriding.
        ///     
        /// 
        /// Future:
        ///     • Exception Handling
        ///     • Nesting Coroutines
        ///     • Additional Wait Commands
        /// </summary>
        public abstract class CoroutineEx
        {
            public delegate void CoroutineExecution(IEnumerator<YieldInstruction> aRoutine);
            public delegate void CoroutineCallback(CoroutineEx aRoutine);

            static CoroutineExecution s_StartCoroutine = null;
            static CoroutineExecution s_StopCoroutine = null;

            /// <summary>
            /// Initializes the coroutine execution methods. These are typically used with a MonoBehaviour class.
            /// But any method that supports Unity StartCoroutine and StopCoroutine methods will work.
            /// Only initialize once. If the methods are changed in runtime this could cause bad states to occur 
            /// and the coroutines will not be able to stop correctly.
            /// </summary>
            /// <param name="aStartCoroutineCallback">The method for starting a Coroutine</param>
            /// <param name="aStopCoroutineCallback">The method for stopping a Coroutine</param>
            public static void InitializeCoroutineExtensions(CoroutineExecution aStartCoroutineCallback, CoroutineExecution aStopCoroutineCallback)
            {
                s_StartCoroutine = aStartCoroutineCallback;
                s_StopCoroutine = aStopCoroutineCallback;
            }

            /// <summary>
            /// If the coroutine has been started and registered by unity
            /// This will be true until the coroutine is finished.
            /// </summary>
            private bool m_IsExecuting = false;
            /// <summary>
            /// If the coroutine has paused execution. Unity will still process this but the execution will not be processed.
            /// </summary>
            private bool m_IsPaused = false;
            /// <summary>
            /// The yield instruction to use.
            /// </summary>
            private CoroutineYield m_Coroutine = null;
            private IEnumerator<YieldInstruction> m_ExecutingCoroutine = null;

            /// <summary>
            /// The time the coroutine has started/resumed execution at
            /// </summary>
            private float m_TimeAtStart = 0.0f;
            /// <summary>
            /// The time the coroutine was paused at
            /// </summary>
            private float m_TimeAtPause = 0.0f;

            /// <summary>
            /// How much time the coroutine has executed for.
            /// </summary>
            private float m_ExecutionTime = 0.0f;
            /// <summary>
            /// how much time the coroutine has paused for.
            /// </summary>
            private float m_PauseTime = 0.0f;




            // -- Coroutine Callbacks


            /// <summary>
            /// Gets called when the coroutine finishes execution
            /// </summary>
            private CoroutineCallback m_OnCoroutineFinish = null;
            /// <summary>
            /// Gets called when the coroutine pauses execution
            /// </summary>
            private CoroutineCallback m_OnCoroutinePause = null;
            /// <summary>
            /// Gets called when the coroutine resumes execution.
            /// </summary>
            private CoroutineCallback m_OnCoroutineResume = null;
            /// <summary>
            /// Gets called when the coroutine was stopped
            /// </summary>
            private CoroutineCallback m_OnCoroutineStopped = null;

            /// <summary>
            /// The constructor for the Coroutine which takes the yield used.
            /// </summary>
            /// <param name="aYield"></param>
            public CoroutineEx(CoroutineYield aYield)
                : base()
            {
                m_Coroutine = aYield;
            }

            /// <summary>
            /// The method for starting a coroutine.
            /// Coroutines cannot start without having the InitializeCoroutineExtensions executed with proper data.
            /// Coroutines cannot start while already executing
            /// Coroutines can also not start which there is no valid yield.
            /// </summary>
            public void Start()
            {
                if(s_StartCoroutine == null)
                {
                    DebugUtils.LogError(ErrorCode.COROUTINE_NOT_INITIALIZED);
                }

                if (!m_IsExecuting && m_Coroutine != null && s_StartCoroutine != null)
                {
                    m_IsExecuting = true;
                    m_IsPaused = false;
                    m_ExecutionTime = 0.0f;
                    m_PauseTime = 0.0f;
                    m_ExecutingCoroutine = GetRoutine();
                    s_StartCoroutine.Invoke(m_ExecutingCoroutine);
                }
            }
            /// <summary>
            /// The method for stopping a coroutine
            /// Coroutines cannot be stopped without having the InitializeCoroutineExtensions executed with proper data.
            /// Coroutines cannot be stopped while not running
            /// </summary>
            public void Stop()
            {
                if (s_StopCoroutine == null)
                {
                    DebugUtils.LogError(ErrorCode.COROUTINE_NOT_INITIALIZED);
                }

                if (m_IsExecuting && s_StopCoroutine != null)
                {
                    ///Revert back to default state.
                    m_ExecutionTime = 0.0f;
                    m_PauseTime = 0.0f;
                    m_TimeAtPause = 0.0f;
                    m_TimeAtStart = 0.0f;
                    m_IsExecuting = false;
                    s_StopCoroutine.Invoke(m_ExecutingCoroutine);
                    if (m_OnCoroutineStopped != null)
                    {
                        m_OnCoroutineStopped.Invoke(this);
                    }
                }
            }

            /// <summary>
            /// Pauses the Coroutines execution.
            /// </summary>
            public void Pause()
            {
                if (m_IsExecuting && !m_IsPaused)
                {
                    m_ExecutionTime += Time.time - m_TimeAtStart;

                    m_TimeAtPause = Time.time;
                    m_IsPaused = true;
                    if (m_OnCoroutinePause != null)
                    {
                        m_OnCoroutinePause.Invoke(this);
                    }
                }
            }

            /// <summary>
            /// Resumes the Coroutines execution.
            /// </summary>
            public void Resume()
            {

                if (m_IsExecuting && m_IsPaused)
                {
                    m_IsPaused = false;
                    m_PauseTime += Time.time - m_TimeAtPause;
                    m_TimeAtStart = Time.time;
                    if (m_OnCoroutineResume != null)
                    {
                        m_OnCoroutineResume.Invoke(this);
                    }
                }
            }

            /// <summary>
            /// Retrieves the amount of time the coroutine has executed.
            /// </summary>
            /// <returns>Returns 0 if the coroutine is not executing.</returns>
            public float GetExecutingTime()
            {
                float executionTime = m_ExecutionTime;
                if (!m_IsPaused && m_IsExecuting)
                {
                    executionTime += Time.time - m_TimeAtStart;
                }
                return executionTime;
            }
            /// <summary>
            /// Retrieves the amount of time the coroutine has been paused.
            /// </summary>
            /// <returns>Returns 0 if the coroutine is not executing.</returns>
            public float GetPausedTime()
            {
                float pausedTime = m_PauseTime;
                if (m_IsPaused && m_IsExecuting)
                {
                    pausedTime += Time.time - m_TimeAtPause;
                }
                return pausedTime;
            }

            /// <summary>
            /// How much time has the coroutine been executing for.
            /// </summary>
            /// <returns></returns>
            public float GetTotalExecutionTime()
            {
                return GetExecutingTime() + GetPausedTime();
            }

            /// <summary>
            /// The execution method to override. This will get invoked every frame the coroutine is running.
            /// It is executed before the yield.
            /// </summary>
            protected abstract void OnExecute();
            /// <summary>
            /// The execution method to override. This will gtt invoked every frame the coroutine is running.
            /// It is executed after the yield.
            /// </summary>
            protected abstract void OnPostExecute();

            protected virtual IEnumerator<YieldInstruction> GetRoutine()
            {
                m_TimeAtStart = Time.time;
                ///As long as the coroutine is not done
                do
                {
                    //If the coroutine is not paused
                    if (!m_IsPaused)
                    {
                        ///Execute Callback
                        OnExecute();
                        ///Yield the coroutine for x thing
                        yield return m_Coroutine.Yield();
                        ///Execute Callback
                        OnPostExecute();
                        ///Post Yield. This will push the state of the object and flag IsDone or not.
                        m_Coroutine.PostYield();
                    }
                    else
                    {
                        ///Otherwise if the coroutine is paused just wait for the next frame.
                        yield return new WaitForEndOfFrame();
                    }
                } while (!m_Coroutine.IsDone());

                ///Invoke the Finish Callback
                if (m_OnCoroutineFinish != null)
                {
                    m_OnCoroutineFinish.Invoke(this);
                }

                m_ExecutionTime = 0.0f;
                m_PauseTime = 0.0f;
                m_TimeAtPause = 0.0f;
                m_TimeAtStart = 0.0f;
                m_IsExecuting = false;
            }

            /// <summary>
            /// If the coroutine has been started and registered by unity
            /// This will be true until the coroutine is finished.
            /// </summary>
            public bool isExecuting
            {
                get { return m_IsExecuting; }
            }
            /// <summary>
            /// If the coroutine has paused execution. Unity will still process this but the execution will not be processed.
            /// </summary>
            public bool isPaused
            {
                get { return m_IsPaused; }
            }
            /// <summary>
            /// The yield instruction to use.
            /// </summary>
            public Type coroutineType
            {
                get { return m_Coroutine != null ? m_Coroutine.GetType() : null; }
            }

            /// <summary>
            /// Gets called when the coroutine finishes execution
            /// </summary>
            public CoroutineCallback onCoroutineFinish
            {
                get { return m_OnCoroutineFinish; }
                set { m_OnCoroutineFinish = value; }
            }
            /// <summary>
            /// Gets called when the coroutine pauses execution
            /// </summary>
            public CoroutineCallback onCoroutinePause
            {
                get { return m_OnCoroutinePause; }
                set { m_OnCoroutinePause = value; }
            }
            /// <summary>
            /// Gets called when the coroutine resumes execution.
            /// </summary>
            public CoroutineCallback onCoroutineResume
            {
                get { return m_OnCoroutineResume; }
                set { m_OnCoroutineResume = value; }
            }

            /// <summary>
            /// Gets called when the coroutine was stopped
            /// </summary>
            public CoroutineCallback onCoroutineStopped
            {
                get { return m_OnCoroutineStopped; }
                set { m_OnCoroutineStopped = value; }
            }
        }
    }
}

