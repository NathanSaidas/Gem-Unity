using UnityEngine;
using System.Collections.Generic;

#region CHANGE LOG
/*  Februrary 16 2015 - Nathan Hanlan - Added GameSelection class.
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// This class is a per-scene singleton. This class allows global access to actor selection.
    /// 
    /// Features:
    /// • Actor Selection/Deselection based on Click events
    /// • Actor Selection using Click-Drag events.
    /// • Visual Representation of the Drag Selection
    /// 
    /// How To Use:
    /// • Attach this component to a gameobject and set the appropriate texture for the Style
    /// 
    /// TODO: 
    /// • Change the selection / deselection invoker to a player. This comes in with Networking Update.
    /// </summary>
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

        /// <summary>
        /// The height of the selection volume.
        /// </summary>
        [SerializeField]
        private float m_SelectionHeight = 50.0f;
        /// <summary>
        /// The GUIStyle for the selection GUI. (Set Normal Texture and make the Border equal to the pixel units of the Border)
        /// </summary>
        [SerializeField]
        private GUIStyle m_Style = new GUIStyle();
        /// <summary>
        /// Whether or not the Game Selection is in drag mode.
        /// </summary>
        private bool m_IsDragging = false;

        /// <summary>
        /// The starting screen position of the mouse drag.
        /// </summary>
        private Vector2 m_DragScreenStartPosition = Vector2.zero;
        /// <summary>
        /// The ending screen position of the mouse drag.
        /// </summary>
        private Vector2 m_DragScreenCurrentPosition = Vector2.zero;
        /// <summary>
        /// The starting world position of the mouse drag
        /// </summary>
        private Vector3 m_DragWorldStartPosition = Vector3.zero;
        /// <summary>
        /// The ending world position of the mouse drag.
        /// </summary>
        private Vector3 m_DragWorldEndPosition = Vector3.zero;
        /// <summary>
        /// The currently selected actors
        /// </summary>
        private List<Actor> m_SelectedActors = new List<Actor>();
        /// <summary>
        /// The actors being selected in the drag process. This buffer is copied to the selected actors on drag end.
        /// </summary>
        private List<Actor> m_DragSelectionBuffer = new List<Actor>();
        /// <summary>
        /// The box collider checking for the selection.
        /// </summary>
        private BoxCollider m_BoxCollider = null;

        /// <summary>
        /// Sets the singleton...
        /// </summary>
        private void Awake()
        {
            if(!SetInstance(this))
            {
                Destroy(gameObject);
                return;
            }
            m_BoxCollider = GetComponent<BoxCollider>();
        }

        /// <summary>
        /// Destroys the singleton instance.
        /// </summary>
        private void OnDestroy()
        {
            DestroyInstance(this);
        }



        
        void Update()
        {
            ///This is done to make the transform "dirty" and force a physics update too allow for OnTriggerEnter/Exit messages.
            transform.position = Vector3.one;
            transform.position = Vector3.zero;
            bool madeSelection = false;
            ///Check single selection while not dragging.
            if (!m_IsDragging)
            {
                madeSelection = CheckSingleSelect();
            }
            if(CheckDragSelect() || madeSelection)
            {
                OnSelectionChanged();
            }

            

            
            
        }


        bool CheckDragSelect()
        {
            bool result = false;

            if (Input.GetMouseButtonDown(Constants.INPUT_SELECTION))
            {
                m_BoxCollider.enabled = true;
                m_IsDragging = true;
                m_DragScreenStartPosition = Input.mousePosition;
                Camera cam = Camera.main;

                ///Calcuate the starting position of the drag.
                Ray rayStart = cam.ScreenPointToRay(m_DragScreenStartPosition);
                RaycastHit hitStart;
                if (Physics.Raycast(rayStart, out hitStart, 100.0f))
                {
                    m_DragWorldStartPosition = hitStart.point;
                }
                OnDragBegin();
            }
            else if (Input.GetMouseButtonUp(Constants.INPUT_SELECTION))
            {
                m_BoxCollider.enabled = false;
                m_IsDragging = false;
                bool updateSelection = false;
                ///Copy Buffer to Core Selection
                foreach (Actor actor in m_DragSelectionBuffer)
                {
                    if (!m_SelectedActors.Contains(actor))
                    {
                        updateSelection = true;
                        m_SelectedActors.Add(actor);
                    }
                }
                m_DragSelectionBuffer.Clear();
                OnDragFinish();
                result = updateSelection;
            }

            if (m_IsDragging && !Input.GetKey(Constants.INPUT_REMOVE_SELECTION))
            {
                m_DragScreenCurrentPosition = Input.mousePosition;
                Camera cam = Camera.main;

                ///Recalculate the current position
                Ray rayCurrent = cam.ScreenPointToRay(m_DragScreenCurrentPosition);
                RaycastHit hitCurrent;

                if (Physics.Raycast(rayCurrent, out hitCurrent, 100.0f))
                {
                    m_DragWorldEndPosition = hitCurrent.point;
                }

                ///Determine the world difference in positions
                ///And then calculate the world center / extents with it.
                Vector3 worldDifference = Vector3.zero;
                worldDifference.x = m_DragWorldStartPosition.x - m_DragWorldEndPosition.x;
                worldDifference.y = m_DragWorldStartPosition.y - m_DragWorldEndPosition.y;
                worldDifference.z = m_DragWorldStartPosition.z - m_DragWorldEndPosition.z;

                Vector3 boundsCenter = Vector3.zero;
                Vector3 boundsExtents = Vector3.zero;

                boundsCenter.x = m_DragWorldStartPosition.x + -worldDifference.x * 0.5f;
                boundsCenter.y = m_DragWorldStartPosition.y + -worldDifference.y * 0.5f;
                boundsCenter.z = m_DragWorldStartPosition.z + -worldDifference.z * 0.5f;

                boundsExtents.x = Mathf.Abs(worldDifference.x);
                boundsExtents.y = m_SelectionHeight;
                boundsExtents.z = Mathf.Abs(worldDifference.z);

                m_BoxCollider.center = boundsCenter;
                m_BoxCollider.size = boundsExtents;
            }
            return result;
        }

        bool CheckSingleSelect()
        {
            if (Input.GetMouseButtonDown(Constants.INPUT_SELECTION) && !UIGame.ScreenPointOnGUI(Input.mousePosition))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 1000.0f))
                {
                    Actor actor = hitInfo.collider.GetComponent<Actor>();
                    if (actor != null)
                    {
                        ///Add Selection
                        if (Input.GetKey(Constants.INPUT_ADD_SELECTION))
                        {
                            if (!m_SelectedActors.Contains(actor))
                            {
                                //TODO: Replace null with whoever the current player is.
                                actor.InvokeActorEvent(new ActorEvent(ActorEventType.Select, null));
                                m_SelectedActors.Add(actor);
                            }
                        }
                        ///Remove Selection
                        else if (Input.GetKey(Constants.INPUT_REMOVE_SELECTION))
                        {
                            if (m_SelectedActors.Contains(actor))
                            {
                                //TODO: Replace null with whoever the current player is.
                                actor.InvokeActorEvent(new ActorEvent(ActorEventType.Deselect, null));
                                m_SelectedActors.Remove(actor);
                            }
                        }
                        ///Deselect Previous Selections, Reselect Current Actor
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
                    ///Deselect All if not adding or removing.
                    else
                    {
                        if (!Input.GetKey(Constants.INPUT_ADD_SELECTION) && !Input.GetKey(Constants.INPUT_REMOVE_SELECTION))
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
                    ///Deselect All if not adding or removing.
                    if (!Input.GetKey(Constants.INPUT_ADD_SELECTION) && !Input.GetKey(Constants.INPUT_REMOVE_SELECTION))
                    {
                        foreach (Actor actor in m_SelectedActors)
                        {
                            //TODO: Replace null with whoever the current player is.
                            actor.InvokeActorEvent(new ActorEvent(ActorEventType.Deselect, null));
                        }
                        m_SelectedActors.Clear();

                    }
                }
                return true;
            }
            else
            {
                return false;
            }
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
                float boxLeft = m_DragScreenStartPosition.x;
                float boxRight = m_DragScreenCurrentPosition.x;
                float boxTop = m_DragScreenStartPosition.y;
                float boxBottom = m_DragScreenCurrentPosition.y;

                Rect rect = new Rect();
                rect.x = m_DragScreenStartPosition.x;
                rect.y = Screen.height - m_DragScreenStartPosition.y;
                rect.width = -(m_DragScreenStartPosition.x - m_DragScreenCurrentPosition.x);
                rect.height = m_DragScreenStartPosition.y - m_DragScreenCurrentPosition.y;

                GUI.Box(rect, "", m_Style);
            }
            
        }

        private void OnDragBegin()
        {


        }
        private void OnDragFinish()
        {

        }

        private void OnSelectionChanged()
        {
            if(m_SelectedActors.Count > 0)
            {
                UIGame.SelectActor(m_SelectedActors[0]);
            }
            else
            {
                UIGame.SelectActor(null);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(m_DragWorldStartPosition, 1.0f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_DragWorldEndPosition, 1.0f);
        }

        /// <summary>
        /// Retrieves a list of the selected units.
        /// </summary>
        /// <returns></returns>
        public static List<Unit> GetSelectedUnits()
        {
            List<Unit> units = new List<Unit>();
            if(instance != null)
            {
                foreach (Actor actor in instance.m_SelectedActors)
                {
                    Unit unit = actor as Unit;
                    if (unit != null)
                    {
                        units.Add(unit);
                    }
                }
            }
            return units;
        }

        /// <summary>
        /// Retrieves a list of selected structures.
        /// </summary>
        /// <returns></returns>
        public List<Unit> GetSelectedStructures()
        {
            return null;
        }

        public static float selectionHeight
        {
            get { return instance != null ? instance.m_SelectionHeight : 0.0f; }
            set { if (instance != null) { instance.m_SelectionHeight = value; } }
        }
        public static bool isDragging
        {
            get { return instance != null ? instance.m_IsDragging : false; }
        }
        public static Vector2 dragScreenStartPosition
        {
            get { return instance != null ? instance.m_DragScreenStartPosition : Vector2.zero; }
        }
        public static Vector2 dragScreenCurrentPosition
        {
            get { return instance != null ? instance.m_DragScreenCurrentPosition : Vector2.zero; }
        }
        public static Bounds dragBounds
        {
            get { return instance != null ? instance.m_BoxCollider.bounds : default(Bounds); }
        }
        public static List<Actor> selectedActors
        {
            get { return instance != null ? instance.m_SelectedActors : null; }
        }


    }

}

