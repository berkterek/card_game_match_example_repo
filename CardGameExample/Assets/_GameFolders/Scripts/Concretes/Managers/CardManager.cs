using System.Collections.Generic;
using System.Linq;
using CardGame.Abstracts.Controllers;
using CardGame.Controllers;
using CardGame.Enums;
using CardGame.Helpers;
using CardGame.ScriptableObjects;
using UnityEngine;
using System.Collections;
using CardGame.Abstracts.Helpers;
using Random = UnityEngine.Random;

namespace CardGame.Managers
{
    public class CardManager : MonoSingleton<CardManager>
    {
        [SerializeField] CardController _prefab;
        [SerializeField] DeckValue[] _decks;
        [SerializeField] int _xLoopCount = 4;
        [SerializeField] int _yLoopCount = 4;
        [SerializeField] float _xOffset;
        [SerializeField] float _yOffset;
        [SerializeField] int _playerPlayCount;
        [SerializeField] int _comboStart = 1;
        [SerializeField] Transform _transform;
        [SerializeField] List<CardController> _cards;

        Queue<CardController> _firstCardControllers;
        int _currentCombo;
        DeckName _tempDeck;

        public event System.Action<int> OnSuccessMatching;
        public event System.Action<int> OnPlayerPlayCount;
        public event System.Action OnGameOvered;

        void OnValidate()
        {
            this.GetReference(ref _transform);
        }

        protected override void Awake()
        {
            base.Awake();
            _firstCardControllers = new Queue<CardController>();
        }

        void OnEnable()
        {
            StartCoroutine(InitEvents());
        }

        void OnDisable()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnReturnMenu -= HandleOnReturnMenu;
        }

        IEnumerator InitEvents()
        {
            while (GameManager.Instance == null) yield return null;
            GameManager.Instance.OnReturnMenu += HandleOnReturnMenu;
        }

        public void CreateCards()
        {
            _playerPlayCount = 0;
            _currentCombo = _comboStart;
            GameManager.Instance.PlayerController.ResetTotalValue();
            CleanCards();
            int maxCount = _xLoopCount * _yLoopCount;
            _cards = new List<CardController>(maxCount);

            int randomDeck = Random.Range(0, (int)DeckName.Deck3 + 1);
            _tempDeck = (DeckName)randomDeck;
            var deckValue = _decks.FirstOrDefault(x => x.DeckName == _tempDeck);
            if (deckValue == null)
            {
                deckValue = _decks.FirstOrDefault(x => x.DeckName == DeckName.Deck1);
                _tempDeck = DeckName.Deck1;
            }

            var dataContainers1 = deckValue.GetCardDataContainers(maxCount / 2);
            var dataContainers2 = dataContainers1.ToArray();

            List<CardDataContainerSO> allDataContainers = new List<CardDataContainerSO>();
            AddList(dataContainers1, allDataContainers);
            AddList(dataContainers2, allDataContainers);

            allDataContainers.Shuffle();

            int index = 0;
            for (int i = 0; i < _xLoopCount; i++)
            {
                for (int j = 0; j < _yLoopCount; j++)
                {
                    var cardController = Instantiate(_prefab, _transform);
                    cardController.Transform.localPosition = new Vector3(i * _xOffset, j * _yOffset, 0);
                    cardController.SetDataContainer(allDataContainers[index]);
                    _cards.Add(cardController);
                    index++;
                }
            }
        }

        public void LoadLastGameCards()
        {
            var saveLoadManager = SaveLoadManager.Singleton();
            if (!saveLoadManager.HasKeyAvailable(ConstHelper.CARD_MANAGER_KEY)) return;

            var model = saveLoadManager.LoadDataProcess<DeckDataModel>(ConstHelper.CARD_MANAGER_KEY);

            _playerPlayCount = model.PlayerPlayCount;
            OnPlayerPlayCount?.Invoke(_playerPlayCount);
            _currentCombo = model.CurrentCombo;
            GameManager.Instance.PlayerController.ResetTotalValue();
            var score = model.CurrentScore;
            OnSuccessMatching?.Invoke(score);

            CleanCards();

            int maxCount = _xLoopCount * _yLoopCount;
            _cards = new List<CardController>(maxCount);


            _tempDeck = model.DeckName;
            var deckValue = _decks.FirstOrDefault(x => x.DeckName == _tempDeck);

            List<CardDataContainerSO> loadedList = new List<CardDataContainerSO>();

            for (int i = 0; i < model.CardDataModels.Count; i++)
            {
                loadedList.Add(deckValue.GetDataContainerByType(model.CardDataModels[i].CardType));
            }

            for (int i = 0; i < loadedList.Count; i++)
            {
                var cardController = Instantiate(_prefab, _transform);
                cardController.Transform.localPosition = new Vector3(model.CardDataModels[i].XPosition,
                    model.CardDataModels[i].YPosition, 0f);
                cardController.SetDataContainer(loadedList[i]);
                _cards.Add(cardController);
            }
        }

