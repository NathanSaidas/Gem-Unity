using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added Actor Class
 *
 */
#endregion

namespace Gem
{
    public class Actor : MonoBehaviour , IActor
    {
        private ActorIdentifier m_ActorType = ActorIdentifier.None;
        private bool m_IsObjectAlive = true;
        private bool m_IsInitialized = false;
        private bool m_IsFinalized = false;

        public string GetActorName()
        {
            return gameObject.name;
        }
        public ActorIdentifier GetActorType()
        {
            return m_ActorType;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        public bool IsObjectAlive()
        {
            return m_IsObjectAlive;
        }
        public void Destroy()
        {
            if(m_IsObjectAlive)
            {
                ActorManager.Destroy(this);
            }
            m_IsObjectAlive = false;
        }
        public static void Destroy(IActor aActor)
        {
            if(aActor != null && aActor.IsObjectAlive())
            {
                ActorManager.Destroy(aActor);
            }
        }
        public void InitializeActor()
        {
            if(!m_IsInitialized)
            {
                m_IsInitialized = true;
                OnInitializeActor();
            }
        }
        public void FinalizeActor()
        {
            if(!m_IsFinalized)
            {
                m_IsFinalized = true;
                OnFinalizeActor();
            }
        }

        public virtual void OnInitializeActor()
        {

        }
        public virtual void OnFinalizeActor()
        {

        }



        

        
    }
}