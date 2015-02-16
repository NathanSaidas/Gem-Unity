using UnityEngine;
using System.Collections;
using Gem.Coroutines;


namespace Gem
{
    namespace Examples
    {
        /// <summary>
        /// Class to implement the CoroutineEx
        /// This just moves a transforms position based on time every execution.
        /// </summary>
        public class MoveRoutine : CoroutineEx
        {
            private Transform m_Transform = null;

            public MoveRoutine(Transform aTransform, CoroutineYield aCoroutine) : base(aCoroutine)
            {
                m_Transform = aTransform;
            }

            protected override void OnExecute()
            {
                if(m_Transform != null)
                {
                    m_Transform.position += Vector3.up * Mathf.Sin(Time.time * 5.0f);
                }
            }

            protected override void OnPostExecute()
            {
                
            }

        }

        public class CoroutineExExample : MonoBehaviour
        {
            private MoveRoutine m_Routine = null;

            public int m_Frames = 30000;
            public float m_ExecutionTime = 0.0f;
            public float m_PauseTime = 0.0f;
            public string m_Type = string.Empty;

            void Start()
            {
                m_Routine = new MoveRoutine(transform, new YieldWaitFixedUpdate(m_Frames));
                m_Routine.onCoroutineFinish = OnCoroutineFinish;
                m_Routine.onCoroutinePause = OnCoroutinePause;
                m_Routine.onCoroutineResume = OnCoroutineResume;
                m_Type = m_Routine.coroutineType.Name;
            }

            void Update()
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    m_Routine.Start();
                }
                if(Input.GetKeyDown(KeyCode.Alpha2))
                {
                    m_Routine.Stop();
                }
                if(Input.GetKeyDown(KeyCode.Alpha3))
                {
                    m_Routine.Pause();
                }
                if(Input.GetKeyDown(KeyCode.Alpha4))
                {
                    m_Routine.Resume();
                }

                m_PauseTime = m_Routine.GetPausedTime();
                m_ExecutionTime = m_Routine.GetExecutingTime();
            }

            void OnCoroutinePause(CoroutineEx aCoroutine)
            {

            }
            void OnCoroutineResume(CoroutineEx aCoroutine)
            {

            }
            void OnCoroutineFinish(CoroutineEx aCoroutine)
            {
                Debug.Log("Finished");
            }
        }

    }
}

