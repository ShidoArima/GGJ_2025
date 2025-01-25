using System;
using Cysharp.Threading.Tasks;
using GlassBlower.Scripts.Glass;
using UnityEngine;

namespace GlassBlower.Scripts
{
    public class ShapeMinigame : MonoBehaviour
    {
        [SerializeField] private RodController _controller;
        [SerializeField] private GlassRenderer _glass;
        [SerializeField] private BenderController _benderController;
        [SerializeField] private GlassBender _bender;
        [SerializeField] private GlassExpander _expander;

        [SerializeField] private Transform _sliderPosition;
        [SerializeField] private Transform _blowPosition;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public async UniTaskVoid StartGame()
        {
            gameObject.SetActive(true);
            await _controller.Show();

            _glass.SetupGlass();
            _benderController.Initialize(_glass);
            _expander.Setup(_glass, _blowPosition.transform.position);
        }

        public async UniTaskVoid StopGame()
        {
            await _controller.Hide();
            _benderController.Stop();
            _expander.Stop();
            gameObject.SetActive(false);
        }
    }
}