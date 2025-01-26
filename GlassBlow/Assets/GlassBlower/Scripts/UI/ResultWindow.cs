using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GlassBlower.Scripts.UI
{
    public class ResultWindow : BaseWindow
    {
        [SerializeField] private Button _continueButton;

        private void OnEnable()
        {
            _continueButton.onClick.AddListener(StartGame);
        }

        private void OnDisable()
        {
            _continueButton.onClick.RemoveListener(StartGame);
        }

        private void StartGame()
        {
            Control.StartGame().Forget();
        }
    }
}