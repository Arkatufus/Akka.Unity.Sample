using UnityEngine;

namespace Akka.Chat.Client
{
    public class Singleton<T> : MonoBehaviour where T:Component
    {
        public const string PersistentGameObjectName = "Persistent Container";

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var gameObject = GameObject.Find(PersistentGameObjectName) ?? new GameObject(PersistentGameObjectName);
                        DontDestroyOnLoad(gameObject);
                        _instance = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
                _instance = this as T;
            else if(_instance != this)
                Destroy(gameObject);
        }
    }
}