        void CleanCards()
        {
            _cards = null;
            while (_transform.childCount > 0)
            {
                DestroyImmediate(_transform.GetChild(0).gameObject);
            }
        }

        void AddList(CardDataContainerSO[] dataContainers, List<CardDataContainerSO> list)
        {
            foreach (var cardDataContainer in dataContainers)
            {
                list.Add(cardDataContainer);
            }
        }

        public IEnumerator FlipAllCardsCoroutine()
        {
            foreach (var cardController in _cards)
            {
                cardController.RotateCard();
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void MatchCards(ICardController cardController)
        {
            StartCoroutine(MatchCardsCoroutine(cardController));
        }

        private IEnumerator MatchCardsCoroutine(ICardController cardController)
        {
            if (_firstCardControllers.Count <= 0)
            {
                _firstCardControllers.Enqueue(cardController as CardController);
                cardController.RotateCard();
                SoundManager.Instance.Play(SoundType.Flip);
            }
            else
            {
                var secondCardController = cardController as CardController;
                secondCardController.RotateCard();
                SoundManager.Instance.Play(SoundType.Flip);
                var firstCard = _firstCardControllers.Dequeue();

                if (firstCard.CardDataContainer.CardType == secondCardController.CardDataContainer.CardType)
                {
                    _cards.Remove(firstCard);
                    _cards.Remove(secondCardController);
                    yield return new WaitForSeconds(1f);
                    OnSuccessMatching?.Invoke(firstCard.CardDataContainer.CardScore * _currentCombo);
                    Destroy(firstCard.gameObject);
                    Destroy(secondCardController.gameObject);
                    _currentCombo++;
                    SoundManager.Instance.Play(SoundType.Success);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    firstCard.RotateCard();
                    secondCardController.RotateCard();
                    _currentCombo = _comboStart;
                    SoundManager.Instance.Play(SoundType.Failed);
                }

                _playerPlayCount++;
                OnPlayerPlayCount?.Invoke(_playerPlayCount);

                if (_cards.Count <= 0)
                {
                    var saveLoadManager = SaveLoadManager.Singleton();
                    if (saveLoadManager.HasKeyAvailable(ConstHelper.CARD_MANAGER_KEY))
                    {
                        saveLoadManager.DeleteData(ConstHelper.CARD_MANAGER_KEY);
                    }

                    SoundManager.Instance.Play(SoundType.Finished);
                    OnGameOvered?.Invoke();
                }
            }
        }

        void HandleOnReturnMenu()
        {
            if (_cards.Count > 0)
            {
                DeckDataModel model = new DeckDataModel();
                model.DeckName = _tempDeck;
                model.CurrentCombo = _currentCombo;
                model.PlayerPlayCount = _playerPlayCount;
                model.CurrentScore = GameManager.Instance.PlayerController.CurrentScore;
                model.CardDataModels = new List<CardDataModel>();
                foreach (CardController cardController in _cards)
                {
                    model.CardDataModels.Add(new CardDataModel()
                    {
                        CardType = cardController.CardDataContainer.CardType,
                        XPosition = cardController.Transform.localPosition.x,
                        YPosition = cardController.Transform.localPosition.y,
                    });
                }

                var saveLoadManager = SaveLoadManager.Singleton();
                saveLoadManager.SaveDataProcess(ConstHelper.CARD_MANAGER_KEY, model);
            }
        }
    }

    [System.Serializable]
    public class DeckValue
    {
        public DeckName DeckName;
        [SerializeField] CardDataContainerSO[] _cardDataContainers;

        public CardDataContainerSO[] CardDataContainers { get; set; }

        public CardDataContainerSO[] GetCardDataContainers(int count)
        {
            if (CardDataContainers == null) CardDataContainers = _cardDataContainers;

            if (CardDataContainers.Length < count) count = Mathf.Max(CardDataContainers.Length - 1, 0);

            List<CardDataContainerSO> tempList = new List<CardDataContainerSO>(count);

            while (tempList.Count < count)
            {
                var randomIndex = Random.Range(0, count);
                var cardData = CardDataContainers[randomIndex];

                while (tempList.Contains(cardData))
                {
                    randomIndex = Random.Range(0, count);
                    cardData = CardDataContainers[randomIndex];
                }

                tempList.Add(cardData);
            }

            return tempList.ToArray();
        }

        public CardDataContainerSO GetDataContainerByType(CardType cardType)
        {
            return _cardDataContainers.FirstOrDefault(x => x.CardType == cardType);
        }
    }

    public class DeckDataModel
    {
        public DeckName DeckName;
        public int PlayerPlayCount;
        public int CurrentCombo;
        public int CurrentScore;
        public List<CardDataModel> CardDataModels;
    }

    public class CardDataModel
    {
        public float XPosition;
        public float YPosition;
        public CardType CardType;
    }
}