using CardGame.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Abstracts.Uis
{
    [RequireComponent(typeof(Button))]
    public abstract class BaseButton : MonoBehaviour
    {
        [SerializeField] protected Button _button;

        protected virtual void OnValidate()
        {
            this.GetReference(ref _button);
        }

        protected virtual void OnEnable()
        {
            _button.onClick.AddListener(HandleOnButtonClicked);
        }

        protected virtual void OnDisable()
        {
            _button.onClick.RemoveListener(HandleOnButtonClicked);
        }

        protected abstract void HandleOnButtonClicked();
    }
}