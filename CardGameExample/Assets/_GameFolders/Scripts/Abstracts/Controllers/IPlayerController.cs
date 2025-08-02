using CardGame.Abstracts.Inputs;
using UnityEngine;

namespace CardGame.Abstracts.Controllers
{
    public interface IPlayerController
    {
        IInputReader InputReader { get; }
        Camera Camera { get; }
        LayerMask LayerMask { get; }
        int CurrentScore { get; }
        void ResetTotalValue();
    }
}