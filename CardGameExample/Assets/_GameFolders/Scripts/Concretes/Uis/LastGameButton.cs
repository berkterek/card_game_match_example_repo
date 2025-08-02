using System.Collections;
using CardGame.Abstracts.Uis;
using CardGame.Helpers;
using CardGame.Managers;

namespace CardGame.Uis
{
    public class LastGameButton : BaseButton
    {
        void Start()
        {
            HandleOnReturnMenu();
        }

        protected override void OnEnable()
        {
            StartCoroutine(InitEvents());
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            GameManager.Instance.OnReturnMenu -= HandleOnReturnMenu;
            base.OnDisable();
        }

        IEnumerator InitEvents()
        {
            while (GameManager.Instance == null) yield return null;
            GameManager.Instance.OnReturnMenu += HandleOnReturnMenu;
        }

        void HandleOnReturnMenu()
        {
            var saveLoadManager = SaveLoadManager.Singleton();
            _button.interactable = saveLoadManager.HasKeyAvailable(ConstHelper.CARD_MANAGER_KEY);   
        }

        protected override void HandleOnButtonClicked()
        {
            StartCoroutine(GameManager.Instance.LoadLastGameCoroutine());
        }
    }   
}