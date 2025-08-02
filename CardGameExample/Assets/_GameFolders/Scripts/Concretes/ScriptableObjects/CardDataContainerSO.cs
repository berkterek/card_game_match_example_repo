using CardGame.Enums;
using UnityEngine;

namespace CardGame.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Card Data Container", menuName = "Terek Gaming/Data Container/Card Data Container")]
    public class CardDataContainerSO : ScriptableObject
    {
        [SerializeField] CardType _cardType;
        [SerializeField] int _cardScore;
        [SerializeField] CardStatsSO _cardStats;
        [SerializeField] Sprite _cardSprite;
        
        public CardType CardType => _cardType;
        public Sprite CardSprite => _cardSprite;
        public int CardScore => _cardScore;
        public CardStatsSO CardStats => _cardStats;
    }
}