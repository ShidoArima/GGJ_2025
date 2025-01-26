using System;
using Cysharp.Threading.Tasks;
using GlassBlower.Scripts.Glass;
using UnityEngine;

namespace GlassBlower.Scripts
{
    public class ShapeMinigame : MonoBehaviour
    {
        [SerializeField] private RodController _rodController;
        [SerializeField] private GlassRenderer _glass;
        [SerializeField] private BenderController _benderController;
        [SerializeField] private GlassExpander _expander;
        [SerializeField] private PullController _pullController;
        [SerializeField] private FireController _fireController;

        [SerializeField] private Transform _blowPosition;

        private bool _hasStarted;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _pullController.Pulled += PullControllerOnPulled;
        }

        private void OnDisable()
        {
            _pullController.Pulled -= PullControllerOnPulled;
            _hasStarted = false;
        }

        public async UniTask StartGame()
        {
            gameObject.SetActive(true);

            _glass.SetupGlass();
            _pullController.Initialize();
            _benderController.Initialize(_glass);
            _fireController.Initialize();
            _glass.UpdateWeight(_fireController.Phase);
            _expander.Setup(_glass, _blowPosition.transform.position);
            await _rodController.Show();
            await _fireController.Show();
            _pullController.Show();
            _hasStarted = true;
        }

        public void Update()
        {
            if (!_hasStarted)
                return;

            _expander.UpdateBend();
            _benderController.UpdateBend();
            _glass.UpdateWeight(_fireController.Phase);
        }

        public async UniTask StopGame()
        {
            await _rodController.Hide();
            _benderController.Stop();
            _expander.Stop();
            _fireController.Stop();
            await _fireController.Hide();
            _pullController.Hide();
            gameObject.SetActive(false);
            _hasStarted = false;
        }

        private void PullControllerOnPulled()
        {
            _fireController.Switch();
        }
    }
}