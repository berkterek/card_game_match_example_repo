using UnityEngine;

namespace CardGame.Helpers
{
    public static class MonoExtensions
    {
        public static void GetReference<T>(this MonoBehaviour mono, ref T value)
        {
            if (value == null)
            {
                value = mono.GetComponentInChildren<T>();
            }
        }

        public static void GetReferenceParent<T>(this MonoBehaviour mono, ref T value)
        {
            if (value == null)
            {
                if (mono.transform.parent == null) return;

                value = mono.transform.parent.GetComponent<T>();
            }
        }
    }
}