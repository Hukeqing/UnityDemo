using UnityEngine;

namespace CameraGrey
{
    [ExecuteInEditMode]
    public class CameraGreyScript : MonoBehaviour
    {
        public Shader curShader;
        [Range(0.0f, 1.0f)]
        public float grayScaleAmount = 1.0f;
        private Material _curMaterial;
        private static readonly int LuminosityAmount = Shader.PropertyToID("_LuminosityAmount");

        private Material Material
        {
            get
            {
                if (_curMaterial != null) return _curMaterial;
                _curMaterial = new Material(curShader) {hideFlags = HideFlags.HideAndDontSave};
                return _curMaterial;
            }
        }

        private void Start()
        {
            if (SystemInfo.supportsImageEffects == false)
            {
                enabled = false;
                return;
            }
            if (curShader != null && curShader.isSupported == false)
            {
                enabled = false;
            }
        }

        private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if (curShader != null)
            {
                Material.SetFloat(LuminosityAmount, grayScaleAmount);

                Graphics.Blit(sourceTexture, destTexture, Material);
            }
            else
            {
                Graphics.Blit(sourceTexture, destTexture);
            }
        }

        private void OnDisable()
        {
            if (_curMaterial != null)
            {
                DestroyImmediate(_curMaterial);
            }
        }
    }
}
