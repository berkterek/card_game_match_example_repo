using CardGame.Abstracts.Controllers;
using CardGame.Abstracts.Handlers;
using UnityEngine;

namespace CardGame.Handlers
{
    public class WorldPositionWithPhysicsHandler : IWorldPositionHandler
    {
        readonly IPlayerController _playerController;

        public WorldPositionWithPhysicsHandler(IPlayerController playerController)
        {
            _playerController = playerController;
        }
        
        public ICardController ExecuteGetWorldPosition()
        {
            Vector3 worldPosition = _playerController.Camera.ScreenToWorldPoint(_playerController.InputReader.TouchPosition);
            worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

            var raycastHit = Physics2D.Raycast(worldPosition, Vector2.zero, 1f);
            if(raycastHit.collider != null)
            {
                if (raycastHit.collider.TryGetComponent(out ICardController cardController))
                {
                    if (cardController.IsFront) return null;
                    
                    return cardController;
                }
            }

            return null;
        }
    }
}