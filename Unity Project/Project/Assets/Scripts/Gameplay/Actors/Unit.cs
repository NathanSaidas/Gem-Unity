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

        [SerializeField]
        private List<Ability> m_Abilities = new List<Ability>();

        private Vector3 m_HitPoint = Vector3.zero;
        [SerializeField]
        private float m_Radius = 0.5f;
        [SerializeField]
        private float m_CooldownRemaining = 0.0f;

        public override ActorType GetActorType()
        {
            return ActorType.Unit;
        }

        private void Start()
        {
            InitializeActor();
            foreach(Ability ability in m_Abilities)
            {
                ability.Initialize();
            }
        }

        private void Update()
        {
            if(m_Abilities.Count == 0)
            {
                return;
            }
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if(Physics.Raycast(ray,out hitInfo, 1000.0f))
                {
                    Actor actor = hitInfo.collider.GetComponent<Actor>();
                    if(actor != null)
                    {
                        m_Abilities[0].BeginCast(actor);
                    }

                    //m_Abilities[0].BeginCast(hitInfo.point);
                    
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                
                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //RaycastHit hitInfo;
                //if (Physics.Raycast(ray, out hitInfo, 1000.0f))
                //{
                //    Actor actor = hitInfo.collider.GetComponent<Actor>();
                //    //m_Abilities[0].BeginCast(hitInfo.point);
                //    if (actor != null)
                //    {
                //        Debug.Log("Trying to cast at " + actor.GetActorName());
                //        m_Abilities[0].BeginSplashCast(actor,0.5f);
                //    }
                //}
                m_Abilities[0].BeginCastSelf();

            }

            //if(Input.GetKeyDown(KeyCode.S))
            //{
            //    m_Abilities[0].CancelCast();
            //}

            if(m_Abilities[0].IsOnCooldown())
            {
                m_CooldownRemaining = m_Abilities[0].GetTimeLeftOnCooldown();
            }
            else
            {
                m_CooldownRemaining = 0.0f;
            }
        }

        public void OnDrawGizmos()
        {
            if(m_Abilities.Count == 0)
            {
                Gizmos.color = Color.red * 0.65f * Color.green;
                Gizmos.DrawSphere(m_HitPoint, 0.5f);

                Gizmos.color = 0.65f * Color.green;
                Gizmos.DrawSphere(m_HitPoint, m_Radius);
            }
            

        }


    }
}