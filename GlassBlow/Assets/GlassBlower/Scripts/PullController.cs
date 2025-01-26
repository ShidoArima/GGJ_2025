using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlassBlower.Scripts
{
    public class PullController : MonoBehaviour
    {
        [SerializeField] private Transform _startPosition;
        [SerializeField] private Transform _endPosition;
        [SerializeField] private float _showDuration;

        [SerializeField] private Transform _pivot;
        [SerializeField] private Collider2D _pullTrigger;

        [SerializeField] private float _pullDistance;
        [SerializeField] private float _pullBackTime;

        [SerializeField] private InputActionReference _interactAction;
        [SerializeField] private InputActionReference _positionAction;

        public event Action Pulled;

        private bool _isPulling;
        private Tween _showTween;
        private Tween _pullTween;
        private Vector3 _offset;

        private void OnEnable()
        {
            _interactAction.action.started += Interact;
            _interactAction.action.canceled += Cancel;
        }

        private void OnDisable()
        {
            _interactAction.action.started -= Interact;
            _interactAction.action.canceled -= Cancel;
        }

        public void Initialize()
        {
            transform.position = _startPosition.position;
        }

        public void Show()
        {
            _showTween = Tween.Position(transform, _startPosition.position, _endPosition.position, _showDuration,
                Ease.InOutBack);
        }
        
        public void Hide()
        {
            _showTween = Tween.Position(transform, _endPosition.position, _startPosition.position, _showDuration,
                Ease.InOutBack);
        }

        private void Cancel(InputAction.CallbackContext obj)
        {
            Cancel();
        }

        private void Interact(InputAction.CallbackContext obj)
        {
            var pointerPosition = _positionAction.action.ReadValue<Vector2>();
            Vector3 position = Camera.main.ScreenToWorldPoint(pointerPosition);
            if (Contains(position))
            {
                Vector3 localPosition = transform.InverseTransformPoint(position);

                _offset = _pivot.localPosition - localPosition;
                _isPulling = true;
                _pullTween.Stop();
            }
        }

        private void Cancel()
        {
            if (!_isPulling)
                return;

            _isPulling = false;
            _pullTween.Stop();
            _pullTween = Tween.LocalPositionY(_pivot, 0, _pullBackTime, Ease.InOutBack);
        }

        private void Update()
        {
            if (_isPulling)
            {
                var pointerPosition = _positionAction.action.ReadValue<Vector2>();
                Vector3 position = Camera.main.ScreenToWorldPoint(pointerPosition);
                if (!Contains(position))
                {
                    Cancel();
                    return;
                }

                Vector3 pullPosition = transform.InverseTransformPoint(position) + _offset;
                var localPosition = _pivot.localPosition;
                localPosition = new Vector3(localPosition.x, Mathf.Min(0, pullPosition.y), localPosition.z);
                _pivot.localPosition = localPosition;

                if (_pivot.localPosition.y < -_pullDistance)
                {
                    Pulled?.Invoke();
                    Cancel();
                }
            }
        }

        private bool Contains(Vector3 position)
        {
            return  _pullTrigger.OverlapPoint(position);;
        }

        private void OnDrawGizmos()
        {
            var position = _pullTrigger.transform.position;
            Gizmos.DrawLine(position, position + Vector3.down * _pullDistance);
        }
    }
}