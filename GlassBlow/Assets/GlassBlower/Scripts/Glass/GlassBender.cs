using UnityEngine;

namespace GlassBlower.Scripts.Glass
{
    [ExecuteInEditMode]
    public class GlassBender : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private GlassRenderer _renderer;

        private void Update()
        {
            if (_renderer == null)
                return;

            _renderer.Bend(transform.position, _radius);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}