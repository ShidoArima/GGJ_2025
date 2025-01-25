using System;
using UnityEditor;
using UnityEngine;

namespace GlassBlower.Scripts.Glass
{
    [ExecuteInEditMode]
    public class GlassRenderer : MonoBehaviour
    {
        [SerializeField] private float _length;
        [SerializeField] private float _width;
        [SerializeField] private float _minWidth;
        [SerializeField] private int _segmentsCount;
        [SerializeField] private MeshFilter _meshFilter;

        [SerializeField] private AnimationCurve _bendCurve;
        [SerializeField] [Range(0, 1)] private float _bendRatio;

        //We need center points to get expand position easier
        private Vector3[] _centerPoints;

        private Vector3[] _vertices;
        private Vector2[] _uvs;
        private int[] _tris;

        private Mesh _mesh;
        private bool _hasMesh;

        public void Bend(Vector3 position, float radius)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            float localRadius = transform.InverseTransformVector(Vector3.one * radius).y;

            var pointsLength = _vertices.Length / 2;

            for (int i = 0; i < pointsLength; i++)
            {
                var index = i * 2;

                Vector2 upDir = _vertices[index] - localPosition;
                Vector2 downDir = _vertices[index + 1] - localPosition;
                float distance = Mathf.Sqrt(Mathf.Min(upDir.sqrMagnitude, downDir.sqrMagnitude));

                if (distance > localRadius)
                    continue;

                float normalImpact = 1 - distance / localRadius;
                float impact = (distance - localRadius) * _bendRatio * _bendCurve.Evaluate(normalImpact);
                Vector3 vertexPosition = _vertices[index] + Vector3.up * impact;
                vertexPosition.y = Mathf.Max(_minWidth, vertexPosition.y);

                _vertices[index] = new Vector3(vertexPosition.x, vertexPosition.y, vertexPosition.z);
                _vertices[index + 1] = new Vector3(vertexPosition.x, -vertexPosition.y, vertexPosition.z);
            }

            _mesh.vertices = _vertices;
            _mesh.RecalculateBounds();
        }

        public Vector3 GetCenter(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);

            //Get center of expand
            float minSqrMag = Single.PositiveInfinity;
            Vector3 center = Vector3.zero;
            for (int i = 0; i < _centerPoints.Length; i++)
            {
                var sqrMag = (localPosition - _centerPoints[i]).sqrMagnitude;
                if (minSqrMag > sqrMag)
                {
                    minSqrMag = sqrMag;
                    center = _centerPoints[i];
                }
            }

            return transform.TransformPoint(center);
        }

        public void Expand(Vector3 position, float radius)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            float localRadius = transform.InverseTransformVector(Vector3.one * radius).y;

            //Expand
            for (int i = 0; i < _centerPoints.Length; i++)
            {
                var index = i * 2;

                Vector2 direction = _vertices[index] - localPosition;
                float distance = direction.magnitude;

                if (distance > radius)
                    continue;

                float normalImpact = 1 - distance / localRadius;
                float impact = (distance - radius) * _bendRatio;
                Vector3 vertexPosition = _vertices[index] - Vector3.up * impact * _bendCurve.Evaluate(normalImpact);
                vertexPosition.y = Mathf.Max(_minWidth, vertexPosition.y);

                _vertices[index] = new Vector3(vertexPosition.x, vertexPosition.y, vertexPosition.z);
                _vertices[index + 1] = new Vector3(vertexPosition.x, -vertexPosition.y, vertexPosition.z);
            }

            _mesh.vertices = _vertices;
            _mesh.RecalculateBounds();
        }

        private void OnEnable()
        {
            InitMesh();
            GenerateMesh();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            GenerateMesh();
            EditorUtility.SetDirty(this);
#endif
        }

        private void GenerateMesh()
        {
            if (!_hasMesh)
            {
                return;
            }

            int pointsCount = _segmentsCount + 2;

            _centerPoints = new Vector3[pointsCount];
            _vertices = new Vector3[pointsCount * 2];
            _uvs = new Vector2[pointsCount * 2];

            float delta = _length / (pointsCount - 1);
            float offset = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                var index = i * 2;

                //Generate vertices
                Vector3 horizontal = offset * Vector3.right;
                Vector3 vertical = Vector3.up * Mathf.Max(_minWidth, _width);

                _centerPoints[i] = horizontal;
                _vertices[index] = horizontal + vertical;
                _vertices[index + 1] = horizontal - vertical;

                //Generate Uvs
                float normalDistance = offset / _length;
                _uvs[index] = new Vector2(normalDistance, 1);
                _uvs[index + 1] = new Vector2(normalDistance, 0);

                offset += delta;
            }

            var tris = GetTris(pointsCount, 0);

            _mesh.Clear();
            _mesh.subMeshCount = 1;
            _mesh.SetVertices(_vertices);
            _mesh.SetUVs(0, _uvs);
            _mesh.SetTriangles(tris, 0);
            _mesh.RecalculateBounds();
        }

        private void InitMesh()
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
            }

            _mesh.MarkDynamic();
            _mesh.hideFlags = HideFlags.DontSaveInEditor;
            _meshFilter.sharedMesh = _mesh;
            _hasMesh = true;
        }

        private int[] GetTris(int size, int offset)
        {
            int[] tris = new int[6 * (size - 1)];

            for (int i = 0; i < size - 1; i++)
            {
                int vertOffset = 2 * i + offset;
                int triOffset = i * 6;

                tris[triOffset] = vertOffset;
                tris[triOffset + 1] = vertOffset + 2;
                tris[triOffset + 2] = vertOffset + 1;

                tris[triOffset + 3] = vertOffset + 1;
                tris[triOffset + 4] = vertOffset + 2;
                tris[triOffset + 5] = vertOffset + 3;
            }

            return tris;
        }
    }
}