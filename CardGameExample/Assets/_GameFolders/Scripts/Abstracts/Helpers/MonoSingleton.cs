using UnityEngine;

namespace CardGame.Abstracts.Helpers
{
    public  abstract class MonoSingleton<T> : MonoBehaviour 
        where T : MonoBehaviour, new() 
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                transform.parent = null;
                Instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}