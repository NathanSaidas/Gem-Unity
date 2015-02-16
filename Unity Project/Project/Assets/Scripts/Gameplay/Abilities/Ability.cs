using UnityEngine;
using System;
using System.Collections.Generic;

using Gem.Coroutines;

namespace Gem
{
    
    [Serializable]
    public class Ability
    {
        public class AbilityRoutine : CoroutineEx
        {
            public AbilityRoutine(CoroutineYield aYield) : base(aYield)
            {

            }

            protected override void OnExecute()
            {

            }
            protected override void OnPostExecute()
            {

            }
        }

        
        [SerializeField]
        private Actor m_Caster = null;
        /// <summary>
        /// The name of the ability
        /// </summary>
        [SerializeField]
        private string m_Name = string.Empty;
        /// <summary>
        /// How much time before the ability can be used again
        /// </summary>
        [SerializeField]
        private float m_Cooldown = 0.0f;
        /// <summary>
        /// How much time it takes to cast the ability (m_Casted = true)
        /// </summary>
        [SerializeField]
        private float m_CastTime = 0.0f;
        /// <summary>
        /// If the ability is being casted
        /// </summary>
        [SerializeField]
        private bool m_IsCasting = false;
        /// <summary>
        /// A mask of all the allowed targets.
        /// </summary>
        [SerializeField]
        [EnumFlags]
        private ActorIdentifier m_AllowedTargets = (ActorIdentifier)0;
        /// <summary>
        /// The prefab that is used when the ability is activated.
        /// </summary>
        [SerializeField]
        private GameObject m_AbilityPrefab = null;
        /// <summary>
        /// The list of gameobjects spawned when the ability is spawned. Destroy them all when the ability has stopped being channeled.
        /// </summary>
        [SerializeField]
        private List<GameObject> m_SpawnedGameObject = null;

        
        private CastInfo m_CastInfo = new CastInfo();

        /// <summary>
        /// A coroutine to use for casting.
        /// </summary>
        private AbilityRoutine m_CastRoutine = null;
        private AbilityRoutine m_CooldownRoutine = null;
        


        public Ability()
        {
            
        }

        public void Initialize()
        {
            m_CastRoutine = new AbilityRoutine(new YieldWaitForSeconds(m_CastTime));
            m_CastRoutine.onCoroutineFinish = OnRoutineFinish;
            m_CastRoutine.onCoroutineStopped = OnCastCancelled;

            m_CooldownRoutine = new AbilityRoutine(new YieldWaitForSeconds(m_Cooldown));
            m_CooldownRoutine.onCoroutineFinish = OnRoutineFinish;

            m_CastInfo.Empty();
        }

        /// <summary>
        /// Starts the casting routine.
        /// 
        /// Example. Cast a fireball thats tracking a target...
        /// </summary>
        /// <param name="aTarget">The target to be tracking</param>
        /// /// <returns>Returns information about the cast.</returns>
        public CastResult BeginCast(Actor aTarget)
        {
            if (m_IsCasting)
            {
                return CastResult.CastInProgress;
            }

            ///Check Cooldown
            if(IsOnCooldown())
            {
                DebugUtils.LogError(ErrorCode.CAST_FAILED_COOLDOWN);
                return CastResult.IsOnCooldown; ;
            }

            ///Check target Mask
            ActorIdentifier actorIdentifier = aTarget.GetActorIdentifier();
            if((actorIdentifier & m_AllowedTargets) == 0)
            {
                DebugUtils.LogError(ErrorCode.CAST_FAILED_INVALID_TARGET);
                return CastResult.InvalidTarget;
            }

            ///Stop the coroutine casting execution.
            if(m_CastRoutine.isExecuting && !m_CastRoutine.isPaused)
            {
                m_CastRoutine.Stop();
            }

            Debug.Log("Start Cast");
            m_CastRoutine.Start();
            m_IsCasting = true;
            m_CastInfo.SetTarget(aTarget);
            return CastResult.Good;
        }
        /// <summary>
        /// 
        /// Example. Cast a fireball at a position in space. Only hits targets upon collision.
        /// </summary>
        /// <param name="aWorldPoint">The point to direct the cast at.</param>
        /// /// <returns>Returns information about the cast.</returns>
        public CastResult BeginCast(Vector3 aWorldPoint)
        {
            if (m_IsCasting)
            {
                return CastResult.CastInProgress;
            }
            ///Check Cooldown
            if(IsOnCooldown())
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_COOLDOWN);
                return CastResult.IsOnCooldown;
            }

