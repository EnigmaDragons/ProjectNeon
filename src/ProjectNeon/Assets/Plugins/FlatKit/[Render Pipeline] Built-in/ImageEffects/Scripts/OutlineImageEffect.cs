using UnityEngine;

namespace FlatKit {
    [ExecuteInEditMode, ImageEffectAllowedInSceneView, RequireComponent(typeof(Camera))]
    public class OutlineImageEffect : MonoBehaviour {
        public Color edgeColor = Color.white;

        [Range(0, 5)] public int thickness = 1;

        [Space] public bool useDepth = true;
        public bool useNormals = false;

        [Header("Advanced settings")] [Space] public float minDepthThreshold = 0f;
        public float maxDepthThreshold = 0.25f;
        [Space] public float minNormalsThreshold = 0f;
        public float maxNormalsThreshold = 0.25f;

        [HideInInspector]
        public Material material;

        private Camera _camera;
        
        private static readonly string ShaderName = "Hidden/OutlinePlus";
        private static readonly int EdgeColorProperty = Shader.PropertyToID("_EdgeColor");
        private static readonly int ThicknessProperty = Shader.PropertyToID("_Thickness");
        private static readonly int DepthThresholdsProperty = Shader.PropertyToID("_DepthThresholds");
        private static readonly int NormalsThresholdsProperty = Shader.PropertyToID("_NormalsThresholds");

        private void Start() {
            material = new Material(Shader.Find(ShaderName));
            _camera = GetComponent<Camera>();
            UpdateShader();
        }

        void OnValidate() {
            if (material == null) {
                material = new Material(Shader.Find(ShaderName));
            }

            if (_camera == null) {
                _camera = GetComponent<Camera>();
            }

            UpdateShader();
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (material == null) {
                material = new Material(Shader.Find(ShaderName));
                UpdateShader();
            }

            if (_camera == null) {
                _camera = GetComponent<Camera>();
            }

#if UNITY_EDITOR
            minDepthThreshold = Mathf.Clamp(minDepthThreshold, 0f, maxDepthThreshold);
            maxDepthThreshold = Mathf.Max(0f, maxDepthThreshold);
            minNormalsThreshold = Mathf.Clamp(minNormalsThreshold, 0f, maxNormalsThreshold);
            maxNormalsThreshold = Mathf.Max(0f, maxNormalsThreshold);
            UpdateShader();
#endif

            Graphics.Blit(source, destination, material);
        }

        private void UpdateShader() {
            const string depthKeyword = "OUTLINE_USE_DEPTH";
            if (useDepth) {
                material.EnableKeyword(depthKeyword);
                _camera.depthTextureMode = DepthTextureMode.Depth;
            }
            else {
                material.DisableKeyword(depthKeyword);
            }

            const string normalsKeyword = "OUTLINE_USE_NORMALS";
            if (useNormals) {
                material.EnableKeyword(normalsKeyword);
                _camera.depthTextureMode = DepthTextureMode.DepthNormals;
            }
            else {
                material.DisableKeyword(normalsKeyword);
            }

            material.SetColor(EdgeColorProperty, edgeColor);
            material.SetFloat(ThicknessProperty, thickness);
            const float depthThresholdScale = 1e-3f;
            material.SetVector(DepthThresholdsProperty,
                new Vector2(minDepthThreshold, maxDepthThreshold) * depthThresholdScale);
            material.SetVector(NormalsThresholdsProperty,
                new Vector2(minNormalsThreshold, maxNormalsThreshold));
        }
    }
}