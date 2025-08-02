using UnityEngine;

namespace CardGame.Abstracts.Inputs
{
    public interface IInputReader
    {
        public Vector2 TouchPosition { get; }
        public bool IsTouch { get; }
    }
}