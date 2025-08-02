using System.Collections;
using CardGame.Handlers;
using CardGame.Helpers;
using CardGame.Managers;
using TMPro;
using UnityEngine;

namespace CardGame.Controllers
{
    public class GameOverPanelController : MonoBehaviour
    {
        [SerializeField] CanvasGroupHandler _canvasGroupHandler;
        [SerializeField] TMP_Text _bestScoreText;
        [SerializeField] TMP_Text _currentScoreText;

        void OnValidate()
        {
            this.GetReference(ref _canvasGroupHandler);
        }

        void OnEnable()
        {
            StartCoroutine(InitEvents());
        }

        void OnDisable()
        {
            GameManager.Instance.OnGameEnded -= HandleOnGameEnded;
            GameManager.Instance.OnGameStarted -= HandleOnGameStarted;
        }
        
        IEnumerator InitEvents()
        {
            while (GameManager.Instance == null) yield return null;
           
            GameManager.Instance.OnGameEnded += HandleOnGameEnded;
            GameManager.Instance.OnGameStarted += HandleOnGameStarted;
        }

        void HandleOnGameEnded(int currentScore, int bestScore)
        {
            _currentScoreText.SetText(currentScore.ToString());
            _bestScoreText.SetText(bestScore.ToString());
            _canvasGroupHandler.FadeIn();
        }
        
        void HandleOnGameStarted()
        {
            _currentScoreText.SetText("0");
            _bestScoreText.SetText("0");
            _canvasGroupHandler.FadeOut();
        }
    }   
}