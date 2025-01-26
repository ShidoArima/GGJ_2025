using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace GlassBlower.Scripts
{
    public class TweenLocalPosition : MonoBehaviour
    {
        [SerializeField] private Vector3 _start;
        [SerializeField] private Vector3 _end;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _easeType;

        private Tween _showTween;

        public void Show(bool immediate = false)
        {
            _showTween.Stop();
            if (immediate)
            {
                transform.localPosition = _end;
                return;
            }

            _showTween = Tween.LocalPosition(transform, _start, _end, _duration, _easeType);
        }

        public void Hide(bool immediate = false)
        {
            _showTween.Stop();
            if (immediate)
            {
                transform.localPosition = _start;
                return;
            }

            _showTween = Tween.LocalPosition(transform, _end, _start, _duration, _easeType);
        }

        public async UniTask ShowAsync()
        {
            _showTween.Stop();
            _showTween = Tween.LocalPosition(transform, _start, _end, _duration, _easeType);
            await _showTween;
        }

        public async UniTask HideAsync()
        {
            _showTween.Stop();
            _showTween = Tween.LocalPosition(transform, _end, _start, _duration, _easeType);
            await _showTween;
        }
    }
}