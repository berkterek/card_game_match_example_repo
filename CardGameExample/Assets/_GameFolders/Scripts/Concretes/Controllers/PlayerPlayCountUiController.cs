using System.Collections;
using CardGame.Managers;
using CardGame.Uis;

namespace CardGame.Controllers
{
    public class PlayerPlayCountUiController : BaseUiTextCounter
    {
        void OnEnable()
        {
            StartCoroutine(InitEvents());
        }

        void OnDisable()
        {
            CardManager.Instance.OnPlayerPlayCount -= HandleOnTextValueChanged;
            GameManager.Instance.OnGameStarted -= HandleOnGameStarted;
        }

        IEnumerator InitEvents()
        {
            while (GameManager.Instance == null || CardManager.Instance == null) yield return null;
            
            CardManager.Instance.OnPlayerPlayCount += HandleOnTextValueChanged;
            GameManager.Instance.OnGameStarted += HandleOnGameStarted;
        }
        
        void HandleOnGameStarted()
        {
            _counterText.SetText("0");
        }
    }    
}