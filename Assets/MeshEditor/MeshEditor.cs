using UnityEngine;

namespace MeshEditor
{
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    [ExecuteInEditMode]
    public class MeshEditor : MonoBehaviour
    {
        public Vector3[] vertices;

        public Shader shader;

        private Color _color;
        private int[] _triangles;

        [Range(0, 1)] public float ap;

        private Mesh _mesh;
        private Material _mat;
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        private void Start()
        {
            _mesh = new Mesh();
            gameObject.GetComponent<MeshFilter>().mesh = _mesh;
            _mat = new Material(shader);
//            _mat.renderQueue
            SetMaterialRenderingMode(_mat, RenderingMode.Fade);
            gameObject.GetComponent<MeshRenderer>().material = _mat;
            _color = new Color(Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f));
        }

        private void Update()
        {
            _mat.color = new Color(_color.r, _color.g, _color.b, ap);
            _triangles = new int[(vertices.Length * 6 - 12) + (vertices.Length * 12 - 24)];
            var curTri = 0;
            int i;
            for (i = 0; i < vertices.Length / 2 - 1; i++)
            {
                _triangles[curTri + 0] = i;
                _triangles[curTri + 1] = i + 1;
                _triangles[curTri + 2] = vertices.Length / 2 + i;
                _triangles[curTri + 3] = i;
                _triangles[curTri + 4] = vertices.Length / 2 + i;
                _triangles[curTri + 5] = i + 1;
                _triangles[curTri + 6] = i + 1;
                _triangles[curTri + 7] = i + vertices.Length / 2;
                _triangles[curTri + 8] = i + vertices.Length / 2 + 1;
                _triangles[curTri + 9] = i + 1;
                _triangles[curTri + 10] = i + vertices.Length / 2 + 1;
                _triangles[curTri + 11] = i + vertices.Length / 2;
                curTri += 12;
            }

            for (i = 0; i < vertices.Length / 2 - 2; i++)
            {
                _triangles[curTri + 0] = i;
                _triangles[curTri + 1] = vertices.Length / 2 - 2;
                _triangles[curTri + 2] = vertices.Length / 2 - 1;
                _triangles[curTri + 3] = vertices.Length / 2 + i;
                _triangles[curTri + 4] = vertices.Length - 2;
                _triangles[curTri + 5] = vertices.Length - 1;
                _triangles[curTri + 6] = i;
                _triangles[curTri + 7] = vertices.Length / 2 - 1;
                _triangles[curTri + 8] = vertices.Length / 2 - 2;
                _triangles[curTri + 9] = vertices.Length / 2 + i;
                _triangles[curTri + 10] = vertices.Length - 1;
                _triangles[curTri + 11] = vertices.Length - 2;
                curTri += 12;
            }

            _mesh.vertices = vertices;
            _mesh.triangles = _triangles;
        }

        //设置材质的渲染模式
        private void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
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
            }
        }
    }
}