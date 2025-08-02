using CardGame.Abstracts.Uis;
using CardGame.Managers;

namespace CardGame.Uis
{
    public class MenuButton : BaseButton
    {
        protected override void HandleOnButtonClicked()
        {
            GameManager.Instance.ReturnMenu();
        }
    }   
}