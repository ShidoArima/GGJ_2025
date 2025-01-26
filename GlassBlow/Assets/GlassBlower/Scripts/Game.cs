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
            Stop().Forget();
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

        private async UniTaskVoid Stop()
        {
            await _minigame.StopGame();
            await _minigame.ShowResult();
            await _uiController.ShowResult();
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}