using Cysharp.Threading.Tasks;
using GlassBlower.Scripts.UI;
using UnityEngine;

namespace GlassBlower.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private UIController _uiController;
        [SerializeField] private ShapeMinigame _minigame;

        private void Awake()
        {
            _uiController.Initialize();
        }

        private void OnEnable()
        {
            _uiController.GameStarted += StartMinigame;
            _uiController.GameExit += ExitGame;
        }

        private void OnDisable()
        {
            _uiController.GameStarted -= StartMinigame;
            _uiController.GameExit -= ExitGame;
        }

        private void StartMinigame()
        {
            _minigame.StartGame().Forget();
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}