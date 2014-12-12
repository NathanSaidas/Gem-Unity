using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Gem
{
    public class World : MonoBehaviour
    {
        #region SINGLETON
        private static World s_Instance = null;
        private static World instance
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
                    persistent = new GameObject(Constants.GAME_OBJECT_PERSISTENT);
                    persistent.transform.position = Vector3.zero;
                    persistent.transform.rotation = Quaternion.identity;
                }
            }
            if (persistent != null)
            {
                s_Instance = persistent.GetComponent<World>();
                if (s_Instance == null && Application.isPlaying)
                {
                    s_Instance = persistent.AddComponent<World>();
                }
            }
        }
        private static bool SetInstance(World aInstance)
        {
            if (s_Instance != null && s_Instance != aInstance)
            {
                return false;
            }
            s_Instance = aInstance;
            return true;
        }
        private static void DestroyInstance(World aInstance)
        {
            if (s_Instance == aInstance)
            {
                s_Instance = null;
            }
        }
        #endregion

        /// <summary>
        /// Sub Collectinos for each base type of actor
        /// </summary>
        private HashSet<Unit> m_UnitActors = new HashSet<Unit>();
        private HashSet<Interactive> m_InteractiveActors = new HashSet<Interactive>();
        private HashSet<StaticActor> m_StaticActors = new HashSet<StaticActor>();
        private HashSet<Actor> m_DefaultActors = new HashSet<Actor>();

        private HashSet<Actor> m_AllActors = new HashSet<Actor>();

        private bool m_IsPaused = false;
        private bool m_IsLoading = false;
        private bool m_IsUnloading = false;

        
        void Start()
        {
            if(!SetInstance(this))
            {
                Utilities.Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        void OnDestroy()
        {
            DestroyInstance(this);
        }

        #region REGISTRATION

        public static void Register(Actor aActor)
        {
            if(instance != null)
            {
                instance.RegisterWorld(aActor);
            }
        }

        void RegisterWorld(Actor aActor)
        {
            if(aActor == null)
            {
                return;
            }
            if(aActor is Unit)
            {
                m_UnitActors.Add((Unit)aActor);
            }
            else if(aActor is Interactive)
            {
                m_InteractiveActors.Add((Interactive)aActor);
            }
            else if(aActor is StaticActor)
            {
                m_StaticActors.Add((StaticActor)aActor);
            }
            else
            {
                m_DefaultActors.Add(aActor);
            }
            m_AllActors.Add(aActor);
        }

        public static void Unregister(Actor aActor)
        {
            if(instance != null)
            {
                instance.UnregisterWorld(aActor);
            }
        }
        void UnregisterWorld(Actor aActor)
        {
            if(aActor == null)
            {
                return;
            }
            if (aActor is Unit)
            {
                m_UnitActors.Remove((Unit)aActor);
            }
            else if (aActor is Interactive)
            {
                m_InteractiveActors.Remove((Interactive)aActor);
            }
            else if (aActor is StaticActor)
            {
                m_StaticActors.Remove((StaticActor)aActor);
            }
            else
            {
                m_DefaultActors.Remove(aActor);
            }
            m_AllActors.Remove(aActor);
        }

        #endregion

        #region FIND HELPERS

        public static Actor Find(string aName)
        {
            if(instance == null)
            {
                return null;
            }
            return instance.m_AllActors.FirstOrDefault<Actor>(Element => Element.actorName == aName);
        }
        public static Actor Find(Guid aID)
        {
            if (instance == null)
            {
                return null;
            }
            string id = aID.ToString();
            return instance.m_AllActors.FirstOrDefault<Actor>(Element => Element.uniqueID == id);
        }
        public static IEnumerable<Actor> FindAll(string aName)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_AllActors.Where<Actor>(Element => Element.actorName == aName);
        }
        public static IEnumerable<Actor> FindAll(ActorIdentifier aIdentifier)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_AllActors.Where<Actor>(Element => Element.identifier == aIdentifier);
        }
        public static IEnumerable<Actor> FindAll(Player aPlayer)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_AllActors.Where<Actor>(Element => Element.player == aPlayer);
        }
        public static T Find<T>(string aName) where T : Actor
        {
            if(instance == null)
            {
                return null;
            }
            return instance.m_AllActors.FirstOrDefault<Actor>(Element => (Element.actorName == aName && Element is T)) as T;
        }
        public static T Find<T>(Guid aID) where T : Actor
        {
            if (instance == null)
            {
                return null;
            }
            string id = aID.ToString();
            return instance.m_AllActors.FirstOrDefault<Actor>(Element => (Element.uniqueID == id && Element is T)) as T;
        }
        public static IEnumerable<T> FindAll<T>(string aName) where T : Actor
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_AllActors.Where<Actor>(Element => (Element.actorName == aName && Element is T)) as IEnumerable<T>;
        }
        public static IEnumerable<T> FindAll<T>(ActorIdentifier aIdentifier) where T: Actor
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_AllActors.Where<Actor>(Element => (Element.identifier == aIdentifier && Element is T)) as IEnumerable<T>;
        }
        public static IEnumerable<T> FindAll<T>(Player aPlayer) where T : Actor
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_AllActors.Where<Actor>(Element => (Element.player == aPlayer && Element is T)) as IEnumerable<T>;
        }


        #endregion

        #region CUSTOM SEARCH METHODS
        public static Actor SearchActors(Func<Actor,bool> aPredicate)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_DefaultActors.FirstOrDefault(aPredicate);
        }
        public static Unit SearchUnits(Func<Unit, bool> aPredicate)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_UnitActors.FirstOrDefault(aPredicate);
        }
        public static Interactive SearchInteractive(Func<Interactive, bool> aPredicate)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_InteractiveActors.FirstOrDefault(aPredicate);
        }
        public static StaticActor SearchStaticActor(Func<StaticActor, bool> aPredicate)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_StaticActors.FirstOrDefault(aPredicate);
        }
        public static IEnumerable<Actor> SearchAllActors(Func<Actor,bool> aPredicate)
        {
            if(instance == null)
            {
                return null;
            }
            return instance.m_DefaultActors.Where(aPredicate);
        }
        public static IEnumerable<Unit> SearchAllUnits(Func<Unit, bool> aPredicate)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_UnitActors.Where(aPredicate);
        }
        public static IEnumerable<Interactive> SearchAllInteractive(Func<Interactive, bool> aPredicate)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_InteractiveActors.Where(aPredicate);
        }
        public static IEnumerable<StaticActor> SearchAllStaticActors(Func<StaticActor, bool> aPredicate)
        {
            if (instance == null)
            {
                return null;
            }
            return instance.m_StaticActors.Where(aPredicate);
        }

#endregion

        public static Actor SpawnActor(GameObject aPrefab)
        {
            GameObject gameObj = Instantiate(aPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            Actor actor = gameObj.GetComponent<Actor>();
            if(actor == null)
            {
                Utilities.Destroy(gameObj);
            }
            return actor;
        }
        public static T SpawnActor<T>() where T : Actor
        {
            GameObject gameObj = new GameObject("Actor");
            gameObj.transform.position = Vector3.zero;
            gameObj.transform.rotation = Quaternion.identity;
            return gameObj.AddComponent<T>();
        }

        public static bool isPaused
        {
            get { return instance == null ? false : instance.m_IsPaused; }
        }
        public static bool isLoading
        {
            get { return instance == null ? false : instance.m_IsLoading; }
        }
        public static bool isUnloading
        {
            get { return instance == null ? false : instance.m_IsUnloading; }
        }
    }
}