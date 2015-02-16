using UnityEngine;
using System.Collections.Generic;

namespace Gem
{
    public class GameSelection : MonoBehaviour
    {
        #region SINGLETON IMPLEMENTATION
        private static GameSelection s_Instance = null;
        private static GameSelection instance
        {
            get { if (s_Instance == null) { CreateInstance(); } return s_Instance; }
        }
        private static void CreateInstance()
        {
            GameObject persistent = GameObject.Find(Constants.GAME_OBJECT_PERSISTENT);
            if (persistent == null)
            {
                if (Application.isPlaying)
                {
                    persistent = new GameObject(Constants.GAME_OBJECT_GAME_SELECTION);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if (persistent != null)
            {
                s_Instance = persistent.GetComponent<GameSelection>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<GameSelection>();
                }
            }
        }
        private static bool SetInstance(GameSelection aInstance)
        {
            if (s_Instance == null)
            {
                s_Instance = aInstance;
                return true;
            }
            return false;
        }
        private static void DestroyInstance(GameSelection aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        [SerializeField]
        private float m_SelectionHeight = 50.0f;
        [SerializeField]
        private GUIStyle m_Style = new GUIStyle();
        private bool m_IsDragging = false;

        private Vector2 m_StartPosition = Vector2.zero;
        private Vector2 m_CurrentPosition = Vector2.zero;

        private List<Actor> m_SelectedActors = new List<Actor>();



        private Vector3 m_WorldStartPosition = Vector3.zero;
        private Vector3 m_WorldEndPosition = Vector3.zero;

        private Vector3 m_BoundsCenter = Vector3.zero;
        private Vector3 m_BoundsExtents = Vector3.zero;

        private BoxCollider m_BoxCollider = null;

        private List<Actor> m_DragSelectionBuffer = new List<Actor>();


        private void Start()
        {
            if(!SetInstance(this))
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);

            m_BoxCollider = GetComponent<BoxCollider>();
        }
        private void OnDestroy()
        {
            DestroyInstance(this);
        }



        // Update is called once per frame
        void Update()
        {
            ///This is done to make the transform "dirty" and force a physics update too allow for OnTriggerEnter/Exit messages.
            transform.position = Vector3.one;
            transform.position = Vector3.zero;

            if (!m_IsDragging)
            {
                CheckSingleSelect();
            }
            CheckDragSelect();

            

            
            
        }


        void CheckDragSelect()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_BoxCollider.enabled = true;
                m_IsDragging = true;
                m_StartPosition = Input.mousePosition;
                Camera cam = Camera.main;

                Ray rayStart = cam.ScreenPointToRay(m_StartPosition);
                RaycastHit hitStart;

                if (Physics.Raycast(rayStart, out hitStart, 100.0f))
                {
                    m_WorldStartPosition = hitStart.point;
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_BoxCollider.enabled = false;
                m_IsDragging = false;

                foreach (Actor actor in m_DragSelectionBuffer)
                {
                    if (!m_SelectedActors.Contains(actor))
                    {
                        m_SelectedActors.Add(actor);
                    }
                }

                m_DragSelectionBuffer.Clear();

            }

            if (m_IsDragging)
            {
                m_CurrentPosition = Input.mousePosition;
                Camera cam = Camera.main;

                Ray rayCurrent = cam.ScreenPointToRay(m_CurrentPosition);
                RaycastHit hitCurrent;

                if (Physics.Raycast(rayCurrent, out hitCurrent, 100.0f))
                {
                    m_WorldEndPosition = hitCurrent.point;
                }

                Vector3 worldDifference = Vector3.zero;
                worldDifference.x = m_WorldStartPosition.x - m_WorldEndPosition.x;
                worldDifference.y = m_WorldStartPosition.y - m_WorldEndPosition.y;
                worldDifference.z = m_WorldStartPosition.z - m_WorldEndPosition.z;


                m_BoundsCenter.x = m_WorldStartPosition.x + -worldDifference.x * 0.5f;
                m_BoundsCenter.y = m_WorldStartPosition.y + -worldDifference.y * 0.5f;
                m_BoundsCenter.z = m_WorldStartPosition.z + -worldDifference.z * 0.5f;

                m_BoundsExtents.x = Mathf.Abs(worldDifference.x);
                m_BoundsExtents.y = m_SelectionHeight;
                m_BoundsExtents.z = Mathf.Abs(worldDifference.z);

                m_BoxCollider.center = m_BoundsCenter;
                m_BoxCollider.size = m_BoundsExtents;
            }
        }

