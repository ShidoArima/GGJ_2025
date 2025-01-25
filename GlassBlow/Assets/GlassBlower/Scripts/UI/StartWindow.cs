using UnityEngine;
using UnityEngine.UI;

namespace GlassBlower.Scripts.UI
{
    public class StartWindow : BaseWindow
    {
        [SerializeField] private Button _startButton;

        private void OnEnable()
        {
            _startButton.onClick.AddListener(StartGame);
        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(StartGame);
        }

        private void StartGame()
        {
            Control.StartGame().Forget();
        }
    }
}