using System;
using System.Collections.Generic;
using UnityEngine;

namespace DIYMesh
{
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
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

        [Range(0.05f, 1f)] public float drawCoolDown = 0.1f;
        [Range(0.05f, 2f)] public float drawWeight = 0.1f;

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
            if (Input.GetMouseButton(0) && !_drawStatus && _nextDraw <= Time.time)
            {
                _nextDraw = Time.time + drawCoolDown;
                if (_isCameraNull) return;
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hitInfo, 1000)) return;
                if (_vector3S.Count > 0 && Vector3.Distance(_vector3S[_vector3S.Count - 1],
                        new Vector3(hitInfo.point.x, hitInfo.point.y, 0)) < 0.2) return;
                _vector3S.Add(new Vector3(hitInfo.point.x, hitInfo.point.y, 0));
//                if (_vector3S.Count >= 3)
                SetVertices(_vector3S, drawWeight);
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
        public void SetVertices(List<Vector3> verticesList, float weight = 1f)
        {
            if (verticesList.Count < 3)
            {
                _meshRenderer.enabled = false;
                _meshCollider.enabled = false;
                return;
            }

            _meshRenderer.enabled = true;
            _meshCollider.enabled = true;

            _vertices = new Vector3[verticesList.Count * 2];
            _indices = new int[verticesList.Count * 24];
            for (var i = 0; i < verticesList.Count; i++)
            {
                _vertices[i] = verticesList[i];
                _vertices[i].z -= weight / 2;
                _vertices[i + verticesList.Count] = verticesList[i];
                _vertices[i + verticesList.Count].z += weight / 2;

                var nearby = i == verticesList.Count - 1 ? 0 : i + 1;
                _indices[i * 12 + 00] = i;
                _indices[i * 12 + 01] = nearby;
                _indices[i * 12 + 02] = i + verticesList.Count;
                _indices[i * 12 + 03] = nearby;
                _indices[i * 12 + 04] = nearby + verticesList.Count;
                _indices[i * 12 + 05] = i + verticesList.Count;

                _indices[i * 12 + 06] = i;
                _indices[i * 12 + 07] = i + verticesList.Count;
                _indices[i * 12 + 08] = nearby;
                _indices[i * 12 + 09] = nearby;
                _indices[i * 12 + 10] = i + verticesList.Count;
                _indices[i * 12 + 11] = nearby + verticesList.Count;
            }

            for (var i = 0; i < verticesList.Count - 2; i++)
            {
                _indices[verticesList.Count * 12 + i * 12 + 00] = i;
                _indices[verticesList.Count * 12 + i * 12 + 01] = i + 1;
                _indices[verticesList.Count * 12 + i * 12 + 02] = verticesList.Count - 1;
                _indices[verticesList.Count * 12 + i * 12 + 03] = i;
                _indices[verticesList.Count * 12 + i * 12 + 04] = verticesList.Count - 1;
                _indices[verticesList.Count * 12 + i * 12 + 05] = i + 1;

                _indices[verticesList.Count * 12 + i * 12 + 06] = i + verticesList.Count;
                _indices[verticesList.Count * 12 + i * 12 + 07] = verticesList.Count + i + 1;
                _indices[verticesList.Count * 12 + i * 12 + 08] = verticesList.Count + verticesList.Count - 1;
                _indices[verticesList.Count * 12 + i * 12 + 09] = i + verticesList.Count;
                _indices[verticesList.Count * 12 + i * 12 + 10] = verticesList.Count + verticesList.Count - 1;
                _indices[verticesList.Count * 12 + i * 12 + 11] = verticesList.Count + i + 1;
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