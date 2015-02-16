using UnityEngine;
using System.Collections;

namespace Gem
{
    public class ExampleAbility : AbilityActor
    {
        [SerializeField]
        private bool m_TravelToTarget = true;

        /// <summary>
        /// The speed at which the ability missle starts at
        /// </summary>
        [SerializeField]
        private float m_MissileSpeed = 0.0f;
        /// <summary>
        /// The acceleration of the ability missle
        /// </summary>
        [SerializeField]
        private float m_MissileAcceleration = 5.0f;
        /// <summary>
        /// The maximum speed the missile can be
        /// </summary>
        [SerializeField]
        private float m_MissileMaxSpeed = 300.0f;
        [SerializeField]
        private float m_SplashRadius = 0.0f;
        [SerializeField]
        private bool m_Pulse = false;
        [SerializeField]
        private float m_PulseFrequency = 0.0f;
        [SerializeField]
        private int m_MaxPulseCount = 0;
        
        [SerializeField]
        [EnumFlags]
        private ActorIdentifier m_AllowedActors = (ActorIdentifier)0;

        private int m_PulseCounts = 0;
        private float m_CurrentPulseTime = 0.0f; 
        private float m_SquareDistance = 0.0f;

       

        /// <summary>
        /// Use to invoke an event on the actor.
        /// </summary>
        /// <param name="aEvent">The event data. (Type,Context)</param>
        public override void InvokeActorEvent(ActorEvent aEvent)
        {

        }
        /// <summary>
        /// Override this method to finalize the actor once in its entire lifetime.
        /// 
        /// Entry to this method comes from the static Destroy Method. Common ways of invoking that is through
        ///     - OnDestroy
        ///     - Explicity use of the Actor static method
        /// </summary>
        protected override void OnFinalizeActor()
        {

        }
        /// <summary>
        /// Override this method to initialize the actor once in its entire lifetime.
        /// 
        /// Entry to this method comes from InitializationActor. Common ways of invoking that is through
        ///     - Unity Awake message method
        ///     - Unity Start message method
        ///     - Explicitly Initialize using Actor static Instantiate Methods
        ///     - Explicitly Initialize using GameObjectExtensions Instantiate Methods
        /// </summary>
        protected override void OnInitializeActor()
        {

        }

        public override void InitializeAbility(Actor aCaster, CastInfo aCastInfo)
        {
            base.InitializeAbility(aCaster, aCastInfo);


            ///Do I Start at the caster
            if(m_TravelToTarget)
            {
                transform.position = caster.transform.position;
            }
            ///Or Do I Start at the target
            else
            {
                transform.position = castInfo.GetTargetPosition();
            }

        }

        private void OnTriggerEnter(Collider aCollider)
        {
            if(aCollider != null)
            {
                if(m_Pulse)
                {
                    Pulse(m_SplashRadius);
                }
                else
                {
                    HandleCollisionWithTarget(aCollider);
                }
                
            }
        }

        private void Update()
        {
            ///Move to target position based on missle speed and time.
            MoveToTarget();

            ///World Point distance Check
            if(m_SquareDistance == 0.0f)
            {
                if(m_Pulse)
                {
                    Pulse(m_SplashRadius);
                }
                else
                {
                    if (castInfo.useTarget && castInfo.target != null)
                    {
                        HandleCollisionWithTarget(castInfo.target);
                    }
                }
                ability.OnAbilityDestroyed(this);
                Actor.Destroy(this);
            }

            if(m_Pulse && isActorAlive)
            {
                m_CurrentPulseTime += Time.deltaTime;
                if (m_CurrentPulseTime > m_PulseFrequency)
                {
                    m_PulseCounts++;
                    m_CurrentPulseTime = 0.0f;
                    if(m_PulseCounts > m_MaxPulseCount && m_MaxPulseCount != 0)
                    {
                        m_Pulse = false;
                    }

                    DebugUtils.Log("Pulsing: " + m_SplashRadius);
                    Pulse(m_SplashRadius);
                    
                }
            }
            
        }

        protected void MoveToTarget()
        {
            if (castInfo != null)
            {
                ///Get Position.
                Vector3 targetPosition = castInfo.GetTargetPosition();

                ///Increase the overall speed
                m_MissileSpeed += m_MissileAcceleration * Time.deltaTime;
                m_MissileSpeed = Mathf.Clamp(m_MissileSpeed, 0.0f, m_MissileMaxSpeed);

                ///Move towards the target
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, m_MissileSpeed * Time.deltaTime);

                ///Set the distance from target.
                m_SquareDistance = (targetPosition - transform.position).sqrMagnitude;
            }
        }

        /// <summary>
        /// This method gets invoked for each actor affected by this ability.
        /// 
        /// </summary>
        /// <param name="aActor"></param>
        protected override void OnHitTarget(Actor aActor)
        {
            ///Only hit units.
            if(aActor.GetActorType() != ActorType.Unit)
            {
                return;
            }

            Debug.Log("Apply Effect to Actor: " + aActor.GetActorName());
            //ability.OnAbilityDestroyed(this);
            //Actor.Destroy(this);

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.5f, 0.6f, 0.7f, 0.4f);
            Gizmos.DrawSphere(transform.position, m_SplashRadius);
        }
    }
}


