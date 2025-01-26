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
            _uiController.GameStarted += StartGame;
            _uiController.GameExit += ExitGame;
            _uiController.GameFinish += OnFinish;
        }


        private void OnDisable()
        {
            _uiController.GameStarted -= StartGame;
            _uiController.GameExit -= ExitGame;
            _uiController.GameFinish += OnFinish;
        }

        private void StartGame()
        {
            StartGameAsync().Forget();
        }

        private async UniTaskVoid StartGameAsync()
        {
            _uiController.DisableInput();
            await _minigame.StartGame();
            _uiController.EnableInput();
        }

        private void OnFinish()
        {
            StopGameAsync().Forget();
        }

        private async UniTaskVoid StopGameAsync()
        {
            _uiController.DisableInput();
            await _minigame.StopGame();
            await _minigame.ShowResult();
            await _uiController.ShowResult();
            _uiController.EnableInput();
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}