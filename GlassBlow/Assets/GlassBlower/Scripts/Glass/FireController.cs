using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace GlassBlower.Scripts.Glass
{
    public class FireController : MonoBehaviour
    {
        [SerializeField] private Transform _startPosition;
        [SerializeField] private Transform _endPosition;
        [SerializeField] private float _showDuration;

        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Color _minColor;
        [SerializeField] private Color _maxColor;
        [SerializeField] private Vector2 _minMaxSize;
        [SerializeField] private Vector2 _minMaxLifetime;
        [SerializeField] private Vector2 _minMaxVolume;
        [SerializeField] private AudioSource _fireSource;

        [SerializeField] private float _fireIncrement;
        [SerializeField] private float _fireDecrement;
        [SerializeField] private float _safeTime;
        [SerializeField] private AnimationCurve _phaseCurve;
        [SerializeField] private float _initialPhase;

        private bool _isInitialized;
        private bool _fireStarted;
        private float _rawPhase;
        private float _phase;
        private float _lastFireTime;

        public float Phase => _phase;

        private Tween _showTween;

        private ParticleSystem.MainModule _mainModule;

        public void Initialize()
        {
            _mainModule = _particleSystem.main;

            _isInitialized = true;
            UpdatePhase(_initialPhase);
            _fireStarted = false;
            _lastFireTime = Time.time;
            transform.position = _startPosition.position;
            UpdateParticles(_phase);
        }

        public void OnDisable()
        {
            _isInitialized = false;
        }

        public void Stop()
        {
            _isInitialized = false;
        }

        public void Switch()
        {
            _fireStarted = !_fireStarted;

            Debug.Log($"Fire: {_fireStarted}");

            if (!_fireStarted)
            {
                _lastFireTime = Time.time;
            }
        }

        public async UniTask Show()
        {
            _showTween.Stop();
            _showTween = Tween.Position(transform, _startPosition.position, _endPosition.position, _showDuration);
            await _showTween;
        }

        public async UniTask Hide()
        {
            _showTween.Stop();
            _showTween = Tween.Position(transform, _endPosition.position, _startPosition.position, _showDuration);
            await _showTween;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            if (_fireStarted)
            {
                _rawPhase += _fireIncrement * Time.deltaTime;
            }
            else if (Time.time - _lastFireTime > _safeTime)
            {
                _rawPhase -= _fireDecrement * Time.deltaTime;
            }

            UpdatePhase(_rawPhase);
            UpdateParticles(_phase);
        }

        private void UpdatePhase(float phase)
        {
            _rawPhase = phase;
            _rawPhase = Mathf.Clamp01(_rawPhase);
            _phase = _phaseCurve.Evaluate(_rawPhase);
        }

        private void UpdateParticles(float phase)
        {
            _mainModule.startColor = new ParticleSystem.MinMaxGradient(Color.Lerp(_minColor, _maxColor, phase));
            _mainModule.startSize = new ParticleSystem.MinMaxCurve(Mathf.Lerp(_minMaxSize.x, _minMaxSize.y, phase));
            _mainModule.startLifetime = new ParticleSystem.MinMaxCurve(Mathf.Lerp(_minMaxLifetime.x, _minMaxLifetime.y, phase));
            _fireSource.volume = Mathf.Lerp(_minMaxVolume.x, _minMaxVolume.y, phase);
        }
    }
}