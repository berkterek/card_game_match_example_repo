using System.Collections;
using CardGame.Abstracts.Controllers;
using CardGame.Abstracts.Helpers;
using UnityEngine;

namespace CardGame.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        const string SAVE_LOAD_KEY = "game_manager_save_load_key"; 
        
        [SerializeField] int _bestScore = 0;
        [SerializeField] int _currentScore;
        [SerializeField] int _delaySecond = 3;
        
        IPlayerController _playerController;
        //ISaveLoadService _saveLoadService;

        public event System.Action<int,int> OnGameEnded;
        public event System.Action OnGameStarted;
        public event System.Action OnReturnMenu;
        
        void Constructor()
        {
            // _saveLoadService = saveLoadService;
            // if (_saveLoadService.HasKeyAvailable(SAVE_LOAD_KEY))
            // {
            //     var model = _saveLoadService.LoadDataProcess<GameManagerSaveModel>(SAVE_LOAD_KEY);
            //     _bestScore = model.BestScore;
            // }
        }

        void OnEnable()
        {
            StartCoroutine(InitCoroutine());
        }

        void OnDisable()
        {
            CardManager.Instance.OnGameOvered -= HandleOnGameOvered;
        }

        IEnumerator InitCoroutine()
        {
            while (CardManager.Instance == null) yield return null;
            CardManager.Instance.OnGameOvered += HandleOnGameOvered;
        }

        public void Init(IPlayerController playerController)
        {
            _playerController = playerController;
        }

        public IEnumerator GameStart()
        {
            OnGameStarted?.Invoke();
            CardManager.Instance.CreateCards();
            yield return new WaitForSeconds(_delaySecond);
            yield return StartCoroutine(CardManager.Instance.FlipAllCardsCoroutine());
            _playerController.PlayerCanPlay();
        }

        public IEnumerator LoadLastGame()
        {
            OnGameStarted?.Invoke();
            CardManager.Instance.LoadLastGameCards();
            yield return new WaitForSeconds(_delaySecond);
            yield return StartCoroutine(CardManager.Instance.FlipAllCardsCoroutine());
            _playerController.PlayerCanPlay();
        }

        public void ReturnMenu()
        {
            _playerController.PlayerCantPlay();
            OnReturnMenu?.Invoke();
        }
        
        void HandleOnGameOvered()
        {
            if (_bestScore < _playerController.CurrentScore)
            {
                _bestScore = _playerController.CurrentScore;
                // _saveLoadService.SaveDataProcess(SAVE_LOAD_KEY, new GameManagerSaveModel()
                // {
                //     BestScore = _bestScore
                // });
            }
            
            _currentScore = _playerController.CurrentScore;
            _playerController.PlayerCantPlay();
            
            OnGameEnded?.Invoke(_currentScore, _bestScore);
        }
    }

    public struct GameManagerSaveModel
    {
        public int BestScore;
    }
}

