using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlassBlower.Scripts.Glass
{
    public class BenderController : MonoBehaviour
    {
        [SerializeField] private Transform _animationEnd;
        [SerializeField] private float _showDuration;

        [SerializeField] private GlassBender _bender;
        [SerializeField] private Bounds _bounds;
        [SerializeField] private Transform _startPosition;

        [SerializeField] private InputActionReference _positionAction;
        [SerializeField] private InputActionReference _interactAction;


        private Tween _showTween;
        private const float AnimationDistance = 10f;

        public void Initialize(GlassRenderer glass)
        {
            _bender.Setup(glass);
            _bender.transform.position = _startPosition.position;
            transform.position = new Vector3(transform.position.x, AnimationDistance, transform.position.z);
        }

        public async UniTask ShowAsync()
        {
            _showTween.Stop();
            _showTween = Tween.PositionY(transform, AnimationDistance, _animationEnd.position.y, _showDuration, Ease.InOutBack);

            await _showTween;
        }

        public async UniTask HideAsync()
        {
            _showTween.Stop();
            _showTween = Tween.PositionY(transform, _animationEnd.position.y, AnimationDistance, _showDuration, Ease.InOutBack);

            await _showTween;
        }

        public void UpdateBend()
        {
            _bender.UpdateBend();
        }

        public void Stop()
        {
            _bender.Stop();
        }

        public void Update()
        {
            Vector3 target = _bender.transform.localPosition;
            if (_interactAction.action.IsPressed())
            {
                var pointerPosition = _positionAction.action.ReadValue<Vector2>();
                Vector3 position = Camera.main.ScreenToWorldPoint(pointerPosition);
                if (_bender.Contains(position))
                {
                    var localPosition = transform.InverseTransformPoint(position);
                    target = new Vector3(localPosition.x, localPosition.y, target.z);
                }
            }

            target.x = Mathf.Clamp(target.x, _bounds.min.x, _bounds.max.x);
            target.y = Mathf.Clamp(target.y, _bounds.min.y, _bounds.max.y);

            _bender.transform.localPosition = target;
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }
    }
}