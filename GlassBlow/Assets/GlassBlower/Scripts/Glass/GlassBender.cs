using UnityEngine;
using UnityEngine.InputSystem;

namespace GlassBlower.Scripts.Glass
{
    [ExecuteInEditMode]
    public class GlassBender : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private GlassRenderer _glass;
        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField] private InputActionReference _positionAction;
        [SerializeField] private InputActionReference _bendAction;

        private bool _isInitialized;

        public void Setup(GlassRenderer glass)
        {
            _glass = glass;
            _isInitialized = true;
        }

        public void UpdateBend()
        {
            if (!_isInitialized)
                return;

            _glass.Bend(transform.position, _radius);
        }

        public void Stop()
        {
            _isInitialized = false;
            _glass = null;
        }

        public bool Contains(Vector3 position)
        {
            return _renderer.bounds.Contains(new Vector3(position.x, position.y, _renderer.bounds.center.z));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}