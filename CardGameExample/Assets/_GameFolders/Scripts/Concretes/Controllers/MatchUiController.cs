using System.Collections;
using CardGame.Managers;
using CardGame.Uis;

namespace CardGame.Controllers
{
    public class MatchUiController : BaseUiTextCounter
    {
        void OnEnable()
        {
           
            StartCoroutine(InitEvents());
        }

        void OnDisable()
        {
            GameManager.Instance.PlayerController.OnSuccessMatching -= HandleOnTextValueChanged;
            GameManager.Instance.OnGameStarted -= HandleOnGameStarted;
        }

        IEnumerator InitEvents()
        {
            while (GameManager.Instance == null) yield return null;
            GameManager.Instance.OnGameStarted += HandleOnGameStarted;

            while (GameManager.Instance.PlayerController == null) yield return null;
            GameManager.Instance.PlayerController.OnSuccessMatching += HandleOnTextValueChanged;
        }
        
        void HandleOnGameStarted()
        {
            _counterText.SetText("0");
        }
    }
}