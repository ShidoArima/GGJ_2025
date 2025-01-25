using UnityEngine;
using UnityEngine.UI;

namespace GlassBlower.Scripts.UI
{
    public class PauseWindow : BaseWindow
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;

        private void OnEnable()
        {
            _restartButton.onClick.AddListener(Restart);
            _exitButton.onClick.AddListener(Exit);
        }

        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(Restart);
            _exitButton.onClick.RemoveListener(Exit);
        }

        private void Restart()
        {
            Control.StartGame().Forget();
        }

        private void Exit()
        {
            Control.ExitGame();
        }
    }
}