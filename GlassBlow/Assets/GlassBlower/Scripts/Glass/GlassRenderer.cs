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
        [SerializeField] private Renderer _renderer;

        [SerializeField] private float _bendRatio;
        [SerializeField] private float _minBend;
        [SerializeField] private float _maxBend;
        [SerializeField] private float _bendForce;

        [SerializeField] private float _smoothFactor = 1;
        [SerializeField] private float _stretchForce = 0.5f;
        [SerializeField] private int _iterations;

        [SerializeField] private AnimationCurve _weightDistribution;

        //We need center points to get expand position easier
        private Vector3[] _centerPoints;
        private float[] _centerWeights;

        private Vector3[] _vertices;
        private Vector2[] _uvs;
        private int[] _tris;

        private float _restLength;

        private Mesh _mesh;
        private bool _hasMesh;
        private MaterialPropertyBlock _propertyBlock;

        private static readonly int HeatPhase = Shader.PropertyToID("_HeatPhase");

        public void UpdateWeight(float phase)
        {
            _bendRatio = Mathf.Lerp(_minBend, _maxBend, phase);
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(HeatPhase, phase);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void Bend(Vector3 position, float radius)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            float localRadius = transform.InverseTransformVector(Vector3.one * radius).y;

            var pointsLength = _vertices.Length / 2;

            Vector3 stretchForce = Vector3.right * (_stretchForce * Mathf.Clamp01(_bendRatio) * Time.deltaTime);

            for (int i = 0; i < pointsLength; i++)
            {
                var index = i * 2;

                float normalLength = (float)i / (pointsLength - 1);
                _vertices[index] += stretchForce * (_centerWeights[i] * normalLength);
                _vertices[index + 1] += stretchForce * (_centerWeights[i] * normalLength);

                //Optimization we always effect from the top
                Vector2 direction = _vertices[index] - localPosition;

                float x = direction.x;
                if (Mathf.Abs(x) > localRadius)
                    continue;

                float target = -Mathf.Sqrt(localRadius * localRadius - x * x);

                if (direction.y < target)
                    continue;

                float impact = _bendForce * _bendRatio * _centerWeights[i] * Time.deltaTime;

                Vector3 vertexPosition = _vertices[index] - Vector3.up * impact;
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

                Vector3 direction = _vertices[index] - localPosition;
                float distance = direction.magnitude;

                if (distance > localRadius)
                    continue;

                float impact = (distance - localRadius) * _bendRatio * _bendForce;
                impact *= _centerWeights[i] * Time.deltaTime;

                Vector3 dirNormal = direction.normalized;
                Vector3 vertexPosition = _vertices[index] - new Vector3(dirNormal.x, dirNormal.y, 0) * impact;
                vertexPosition.y = Mathf.Max(_minWidth, vertexPosition.y);

                _vertices[index] = new Vector3(vertexPosition.x, vertexPosition.y, vertexPosition.z);
                _vertices[index + 1] = new Vector3(vertexPosition.x, -vertexPosition.y, vertexPosition.z);
            }

            _mesh.vertices = _vertices;
            _mesh.RecalculateBounds();
        }

        Vector3 GetDelta(Vector3 a, Vector3 b)
        {
            Vector3 delta = a - b;
            float current = delta.magnitude;
            if (current == 0)
                return Vector3.zero;

            float f = (current - _restLength * _smoothFactor) / current;

            return f * delta;
        }

        public void Smooth()
        {
            float bendRatio = Mathf.Clamp01(_bendRatio) / _iterations;

            for (int i = 0; i < _iterations; i++)
            {
                for (int j = 0; j < _centerPoints.Length - 1; j++)
                {
                    var index = j * 2;
                    Vector3 a = _vertices[index];
                    Vector3 b = _vertices[index + 2];
                    float aWeight = _centerWeights[j];
                    float bWeight = _centerWeights[j + 1];
                    float weightSum = aWeight + bWeight;

                    if (weightSum == 0)
                        continue;

                    Vector3 delta = GetDelta(a, b) * bendRatio;

                    a -= (aWeight / weightSum) * delta;
                    b += (bWeight / weightSum) * delta;

                    _vertices[index] = a;
                    _vertices[index + 2] = b;

                    _vertices[index + 1] = new Vector3(a.x, -a.y, a.z);
                    _vertices[index + 3] = new Vector3(b.x, -b.y, b.z);
                }
            }
        }

        public void SetupGlass()
        {
            if (_propertyBlock == null)
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            InitMesh();
            GenerateMesh();
            _bendRatio = 1;
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                SetupGlass();
            }
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
            _centerWeights = new float[pointsCount];
            _vertices = new Vector3[pointsCount * 2];
            _uvs = new Vector2[pointsCount * 2];

            _restLength = _length / (pointsCount - 1);
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

                _centerWeights[i] = _weightDistribution.Evaluate(normalDistance);

                offset += _restLength;
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