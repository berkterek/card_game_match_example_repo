using System.Collections;
using CardGame.Handlers;
using CardGame.Helpers;
using CardGame.Managers;
using UnityEngine;

namespace CardGame.Controllers
{
    public class MenuCanvasController : MonoBehaviour
    {
        [SerializeField] CanvasGroupHandler _canvasGroupHandler;

        void OnEnable()
        {
            StartCoroutine(InitEvents());
        }

        void OnDisable()
        {
            GameManager.Instance.OnReturnMenu -= HandleOnReturnMenu;
        }

        IEnumerator InitEvents()
        {
            while (GameManager.Instance == null) yield return null;
            GameManager.Instance.OnReturnMenu += HandleOnReturnMenu;
        }

        void OnValidate() => this.GetReference(ref _canvasGroupHandler);
        void ActiveCanvasGroup() => _canvasGroupHandler.FadeIn();
        public void DeactivateCanvasGroup() => _canvasGroupHandler.FadeOut();
        void HandleOnReturnMenu() => ActiveCanvasGroup();
    }
}