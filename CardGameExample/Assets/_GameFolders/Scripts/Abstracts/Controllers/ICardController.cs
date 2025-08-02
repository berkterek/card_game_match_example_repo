using CardGame.ScriptableObjects;

namespace CardGame.Abstracts.Controllers
{
    public interface ICardController
    {
        CardDataContainerSO CardDataContainer { get; }
        bool IsFront { get; }
        void SetDataContainer(CardDataContainerSO cardDataContainer);
        void RotateCard();
    }
}