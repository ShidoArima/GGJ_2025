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
            _uiController.GameFinish += OnFinish;
        }

        private void OnFinish()
        {
            Restart().Forget();
        }

        private void OnDisable()
        {
            _uiController.GameStarted -= StartMinigame;
            _uiController.GameExit -= ExitGame;
            _uiController.GameFinish += OnFinish;
        }

        private void StartMinigame()
        {
            _minigame.StartGame().Forget();
        }

        private async UniTaskVoid Restart()
        {
            await _minigame.StopGame();
            await _minigame.StartGame();
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}