        void CheckSingleSelect()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 1000.0f))
                {
                    Actor actor = hitInfo.collider.GetComponent<Actor>();
                    if (actor != null)
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (!m_SelectedActors.Contains(actor))
                            {
                                //TODO: Replace null with whoever the current player is.
                                actor.InvokeActorEvent(new ActorEvent(ActorEventType.Select, null));
                                m_SelectedActors.Add(actor);
                            }
                        }
                        else if (Input.GetKey(KeyCode.LeftAlt))
                        {
                            if (m_SelectedActors.Contains(actor))
                            {
                                //TODO: Replace null with whoever the current player is.
                                actor.InvokeActorEvent(new ActorEvent(ActorEventType.Deselect, null));
                                m_SelectedActors.Remove(actor);
                            }
                        }
                        else
                        {
                            foreach (Actor enumeratedActor in m_SelectedActors)
                            {
                                //TODO: Replace null with whoever the current player is.
                                enumeratedActor.InvokeActorEvent(new ActorEvent(ActorEventType.Deselect, null));
                            }
                            m_SelectedActors.Clear();

                            //TODO: Replace null with whoever the current player is.
                            actor.InvokeActorEvent(new ActorEvent(ActorEventType.Select, null));
                            m_SelectedActors.Add(actor);

                        }
                    }
                    else
                    {
                        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt))
                        {
                            foreach (Actor enumeratedActor in m_SelectedActors)
                            {
                                //TODO: Replace null with whoever the current player is.
                                enumeratedActor.InvokeActorEvent(new ActorEvent(ActorEventType.Deselect, null));
                            }
                            m_SelectedActors.Clear();
                        }
                    }
                }
                else
                {
                    if (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.LeftAlt))
                    {
                        foreach (Actor actor in m_SelectedActors)
                        {
                            //TODO: Replace null with whoever the current player is.
                            actor.InvokeActorEvent(new ActorEvent(ActorEventType.Deselect, null));
                        }
                        m_SelectedActors.Clear();

                    }
                }

            }
        }

        private bool Raycast(Ray aRay, out RaycastHit aHit, float aDistance)
        {
            return m_BoxCollider.Raycast(aRay, out aHit, aDistance);
        }

        private void OnTriggerEnter(Collider aCollider)
        {
            Actor actor = aCollider.GetComponent<Actor>();
            if(actor != null)
            {
                if(!m_DragSelectionBuffer.Contains(actor))
                {
                    //TODO: Replace null with the whoever the current player is.
                    actor.InvokeActorEvent(new ActorEvent(ActorEventType.Select, null));
                    m_DragSelectionBuffer.Add(actor);
                }
            }
        }

        private void OnTriggerExit(Collider aCollider)
        {
            Actor actor = aCollider.GetComponent<Actor>();
            if(actor != null)
            {
                if(!m_SelectedActors.Contains(actor))
                {
                    //TODO: Replace null with the whoever the current player is.
                    actor.InvokeActorEvent(new ActorEvent(ActorEventType.Deselect, null));
                }
                m_DragSelectionBuffer.Remove(actor);
            }
        }

        private void OnGUI()
        {
            if(m_IsDragging)
            {
                float boxLeft = m_StartPosition.x;
                float boxRight = m_CurrentPosition.x;
                float boxTop = m_StartPosition.y;
                float boxBottom = m_CurrentPosition.y;

                Rect rect = new Rect();
                rect.x = m_StartPosition.x;
                rect.y = Screen.height - m_StartPosition.y;
                rect.width = -(m_StartPosition.x - m_CurrentPosition.x);
                rect.height = m_StartPosition.y - m_CurrentPosition.y;

                GUI.Box(rect, "", m_Style);
            }
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_WorldStartPosition, 1.0f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_WorldEndPosition, 1.0f);
        }

    }

}

