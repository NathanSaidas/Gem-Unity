using UnityEngine;
using System.Collections;
using Gem.Coroutines;

namespace Gem
{
    public class AbilityActor : Actor
    {
        private Actor m_Caster = null;
        private CastInfo m_CastInfo = null;
        private Ability m_Ability = null;

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

        protected void Pulse(float aPulseRadius)
        {
            Pulse(aPulseRadius, Physics.kDefaultRaycastLayers);
        }
        
        /// <summary>
        /// Collect a bunch of colliders in a radius and invokes the AbilityEffect.
        /// 
        /// Units and Structures should handle this appropriately.
        /// </summary>
        /// <param name="aPulseRadius"></param>
        /// <param name="aLayerMask"></param>
        protected void Pulse(float aPulseRadius, int aLayerMask)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, aPulseRadius,aLayerMask);
            foreach(Collider col in colliders)
            {
                HandleCollisionWithTarget(col);
            }
        }

        public override ActorType GetActorType()
        {
            return ActorType.Ability;
        }

        /// <summary>
        /// Handles the collision event with a collider.
        /// 
        /// Determines if the target is an approrpaite target or not.
        /// 
        /// Use with Pulse and OnTriggerEnter events.
        /// </summary>
        /// <param name="aCollider"></param>
        protected void HandleCollisionWithTarget(Collider aCollider)
        {
            Actor actor = aCollider.GetComponent<Actor>();
            HandleCollisionWithTarget(actor);
            
        }

        protected void HandleCollisionWithTarget(Actor aActor)
        {
            if (aActor != null)
            {
                OnHitTarget(aActor);
            }
        }

        /// <summary>
        /// This method gets invoked for each actor affected by this ability.
        /// 
        /// 
        /// </summary>
        /// <param name="aActor"></param>
        protected virtual void OnHitTarget(Actor aActor)
        {

        }

        public virtual void InitializeAbility(Actor aCaster, CastInfo aCastInfo)
        {
            caster = aCaster;
            castInfo = aCastInfo;
        }


        public Actor caster
        {
            get { return m_Caster; }
            set { m_Caster = value; }
        }
        public CastInfo castInfo
        {
            get { return m_CastInfo; }
            set { m_CastInfo = value; }
        }
        public Ability ability
        {
            get { return m_Ability; }
            set { m_Ability = value; }
        }

    }
}


