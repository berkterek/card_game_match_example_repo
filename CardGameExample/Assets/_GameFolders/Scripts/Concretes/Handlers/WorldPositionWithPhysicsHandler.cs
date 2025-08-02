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
            Vector2 worldPoint = _playerController.Camera.ScreenToWorldPoint(_playerController.InputReader.TouchPosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 1f, _playerController.LayerMask);
            if(hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out ICardController cardController))
                {
                    if (cardController.IsFront) return null;
                    
                    return cardController;
                }
            }

            return null;
        }
    }
}