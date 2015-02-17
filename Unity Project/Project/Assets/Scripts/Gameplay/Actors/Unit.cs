using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added class Unit
 * 
 */
#endregion

namespace Gem
{

    /// <summary>
    /// Represents an actor in the world with stats and health.
    /// 
    /// A Unit is an Actor who...
    /// 
    /// - Has Life
    /// - Has Model Controller
    /// - Has Animation Controller
    /// - Has Visual Effects Controller
    /// 
    /// - May have Resource (Mana/Energy/Combo Points Etc)
    /// - May have Abilities (Inventory, Harvest, Lazer Etc)
    /// - May have Effects (Damage over time, Invisible Etc)
    /// 
    /// - AI Pathfinding (Move, Hold, Follow)
    /// - Controlled by Player (AI -> Calculate Decision -> Issue Order -> AI Reacts or Human -> Issue Order -> AI Reacts)
    /// 
    /// - Can Die
    /// - Can Kill
    /// - Can Damage
    /// 
    /// </summary>
    public class Unit : Actor
    {
        // -- Life
        private float m_MaxHealth = 0.0f;
        private float m_CurrentHealth = 0.0f;

        // -- Resources
        private float m_MaxResource = 0.0f;
        private float m_CurrentResource = 0.0f;

        // -- External Controllers
        private UnitModelController m_ModelControler = null;
        private UnitAnimationController m_AnimationController = null;
        private UnitVisualEffectsController m_VisualEffectsController = null;
        private NavMeshAgent m_NavMeshAgent = null;

        // -- Unit Abilities
        [SerializeField]
        private List<Ability> m_Abilities = new List<Ability>();

        // -- Required Setup
        [SerializeField]
        private MeshRenderer m_SelectionCircle = null;

        // -- Unit States
        private bool m_IsSelected = false;

        // -- AI Stuff
        private bool m_InTransit = false;
        private Vector3 m_MoveToPosition = Vector3.zero;
        private Unit m_MoveToTarget = null;
        private Unit m_FollowTarget = null;


        public override ActorType GetActorType()
        {
            return ActorType.Unit;
        }

        private void Start()
        {
            InitializeActor();
            
        }

        protected override void OnInitializeActor()
        {
            foreach (Ability ability in m_Abilities)
            {
                ability.Initialize();
            }

            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            if(m_SelectionCircle == null)
            {
                DebugUtils.MissingProperty<Unit>("m_SelectionCircle");
            }
            else
            {
                m_SelectionCircle.enabled = false;
            }
        }

        private void Update()
        {   
            if(m_InTransit)
            {
                if(m_FollowTarget != null)
                {
                    if(!m_NavMeshAgent.SetDestination(m_FollowTarget.transform.position))
                    {
                        Stop();
                    }
                }
                else if(m_MoveToTarget != null)
                {
                    if(!m_NavMeshAgent.SetDestination(m_MoveToTarget.transform.position))
                    {
                        Stop();
                    }
                    
                    float distanceRemaining = Vector3.Distance(transform.position, m_MoveToTarget.transform.position);
                    if(distanceRemaining < m_NavMeshAgent.stoppingDistance)
                    {
                        Debug.Log("Reached Destination");
                        Stop();
                    }
                }
                else
                {
                    if(!m_NavMeshAgent.SetDestination(m_MoveToPosition))
                    {
                        Stop();
                    }
                    float distanceRemaining = Vector3.Distance(transform.position, m_MoveToPosition);
                    if (distanceRemaining < m_NavMeshAgent.stoppingDistance)
                    {
                        Debug.Log("Reached Destination");
                        Stop();
                    }
                }
            }

        }

        public override void InvokeActorEvent(ActorEvent aEvent)
        {
            switch(aEvent.eventType)
            {
                case ActorEventType.IssueOrderStop:
                    {
                        Stop();
                    }
                    break;
                case ActorEventType.IssueOrderMove:
                    {
                        if(aEvent.context != null)
                        {
                            Unit unit = aEvent.context as Unit;
                            if(unit != null)
                            {
                                Stop();
                                MoveTo(unit);
                            }
                            else
                            {
                                Stop();
                                MoveTo((Vector3)aEvent.context);
                            }
                        }
                    }
                    break;
                case ActorEventType.IssueOrderFollow:
                    {
                        Stop();
                        Follow((Unit)aEvent.context);
                    }
                    break;
                case ActorEventType.IssueOrderWarp:
                    {
                        Warp((Vector3)aEvent.context);
                    }
                    break;
                case ActorEventType.Select:
                    {
                        if(!m_IsSelected)
                        {
                            OnSelect(new PlayerName("Derpus Maximus", "LOL KING", PlayerIndex.Player_One));
                        }
                    }
                    break;
                case ActorEventType.Deselect:
                    {
                        if(m_IsSelected)
                        {
                            OnDeselect(new PlayerName("Derpus Maximus", "LOL KING", PlayerIndex.Player_One));
                        }
                    }
                    break;
                case ActorEventType.IssueOrderAbilityAction:
                    {
                        OnUseAbility((int)aEvent.context);
                    }
                    break;
                case ActorEventType.IssueOrderItemAction:
                    {
                        OnUseItem((int)aEvent.context);
                    }
                    break;
            }
        }

        private void Stop()
        {
            m_NavMeshAgent.Stop();
            m_InTransit = false;
            m_MoveToTarget = null;
            m_MoveToPosition = Vector3.zero;
            m_FollowTarget = null;
        }

        private void SetDestination(Vector3 aPoint)
        {
            m_NavMeshAgent.SetDestination(aPoint);
        }

        private void Warp(Vector3 aPoint)
        {
            m_NavMeshAgent.Warp(aPoint);
        }

        private void Follow(Unit aUnit)
        {
            m_FollowTarget = aUnit;
            m_InTransit = true;
        }
        private void MoveTo(Unit aUnit)
        {
            m_MoveToTarget = aUnit;
            m_InTransit = true;
        }
        private void MoveTo(Vector3 aPosition)
        {
            m_MoveToPosition = aPosition;
            m_InTransit = true;
        }

        private void OnSelect(PlayerName aPlayerName)
        {
            m_SelectionCircle.enabled = true;
            m_IsSelected = true;
        }
        private void OnDeselect(PlayerName aPlayerName)
        {
            m_SelectionCircle.enabled = false;
            m_IsSelected = false;
        }

        private void OnUseAbility(int aAbilityIndex )
        {
            Debug.Log("Unit Use Ability: " + aAbilityIndex);
        }

        private void OnUseItem(int aItemIndex)
        {
            Debug.Log("Unit Use Item: " + aItemIndex);
        }

    }
}