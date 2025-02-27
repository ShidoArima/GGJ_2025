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
        [SerializeField] private SimpleGlass _resultGlass;

        [SerializeField] private Transform _blowPosition;

        private bool _hasStarted;
        private bool _resultShown;

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
            _glass.Initialize();
            _pullController.Initialize();
            _benderController.Initialize(_glass);
            _fireController.Initialize();
            _rodController.Initialize();
            _glass.UpdateWeight(_fireController.Phase);
            _expander.Initialize(_glass, _blowPosition.transform.position);
            await HideResult();
            await _rodController.Show();
            await _benderController.ShowAsync();
            await _fireController.Show();
            await _pullController.ShowAsync();
            _hasStarted = true;
        }

        public void Update()
        {
            if (!_hasStarted)
                return;

            _expander.UpdateBend();
            _benderController.UpdateBend();
            _glass.UpdateWeight(_fireController.Phase);
            _glass.Smooth();
        }

        public async UniTask StopGame()
        {
            await _fireController.Hide();
            _benderController.Stop();
            _expander.Stop();
            _fireController.Stop();
            await _benderController.HideAsync();
            await _pullController.HideAsync();
            await _rodController.Hide();
            _hasStarted = false;
        }

        public async UniTask ShowResult()
        {
            if (_resultShown)
            {
                await UniTask.CompletedTask;
                return;
            }

            _resultShown = true;
            _resultGlass.Setup(_glass.Mesh);
            await _resultGlass.ShowAsync();
        }

        public async UniTask HideResult()
        {
            if (!_resultShown)
            {
                await UniTask.CompletedTask;
                return;
            }

            _resultShown = false;
            await _resultGlass.HideAsync();
        }

        private void PullControllerOnPulled()
        {
            _fireController.Switch();
        }
    }
}