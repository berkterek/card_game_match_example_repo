using System.Collections.Generic;
using CardGame.Abstracts.Controllers;
using CardGame.Abstracts.Helpers;
using CardGame.Enums;
using UnityEngine;

namespace CardGame.Managers
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] SoundInspectorHolder[] _sounds;
        
        Dictionary<SoundType, SoundController> _soundDictionary;

        protected override void Awake()
        {
            base.Awake();
            _soundDictionary = new Dictionary<SoundType, SoundController>();
            foreach (var soundInspectorHolder in _sounds)
            {
                _soundDictionary.Add(soundInspectorHolder.SoundType,soundInspectorHolder.SoundController);
            }
        }

        public void Play(SoundType soundType)
        {
            if (!_soundDictionary.ContainsKey(soundType)) return;
            
            _soundDictionary[soundType].Play();
        }
    }

    [System.Serializable]
    public class SoundInspectorHolder
    {
        public SoundType SoundType;
        public SoundController SoundController;
    }
}