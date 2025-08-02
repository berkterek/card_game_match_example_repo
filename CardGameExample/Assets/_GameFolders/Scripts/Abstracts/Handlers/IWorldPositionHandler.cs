using CardGame.Abstracts.Controllers;

namespace CardGame.Abstracts.Handlers
{
    public interface IWorldPositionHandler
    {
        ICardController ExecuteGetWorldPosition();
    }
}