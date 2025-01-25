using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlassBlower.Scripts.UI
{
    public class GameplayWindow : BaseWindow
    {
        [SerializeField] private InputActionReference _pauseAction;

        private void OnEnable()
        {
            _pauseAction.action.performed += Pause;
        }

        private void OnDisable()
        {
            _pauseAction.action.performed -= Pause;
        }

        private void Pause(InputAction.CallbackContext context)
        {
            Control.ShowPause().Forget();
        }
    }
}