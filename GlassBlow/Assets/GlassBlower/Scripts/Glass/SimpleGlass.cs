using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GlassBlower.Scripts.Glass
{
    public class SimpleGlass : MonoBehaviour
    {
        [SerializeField] private TweenLocalPosition _showTween;
        [SerializeField] private MeshFilter _filter;

        public void Initialize()
        {
            _showTween.Hide(true);
        }
        
        public void Setup(Mesh mesh)
        {
            _filter.sharedMesh = mesh;
        }

        public async UniTask ShowAsync()
        {
            await _showTween.ShowAsync();
        }

        public async UniTask HideAsync()
        {
            await _showTween.HideAsync();
        }
    }
}