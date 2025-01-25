using UnityEngine;

namespace GlassBlower.Scripts
{
    [ExecuteInEditMode]
    public class Glass : MonoBehaviour
    {
        [SerializeField] private float _length;
        [SerializeField] private float _width;
        [SerializeField] private int _segmentsCount;

        [SerializeField] private LineRenderer _renderer;

        private Vector3[] _points;

        private void OnValidate()
        {
            Generate();
        }

        private void Generate()
        {
            int pointsCount = _segmentsCount + 2;
            _points = new Vector3[pointsCount];

            float delta = _length / (pointsCount - 1);
            float offset = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                _points[i] = offset * Vector3.right;
                offset += delta;
            }

            if (_renderer == null)
                return;

            _renderer.widthMultiplier = _width;
            _renderer.positionCount = pointsCount;
            _renderer.SetPositions(_points);
        }
    }
}