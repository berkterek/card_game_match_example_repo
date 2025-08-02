using CardGame.Abstracts.Uis;
using CardGame.Managers;

namespace CardGame.Uis
{
    public class StartButton : BaseButton
    {
        protected override void HandleOnButtonClicked()
        {
            StartCoroutine(GameManager.Instance.GameStartCoroutine());
        }
    }   
}