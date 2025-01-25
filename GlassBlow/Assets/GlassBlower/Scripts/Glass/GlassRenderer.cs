using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GlassBlower.Scripts
{
    [ExecuteInEditMode]
    public class GlassRenderer : MonoBehaviour
    {
        [SerializeField] private float _length;
        [SerializeField] private float _width;
        [SerializeField] private float _minWidth;
        [SerializeField] private int _segmentsCount;

        [SerializeField] private LineRenderer _renderer;
        [SerializeField] private MeshFilter _meshFilter;

        private Vector3[] _points;
        private Vector3[] _vertices;
        private Vector2[] _uvs;
        private int[] _tris;

        private Mesh _mesh;

        public void Bend(Vector3 position, float radius)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            float localRadius = transform.InverseTransformVector(Vector3.one * radius).y;

            if (!_mesh.bounds.Intersects(new Bounds(localPosition, Vector3.one * localRadius)))
                return;

            var pointsLength = _vertices.Length / 2;

            for (int i = 0; i < pointsLength; i++)
            {
                var index = i * 2;

                Vector2 difference = _vertices[index] - localPosition;
                float distance = difference.magnitude;

                if (distance > localRadius)
                    continue;

                //We care only about vertical component during bent
                float impact = localRadius * Mathf.Abs(difference.y) * (1 - MathUtils.SmoothStep(0, localRadius, distance));

                Vector3 vertexPosition = _vertices[index] - Vector3.up * impact;
                float width = Mathf.Max(_minWidth, vertexPosition.y);

                _vertices[index] = new Vector3(vertexPosition.x, width, vertexPosition.z);
                _vertices[index + 1] = new Vector3(vertexPosition.x, -width, vertexPosition.z);
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
            GenerateLinePoints();
            GenerateMesh();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        private void GenerateLinePoints()
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

        private void GenerateMesh()
        {
            int pointsCount = _segmentsCount + 2;

            _vertices = new Vector3[pointsCount * 2];
            _uvs = new Vector2[pointsCount * 2];

            float delta = _length / (pointsCount - 1);
            float offset = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                var index = i * 2;

                //Generate vertices
                Vector3 horizontal = offset * Vector3.right;
                Vector3 vertical = Vector3.up * _width;

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