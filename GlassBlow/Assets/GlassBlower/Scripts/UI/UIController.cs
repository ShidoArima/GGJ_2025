using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GlassBlower.Scripts.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private StartWindow _startWindow;
        [SerializeField] private GameplayWindow _gameplayWindow;
        [SerializeField] private PauseWindow _pauseWindow;

        public event Action GameStarted;
        public event Action GameFinish;
        public event Action GameExit;

        private BaseWindow _currentWindow;
        private GameInput _input;

        public void Initialize()
        {
            _input = new GameInput();
            
            InitializeWindow(_startWindow);
            InitializeWindow(_gameplayWindow);
            InitializeWindow(_pauseWindow);

            ShowIntro().Forget();
        }

        private void InitializeWindow(BaseWindow window)
        {
            window.Initialize(this);
            window.gameObject.SetActive(false);
        }

        public async UniTask ShowIntro()
        {
            await ChangeWindow(_startWindow);
        }

        public async UniTask ShowGame()
        {
            await ChangeWindow(_gameplayWindow);
        }

        public async UniTask ShowPause()
        {
            await ChangeWindow(_pauseWindow);
        }

        public async UniTaskVoid StartGame()
        {
            await ShowGame();
            GameStarted?.Invoke();
        }
        
        public async UniTaskVoid Finish()
        {
            GameFinish?.Invoke();
        }

        public void ExitGame()
        {
            Debug.Log("Exit");
            GameExit?.Invoke();
        }

        private async UniTask ChangeWindow(BaseWindow newWindow)
        {
            if (newWindow == _currentWindow)
                return;

            var oldWindow = _currentWindow ? _currentWindow.Hide() : UniTask.CompletedTask;
            _currentWindow = newWindow;

            await newWindow.Show(oldWindow);
        }
    }
}