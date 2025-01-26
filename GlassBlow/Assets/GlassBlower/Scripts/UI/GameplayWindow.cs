using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GlassBlower.Scripts.UI
{
    public class GameplayWindow : BaseWindow
    {
        [SerializeField] private InputActionReference _pauseAction;
        [SerializeField] private Button _finishButton;

        private void OnEnable()
        {
            _pauseAction.action.performed += Pause;
            _finishButton.onClick.AddListener(Finish);
        }

        private void OnDisable()
        {
            _pauseAction.action.performed -= Pause;
            _finishButton.onClick.RemoveListener(Finish);
        }

        private void Pause(InputAction.CallbackContext context)
        {
            Control.ShowPause().Forget();
        }

        private void Finish()
        {
            Control.Finish().Forget();
        }
    }
}