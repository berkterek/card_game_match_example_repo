using CardGame.Abstracts.Controllers;
using CardGame.Abstracts.Handlers;
using CardGame.Abstracts.Inputs;
using CardGame.Handlers;
using CardGame.Inputs;
using UnityEngine;

namespace CardGame.Controllers
{
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] int _totalScore;
        [SerializeField] bool _canPlay = false;

        public IWorldPositionHandler WorldPositionHandler { get; private set; }
        public IInputReader InputReader { get; private set; }
        public Camera Camera { get; private set; }
        public int CurrentScore => _totalScore;
        public event System.Action<int> OnSuccessMatching;

        void Awake()
        {
            Camera = Camera.main;
            InputReader = new InputReader();
            WorldPositionHandler = new WorldPositionWithPhysicsHandler(this);
        }

        public void Update()
        {
            if (!InputReader.IsTouch || !_canPlay) return;

            var cardController = WorldPositionHandler.ExecuteGetWorldPosition();
            if (cardController == null) return;
        }

        public void PlayerCanPlay()
        {
            _canPlay = true;
        }
        
        public void PlayerCantPlay()
        {
            _canPlay = false;
        }

        public void ResetTotalValue()
        {
            _totalScore = 0;
        }

        void HandleOnSuccessMatching(int score)
        {
            _totalScore += score;
            OnSuccessMatching?.Invoke(_totalScore);
        }
    }
}