            ///Check target Mask
            if ((ActorIdentifier.World & m_AllowedTargets) == 0)
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_INVALID_TARGET);
                return CastResult.InvalidTarget;
            }

            ///Stop the coroutine casting execution.
            if (m_CastRoutine.isExecuting && !m_CastRoutine.isPaused)
            {
                m_CastRoutine.Stop();
            }
            ///Start the coroutine
            m_CastRoutine.Start();
            m_IsCasting = true;
            ///Set the target
            m_CastInfo.SetTarget(aWorldPoint);
            return CastResult.Good;
        }
        /// <summary>
        /// 
        /// 
        /// Example. Cast a meteor at a target. Upon reaching the target do a pulse for the given radius. Affect all targets caught in the radius.
        /// </summary>
        /// <param name="aTarget">The target to direct the cast at.</param>
        /// <param name="aRadius">The radius of the effect.</param>
        /// /// <returns>Returns information about the cast.</returns>
        public CastResult BeginSplashCast(Actor aTarget, float aRadius)
        {
            if (m_IsCasting)
            {
                return CastResult.CastInProgress;
            }
            ///Check Cooldown
            if(IsOnCooldown())
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_COOLDOWN);
                return CastResult.IsOnCooldown;
            }

            ///Check target Mask
            ActorIdentifier actorIdentifier = aTarget.GetActorIdentifier();
            if ((actorIdentifier  & m_AllowedTargets) == 0)
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_INVALID_TARGET);
                return CastResult.InvalidTarget;
            }

            ///Stop the coroutine casting execution.
            if (m_CastRoutine.isExecuting && !m_CastRoutine.isPaused)
            {
                m_CastRoutine.Stop();
            }
            ///Start the coroutine
            m_CastRoutine.Start();
            m_IsCasting = true;
            ///Set the target.
            m_CastInfo.SetTarget(aTarget, aRadius);
            return CastResult.Good;
        }
        /// <summary>
        /// 
        /// 
        /// Example. Cast a meteor at a target world point. Upon reaching the target do a pulse for the given radius.
        /// </summary>
        /// <param name="aWorldPoint">The target world point to direct the cast at.</param>
        /// <param name="aRadius">The radius of the effect.</param>
        /// <returns>Returns information about the cast.</returns>
        public CastResult BeginSplashCast(Vector3 aWorldPoint, float aRadius)
        {
            if (m_IsCasting)
            {
                return CastResult.CastInProgress;
            }
            ///Check the cooldown.
            if(IsOnCooldown())
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_COOLDOWN);
                return CastResult.IsOnCooldown;
            }

            ///Check target Mask
            if ((ActorIdentifier.World & m_AllowedTargets) == 0)
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_INVALID_TARGET);
                return CastResult.InvalidTarget;
            }

            ///Stop the coroutine casting execution.
            if (m_CastRoutine.isExecuting && !m_CastRoutine.isPaused)
            {
                m_CastRoutine.Stop();
            }
            //Start the coroutine
            m_CastRoutine.Start();
            m_IsCasting = true;
            //Set the target.
            m_CastInfo.SetTarget(aWorldPoint, aRadius);
            return CastResult.Good;
        }

        /// <summary>
        /// 
        /// Example. Cast a buff or heal on self.
        /// </summary>
        /// <returns>Returns information about the cast.</returns>
        public CastResult BeginCastSelf()
        {
            if (m_IsCasting)
            {
                return CastResult.CastInProgress;
            }
            ///Check cooldown
            if(IsOnCooldown())
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_COOLDOWN);
                return CastResult.IsOnCooldown;
            }

            ///Check target mask
            if((ActorIdentifier.Self & m_AllowedTargets) == 0)
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_INVALID_TARGET);
                return CastResult.InvalidTarget;
            }

            ///Stop the coroutine casting execution.
            if (m_CastRoutine.isExecuting && !m_CastRoutine.isPaused)
            {
                m_CastRoutine.Stop();
            }
            //Start the coroutine
            m_CastRoutine.Start();
            m_IsCasting = true;
            //Set the target.
            m_CastInfo.SetTarget(m_Caster);
            return CastResult.Good;
        }

        /// <summary>
        /// 
        /// 
        /// Example. A buff or a heal on self and targets around self.
        /// </summary>
        /// <param name="aRadius">A radius to affect the targets.</param>
        /// <returns>Returns information about the cast.</returns>
        public CastResult BeginCastSelfSplash(float aRadius)
        {
            if (m_IsCasting)
            {
                return CastResult.CastInProgress;
            }
            ///Check cooldown
            if (IsOnCooldown())
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_COOLDOWN);
                return CastResult.IsOnCooldown;
            }

            ///Check target mask
            if ((ActorIdentifier.Self & m_AllowedTargets) == 0)
            {
                DebugUtils.LogWarning(ErrorCode.CAST_FAILED_INVALID_TARGET);
                return CastResult.InvalidTarget;
            }

            ///Stop the coroutine casting execution.
            if (m_CastRoutine.isExecuting && !m_CastRoutine.isPaused)
            {
                m_CastRoutine.Stop();
            }
            //Start the coroutine
            m_CastRoutine.Start();
            m_IsCasting = true;
            //Set the target.
            m_CastInfo.SetTarget(m_Caster,aRadius);
            return CastResult.Good;
        }

        public void CancelCast()
        {
            m_CastRoutine.Stop();
            m_IsCasting = false;
            m_CastInfo.Empty();
        }


        /// <summary>
        /// Determines whether or not the ability is on cooldown.
        /// </summary>
        /// <returns>Returns true if the ability is on cooldown.</returns>
        public bool IsOnCooldown()
        {
            return m_CooldownRoutine.isExecuting;
        }
        /// <summary>
        /// Gets the remaining time the ability is on cooldown fro.
        /// </summary>
        /// <returns></returns>
        public float GetTimeLeftOnCooldown()
        {
            return m_Cooldown - m_CooldownRoutine.GetExecutingTime();
        }

        private void OnRoutineFinish(CoroutineEx aCoroutine)
        {
            if(aCoroutine == m_CastRoutine)
            {
                //TODO: Create Object
                Actor actor = Actor.Instantiate(m_AbilityPrefab,m_Caster.transform.position,Quaternion.identity);
                AbilityActor abilityActor = actor as AbilityActor;
                if(abilityActor == null)
                {
                    Debug.Log("Bad Prefab");
                }
                else
                {
                    DebugUtils.Log("Cast Finished ");
                    abilityActor.InitializeAbility(m_Caster, m_CastInfo.Copy());
                    abilityActor.ability = this;
                    m_SpawnedGameObject.Add(abilityActor.gameObject);
                }
                
                m_CooldownRoutine.Start();
                m_IsCasting = false;
            }
            else if(aCoroutine == m_CooldownRoutine)
            {
                DebugUtils.Log("Cooldown Finished");
            }
        }

        private void OnCastCancelled(CoroutineEx aCoroutine)
        {

        }

        public void OnAbilityDestroyed(AbilityActor aActor)
        {
            m_SpawnedGameObject.Remove(aActor.gameObject);
        }
    }
}


