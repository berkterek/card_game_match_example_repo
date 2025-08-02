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
        [SerializeField] int _columns = 4;
        [SerializeField] int _rows = 4;
        [SerializeField] Vector2 _gridSpacing = new Vector2(0.1f, 0.1f);
        [SerializeField] Vector2 _gridTargetSize = new Vector2(10f, 10f);
        [SerializeField] float _screenMargin = 0.1f;
        [SerializeField] int _playerPlayCount;
        [SerializeField] int _comboStart = 1;
        [SerializeField] Transform _transform;
        [SerializeField] List<CardController> _cards;

        Queue<CardController> _firstCardControllers;
        int _currentCombo;
        DeckName _tempDeck;
        Camera _mainCamera;

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
            _mainCamera = Camera.main;
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
        
        public void SetGridLayout(int columns, int rows)
        {
            _columns = Mathf.Max(1, columns);
            _rows = Mathf.Max(1, rows);
        }

        public void CreateCards()
        {
            _playerPlayCount = 0;
            _currentCombo = _comboStart;
            GameManager.Instance.PlayerController.ResetTotalValue();
            CleanCards();
            int maxCount = _columns * _rows;
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
            
            float xOffset = (_gridTargetSize.x - (_columns + 1) * _gridSpacing.x) / _columns;
            float yOffset = (_gridTargetSize.y - (_rows + 1) * _gridSpacing.y) / _rows;
            float startX = -_gridTargetSize.x / 2f + xOffset / 2f + _gridSpacing.x;
            float startY = -_gridTargetSize.y / 2f + yOffset / 2f + _gridSpacing.y;

            int index = 0;
            for (int i = 0; i < _columns; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    var cardController = Instantiate(_prefab, _transform);
                    cardController.Transform.localPosition = new Vector3(startX + i * (xOffset + _gridSpacing.x),
                        startY + j * (yOffset + _gridSpacing.y), 0);
                    cardController.SetDataContainer(allDataContainers[index]);
                    _cards.Add(cardController);
                    index++;
                }
            }

            AdjustGridToScreen();
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

            int maxCount = _columns * _rows;
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

            AdjustGridToScreen();
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

        private void AdjustGridToScreen()
        {
            if (_cards.Count == 0 || _mainCamera == null) return;
            
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (var card in _cards)
            {
                Bounds bounds = card.GetBounds();
                minX = Mathf.Min(minX, bounds.min.x);
                maxX = Mathf.Max(maxX, bounds.max.x);
                minY = Mathf.Min(minY, bounds.min.y);
                maxY = Mathf.Max(maxY, bounds.max.y);
            }

            float gridWidth = maxX - minX;
            float gridHeight = maxY - minY;
            
            float aspect = (float)Screen.width / Screen.height;
            float viewHeight = 2f * _mainCamera.orthographicSize;
            float viewWidth = viewHeight * aspect;
            
            viewWidth *= (1f - _screenMargin);
            viewHeight *= (1f - _screenMargin);
            
            float scaleX = viewWidth / gridWidth;
            float scaleY = viewHeight / gridHeight;
            float scale = Mathf.Min(scaleX, scaleY);
            
            _transform.localScale = new Vector3(scale, scale, 1f);
            
            Vector3 gridCenter = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
            _mainCamera.transform.position = new Vector3(gridCenter.x, gridCenter.y, _mainCamera.transform.position.z);
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