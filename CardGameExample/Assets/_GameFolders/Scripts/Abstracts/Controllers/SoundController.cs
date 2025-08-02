using CardGame.Helpers;
using UnityEngine;

namespace CardGame.Abstracts.Controllers
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundController : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;

        void OnValidate()
        {
            this.GetReference(ref _audioSource);
        }

        public void Play()
        {
            _audioSource.Play();
        }
    }
}