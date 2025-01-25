using UnityEngine;
using UnityEngine.InputSystem;

namespace GlassBlower.Scripts.Glass
{
    [ExecuteInEditMode]
    public class GlassBender : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private GlassRenderer _glass;

        [SerializeField] private InputActionReference _positionAction;
        [SerializeField] private InputActionReference _bendAction;

        private void Update()
        {
            if (!_bendAction.action.IsPressed())
            {
                return;
            }

            UpdatePosition();
            _glass.Bend(transform.position, _radius);
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
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}