using System.Collections;
using CardGame.Abstracts.Controllers;
using CardGame.Helpers;
using CardGame.ScriptableObjects;
using UnityEngine;

namespace CardGame.Controllers
{
   public class CardController : MonoBehaviour, ICardController
    {
        [SerializeField] SpriteRenderer _bodySpriteRenderer;
        [SerializeField] Transform _transform;
        [SerializeField] bool _isFront = true;

        public CardDataContainerSO CardDataContainer { get; private set; }
        public bool IsFront => _isFront;
        public Transform Transform => _transform;

        void OnValidate()
        {
            this.GetReference<Transform>(ref _transform);
        }

        public void SetDataContainer(CardDataContainerSO cardDataContainer)
        {
            CardDataContainer = cardDataContainer;
            _bodySpriteRenderer.sprite = cardDataContainer.CardSprite;
        }
        
        public void RotateCard()
        {
            _isFront = !_isFront;

            if (_isFront)
            {
                StartCoroutine(FlipToFront());
            }
            else
            {
                StartCoroutine(FlipToBack());
            }
        }

        IEnumerator FlipToFront()
        {
            yield return StartCoroutine(RotateTo(new Vector3(0f, 90f, 0f), CardDataContainer.CardStats.RotationDuration));
            _bodySpriteRenderer.sprite = CardDataContainer.CardSprite;
            yield return StartCoroutine(RotateTo(new Vector3(0f, 180f, 0f), CardDataContainer.CardStats.RotationDuration));
        }

        IEnumerator FlipToBack()
        {
            yield return StartCoroutine(RotateTo(new Vector3(0f, 90f, 0f), CardDataContainer.CardStats.RotationDuration));
            _bodySpriteRenderer.sprite = null;
            yield return StartCoroutine(RotateTo(new Vector3(0f, 0f, 0f), CardDataContainer.CardStats.RotationDuration));
        }

        IEnumerator RotateTo(Vector3 targetRotation, float duration)
        {
            Quaternion startRotation = _transform.rotation;
            Quaternion endRotation = Quaternion.Euler(targetRotation);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                _transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return null;
            }

            _transform.rotation = endRotation;
        }
    }
}