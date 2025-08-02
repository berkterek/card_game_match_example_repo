using TMPro;
using UnityEngine;

namespace CardGame.Uis
{
    public abstract class BaseUiTextCounter : MonoBehaviour
    {
        [SerializeField] protected TMP_Text _counterText;

        protected void HandleOnTextValueChanged(int counter)
        {
            _counterText.SetText(counter.ToString());
        }
    }   
}