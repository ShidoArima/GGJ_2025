using System;
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

        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Color _minColor;
        [SerializeField] private Color _maxColor;

        [SerializeField] private float _fireIncrement;
        [SerializeField] private float _fireDecrement;
        [SerializeField] private float _safeTime;
        [SerializeField] private float _initialPhase;

        private bool _isInitialized;
        private bool _fireStarted;
        private float _phase;
        private float _lastFireTime;

        public float Phase => _phase;

        private Tween _showTween;

        public void Initialize()
        {
            _isInitialized = true;
            _phase = _initialPhase;
            _fireStarted = false;
            _lastFireTime = Time.time;
            _renderer.color = Color.Lerp(_minColor, _maxColor, _phase);
            transform.position = _startPosition.position;
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
                _phase += _fireIncrement * Time.deltaTime;
            }
            else if (Time.time - _lastFireTime > _safeTime)
            {
                _phase -= _fireDecrement * Time.deltaTime;
            }

            _phase = Mathf.Clamp01(_phase);
            _renderer.color = Color.Lerp(_minColor, _maxColor, _phase);
        }
    }
}