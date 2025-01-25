using UnityEngine;
using UnityEngine.InputSystem;

namespace GlassBlower.Scripts.Glass
{
    public class GlassExpander : MonoBehaviour
    {
        [SerializeField] private GlassRenderer _glass;
        [SerializeField] private float _force;
        [SerializeField] private float _maxExpandRadius;
        [SerializeField] private InputActionReference _positionAction;
        [SerializeField] private InputActionReference _extendAction;

        private float _currentRadius;
        private Vector3 _centerPosition;
        private bool _isInitialized;

        public void Setup(GlassRenderer glass, Vector3 startPosition)
        {
            _glass = glass;
            transform.position = startPosition;
            _isInitialized = true;
        }

        public void Stop()
        {
            _isInitialized = false;
            _glass = null;
        }

        private void Update()
        {
            if(!_isInitialized)
                return;
            
            if (!_extendAction.action.IsPressed())
            {
                _currentRadius = 0;
                return;
            }

            UpdatePosition();

            _centerPosition = _glass.GetCenter(transform.position);

            _glass.Expand(_centerPosition, _currentRadius);
            _currentRadius += _force * Time.deltaTime;
            _currentRadius = Mathf.Min(_currentRadius, _maxExpandRadius);
        }

        private void UpdatePosition()
        {
            Vector2 mousePosition = _positionAction.action.ReadValue<Vector2>();
            Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);

            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _currentRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_centerPosition, _currentRadius);
        }
    }
}