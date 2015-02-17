using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Gem
{

    public class UIGame : MonoBehaviour
    {
        #region SINGLETON IMPLEMENTATION
        private static UIGame s_Instance = null;
        private static UIGame instance
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
                    persistent = new GameObject(Constants.GAME_OBJECT_UI_GAME);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if (persistent != null)
            {
                s_Instance = persistent.GetComponent<UIGame>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<UIGame>();
                }
            }
        }
        private static bool SetInstance(UIGame aInstance)
        {
            if (s_Instance == null)
            {
                s_Instance = aInstance;
                return true;
            }
            return false;
        }
        private static void DestroyInstance(UIGame aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        [SerializeField]
        private List<UIItemSlot> m_ItemSlots = new List<UIItemSlot>(4);

        [SerializeField]
        private List<UIAbilitySlot> m_AbilitySlots = new List<UIAbilitySlot>(12);

        [SerializeField]
        private Image m_SelectedUnitPortrait = null;
        [SerializeField]
        private Text m_SelectedUnitName = null;
        [SerializeField]
        private Text m_SelectedUnitLevel = null;
        [SerializeField]
        private Text m_SelectedUnitArmor = null;
        [SerializeField]
        private Text m_SelectedUnitHealth = null;
        [SerializeField]
        private Text m_SelectedUnitMana = null;

        [SerializeField]
        private Button m_SelectBuilderBtn = null;
        [SerializeField]
        private Button m_SelectWorkersBtn = null;
        [SerializeField]
        private Button m_SelectHuntersBtn = null;
        [SerializeField]
        private Button m_SelectAllBtn = null;

        [SerializeField]
        private Rect[] m_RaycastIgnoreRects = null;
        [SerializeField]
        private bool m_ShowRaycastIgnoreRect = true;

        private Actor m_SelectedActor = null;

        #region UNITY MESSAGES

        private void Awake()
        {
            SetInstance(this);
        }
        private void OnDestroy()
        {
            DestroyInstance(this);
        }

        private void Start()
        {
            ///Initialize all the item slot buttons.
            for (int i = 0; i < m_ItemSlots.Count; i++ )
            {
                UIItemSlot itemSlot = m_ItemSlots[i];
                int index = i;
                itemSlot.button.onClick.AddListener(() => OnItemAction(index));
            }

            ///Initialize all the ability slot buttons
            for(int i = 0; i < m_AbilitySlots.Count; i++)
            {
                UIAbilitySlot abilitySlot = m_AbilitySlots[i];
                int index = i;
                abilitySlot.button.onClick.AddListener(() => OnAbilityAction(index));
            }

        }

        private void OnGUI()
        {
            if(m_ShowRaycastIgnoreRect && m_RaycastIgnoreRects != null)
            {
                foreach(Rect rect in m_RaycastIgnoreRects)
                {
                    GUI.Box(rect, "");
                }
                
            }
        }


        #endregion

        #region PUBLIC STATICS

        public static void SelectActor(Actor aActor)
        {
            if(instance != null)
            {
                instance.OnSelectActor(aActor);
            }
        }

        public static bool ScreenPointOnGUI(Vector3 aPoint)
        {
            return instance == null ? false : instance.IsScreenPointOnGUI(aPoint);
        }

        #endregion



        #region FUNCTION HANDLERS
        private bool IsScreenPointOnGUI(Vector3 aPoint)
        {
            if(m_RaycastIgnoreRects == null)
            {
                return false;
            }

            aPoint.y = Screen.height - aPoint.y;

            foreach(Rect rect in m_RaycastIgnoreRects)
            {
                if(rect.Contains(aPoint))
                {
                    Debug.Log("Rect Contains Point");
                    return true;
                }
            }
            Debug.Log("Rect does not contain point: " + aPoint);
            return false;
        }

        private void OnSelectActor(Actor aActor)
        {
            m_SelectedActor = aActor;
            if(aActor != null)
            {
                if(typeof(Unit) == aActor.GetType())
                {
                    OnSelectUnit(aActor as Unit);
                }
                else
                {
                    Debug.Log("Selecting Unknown Actor");
                }
            }
            else
            {
                Debug.Log("Deselecting Actor");
            }
        }
        
        private void OnSelectUnit(Unit aUnit)
        {
            if(aUnit == null)
            {
                return;
            }

            Debug.Log("Selecting Unit: " + aUnit.GetActorName());

            ///Set Item Slots
            

            ///Set Ability Slots
            

            ///Set Unit Ability Portrait

        }

        private void OnAbilityAction(int aIndex)
        {
            //TODO: Do a check to see if the active player is the one who can issue orders to the actor
            if(m_SelectedActor != null)
            {
                m_SelectedActor.InvokeActorEvent(new ActorEvent(ActorEventType.IssueOrderAbilityAction, aIndex));
            }
        }

        private void OnItemAction(int aIndex)
        {
            //TODO: Do a check to see if the active player is the one who can issue orders to the actor.
            if(m_SelectedActor != null)
            {
                m_SelectedActor.InvokeActorEvent(new ActorEvent(ActorEventType.IssueOrderItemAction, aIndex));
            }
        }

        #endregion

    }
}

