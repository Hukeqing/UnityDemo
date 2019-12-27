using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DIYMesh
{
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public enum MeshMode
    {
        Mode,
        Object
    }

    public class DiyMeshObject : MonoBehaviour
    {
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        private Vector3[] _vertices;
        private int[] _indices;

        private Shader _shader;
        private Mesh _mesh;
        private Material _material;
        private MeshCollider _meshCollider;

        public bool threeDimensional = true;
        public MeshMode mode = MeshMode.Mode;
        public float drawCoolDown = 0.05f;
        public float drawForwardWeight = -0.1f;
        public float drawBehindWeight = 0.1f;

        private List<Vector3> _vector3S;
        private bool _drawStatus;
        private float _nextDraw;
        private Camera _camera;
        private bool _isCameraNull;
        private MeshRenderer _meshRenderer;

        private void Start()
        {
            _camera = Camera.main;
            _isCameraNull = _camera == null;
            _shader = Shader.Find("Standard");
            _mesh = new Mesh();
            _mesh.Clear();
            _material = new Material(_shader);

            if (!GetComponent<MeshFilter>())
            {
                gameObject.AddComponent<MeshFilter>();
            }

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            if (!GetComponent<MeshRenderer>())
            {
                gameObject.AddComponent<MeshRenderer>();
            }

            (_meshRenderer = GetComponent<MeshRenderer>()).material = _material;

            if (!GetComponent<MeshCollider>())
            {
                gameObject.AddComponent<MeshCollider>();
            }

            _meshCollider = GetComponent<MeshCollider>();

            _vector3S = new List<Vector3>();
            SetColor(Color.red);
            _drawStatus = false;
            _nextDraw = 0;
        }

        private void Update()
        {
            if (mode == MeshMode.Object) return;
            if (Input.GetMouseButton(0) && !_drawStatus && _nextDraw <= Time.time)
            {
                _nextDraw = Time.time + drawCoolDown;
                if (_isCameraNull) return;
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hitInfo, 1000)) return;
                if (_vector3S.Count > 0 && Vector3.Distance(_vector3S[_vector3S.Count - 1],
                        new Vector3(hitInfo.point.x, hitInfo.point.y, 0)) < 0.2) return;
                _vector3S.Add(new Vector3(hitInfo.point.x, hitInfo.point.y, 0));
                SetVertices(_vector3S, drawForwardWeight, drawBehindWeight);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _drawStatus = true;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void SetColor(Color color, RenderingMode renderingMode = RenderingMode.Opaque)
        {
            _material.color = color;
            SetMaterialRenderingMode(_material, renderingMode);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void SetVertices(List<Vector3> verticesList, float forwardWeight = 0.5f, float behindWeight = 0.5f)
        {
            if (verticesList.Count < 3)
            {
                _meshRenderer.enabled = false;
                _meshCollider.enabled = false;
                return;
            }

            _meshRenderer.enabled = true;
            _meshCollider.enabled = true;
            //  vertices:     0 - n-1        forward
            //                n - 2n-1       behind 
            // indices:       0 - 6*(n-2)                forward face
            //                6*(n-2) - 12*(n-2)         behind face
            //                12*(n-2) - 24*(n-2)        weight face
            if (threeDimensional)
            {
                _vertices = new Vector3[verticesList.Count * 2];
                _indices = new int[verticesList.Count * 24];
            }
            else
            {
                _vertices = new Vector3[verticesList.Count];
                _indices = new int[verticesList.Count * 6];
            }

            for (var i = 0; i < verticesList.Count; i++)
            {
                _vertices[i] = verticesList[i];
                _vertices[i].z += forwardWeight;
                if (!threeDimensional) continue;
                _vertices[i + verticesList.Count] = verticesList[i];
                _vertices[i + verticesList.Count].z += behindWeight;
            }

            for (var i = 0; i < verticesList.Count - 2; i++)
            {
                _indices[i * 6 + 0] = 0;
                _indices[i * 6 + 1] = i + 1;
                _indices[i * 6 + 2] = i + 2;
                _indices[i * 6 + 3] = 0;
                _indices[i * 6 + 4] = i + 2;
                _indices[i * 6 + 5] = i + 1;
            }

            if (threeDimensional)
            {
                var beginOfIndices = 6 * (verticesList.Count - 2);
                var behindVertices = verticesList.Count;
                for (var i = 0; i < verticesList.Count - 2; i++)
                {
                    _indices[beginOfIndices + i * 6 + 0] = behindVertices;
                    _indices[beginOfIndices + i * 6 + 1] = behindVertices + i + 1;
                    _indices[beginOfIndices + i * 6 + 2] = behindVertices + i + 2;
                    _indices[beginOfIndices + i * 6 + 3] = behindVertices;
                    _indices[beginOfIndices + i * 6 + 4] = behindVertices + i + 2;
                    _indices[beginOfIndices + i * 6 + 5] = behindVertices + i + 1;
                }

                beginOfIndices *= 2;
                for (var i = 0; i < verticesList.Count; i++)
                {
                    var j = i == 0 ? verticesList.Count - 1 : i - 1;
                    _indices[beginOfIndices + i * 12 + 00] = i;
                    _indices[beginOfIndices + i * 12 + 01] = j;
                    _indices[beginOfIndices + i * 12 + 02] = i + behindVertices;
                    _indices[beginOfIndices + i * 12 + 03] = i;
                    _indices[beginOfIndices + i * 12 + 04] = i + behindVertices;
                    _indices[beginOfIndices + i * 12 + 05] = j;

                    _indices[beginOfIndices + i * 12 + 06] = j;
                    _indices[beginOfIndices + i * 12 + 07] = j + behindVertices;
                    _indices[beginOfIndices + i * 12 + 08] = i + behindVertices;
                    _indices[beginOfIndices + i * 12 + 09] = j;
                    _indices[beginOfIndices + i * 12 + 10] = i + behindVertices;
                    _indices[beginOfIndices + i * 12 + 11] = j + behindVertices;
                }
            }

            _mesh.vertices = _vertices;
            _mesh.triangles = _indices;
            _meshCollider.sharedMesh = _mesh;
        }

        public void Clear()
        {
            _drawStatus = false;
            _vector3S.Clear();
            _mesh.Clear();
        }

        //设置材质的渲染模式
        private static void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt(ZWrite, 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt(ZWrite, 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt(ZWrite, 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt(ZWrite, 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderingMode), renderingMode, null);
            }
        }
    }
}