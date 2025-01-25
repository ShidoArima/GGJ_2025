using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace GlassBlower.Scripts
{
    public class RodController : MonoBehaviour
    {
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _end;
        [SerializeField] private Transform _glassPivot;

        [SerializeField] private float _showDuration;
        [SerializeField] private float _hideDuration;

        private Tween _tween;
        
        public async UniTask Show()
        {
            _tween.Stop();
            _tween = Tween.Position(transform, _start.position, _end.position, _showDuration, Ease.InBack);
            await _tween;
        }

        public async UniTask Hide()
        {
            _tween.Stop();
            _tween = Tween.Position(transform, _end.position, _start.position, _hideDuration, Ease.OutBack);
            await _tween;
        }
    }
}