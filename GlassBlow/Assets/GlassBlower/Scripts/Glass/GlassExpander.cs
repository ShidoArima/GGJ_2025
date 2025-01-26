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
        private static readonly int ExpandPosition = Shader.PropertyToID("_ExpandPosition");
        private static readonly int ExpandRadius = Shader.PropertyToID("_ExpandRadius");

        public void Initialize(GlassRenderer glass, Vector3 startPosition)
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

        public void UpdateBend()
        {
            if (!_isInitialized)
                return;
            
            Shader.SetGlobalFloat(ExpandRadius, _currentRadius);

            if (!_extendAction.action.IsPressed())
            {
                _currentRadius = Mathf.Max(0, _currentRadius - _force * Time.deltaTime);
                return;
            }

            UpdatePosition();

            _centerPosition = _glass.GetCenter(transform.position);

            _glass.Expand(_centerPosition, _currentRadius);
            _currentRadius += _force * Time.deltaTime;
            _currentRadius = Mathf.Min(_currentRadius, _maxExpandRadius);

            Shader.SetGlobalVector(ExpandPosition, _centerPosition);
            
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