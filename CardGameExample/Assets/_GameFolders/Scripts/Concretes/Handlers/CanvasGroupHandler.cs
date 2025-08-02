using System.Collections;
using CardGame.Helpers;
using UnityEngine;

namespace CardGame.Handlers
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupHandler : MonoBehaviour
    {
        [SerializeField] float _duration = 0.5f;
        [SerializeField] CanvasGroup _canvasGroup;

        void OnValidate()
        {
            this.GetReference(ref _canvasGroup);
        }

        public void FadeIn()
        {
            StartCoroutine(FadeCoroutine(1f, true));
        }

        public void FadeOut()
        {
            StartCoroutine(FadeCoroutine(0f, false));
        }

        private IEnumerator FadeCoroutine(float targetAlpha, bool isActive)
        {
            float startAlpha = _canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / _duration;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;
        }
    }
}