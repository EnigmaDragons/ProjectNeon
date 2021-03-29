using UnityEngine;

namespace FlatKit {
    [ExecuteInEditMode, ImageEffectAllowedInSceneView, RequireComponent(typeof(Camera))]
    public class FogImageEffect : MonoBehaviour {
        public bool useDistance = true;
        public Gradient distanceGradient;
        public float near = 0;
        public float far = 100;
        [Range(0, 1)] public float distanceFogIntensity = 1.0f;
        public bool useDistanceFogOnSky = false;

        [Space] public bool useHeight = false;
        public Gradient heightGradient;
        public float low = 0;
        public float high = 10;
        [Range(0, 1)] public float heightFogIntensity = 1.0f;
        public bool useHeightFogOnSky = false;

        [Space] [Range(0, 1)] public float distanceHeightBlend = 0.5f;

        [HideInInspector]
        public Material material;
        
        private Camera _camera;
        private Texture2D _lutDepth;
        private Texture2D _lutHeight;

        private static readonly string ShaderName = "Hidden/FogPlus";
        private static readonly int DistanceLut = Shader.PropertyToID("_DistanceLUT");
        private static readonly int Near = Shader.PropertyToID("_Near");
        private static readonly int Far = Shader.PropertyToID("_Far");
        private static readonly int UseDistanceFog = Shader.PropertyToID("_UseDistanceFog");
        private static readonly int UseDistanceFogOnSky = Shader.PropertyToID("_UseDistanceFogOnSky");
        private static readonly int DistanceFogIntensity = Shader.PropertyToID("_DistanceFogIntensity");
        private static readonly int HeightLut = Shader.PropertyToID("_HeightLUT");
        private static readonly int LowWorldY = Shader.PropertyToID("_LowWorldY");
        private static readonly int HighWorldY = Shader.PropertyToID("_HighWorldY");
        private static readonly int UseHeightFog = Shader.PropertyToID("_UseHeightFog");
        private static readonly int UseHeightFogOnSky = Shader.PropertyToID("_UseHeightFogOnSky");
        private static readonly int HeightFogIntensity = Shader.PropertyToID("_HeightFogIntensity");
        private static readonly int DistanceHeightBlend = Shader.PropertyToID("_DistanceHeightBlend");

        void Awake() {
            material = new Material(Shader.Find(ShaderName));
            _camera = GetComponent<Camera>();
            _camera.depthTextureMode = DepthTextureMode.Depth;
            Debug.Assert(_camera.depthTextureMode != DepthTextureMode.None);
        }

        private void Start() {
            UpdateShader();
        }

        void OnValidate() {
            if (material == null) {
                material = new Material(Shader.Find(ShaderName));
            }
            
            UpdateShader();
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (material == null) {
                material = new Material(Shader.Find(ShaderName));
                UpdateShader();
            }

#if UNITY_EDITOR
            UpdateShader();
#endif

            Graphics.Blit(source, destination, material);
        }

        private void UpdateShader() {
            UpdateDistanceLut();
            material.SetTexture(DistanceLut, _lutDepth);
            material.SetFloat(Near, near);
            material.SetFloat(Far, far);
            material.SetFloat(UseDistanceFog, useDistance ? 1f : 0f);
            material.SetFloat(UseDistanceFogOnSky, useDistanceFogOnSky ? 1f : 0f);
            material.SetFloat(DistanceFogIntensity, distanceFogIntensity);

            UpdateHeightLut();
            material.SetTexture(HeightLut, _lutHeight);
            material.SetFloat(LowWorldY, low);
            material.SetFloat(HighWorldY, high);
            material.SetFloat(UseHeightFog, useHeight ? 1f : 0f);
            material.SetFloat(UseHeightFogOnSky, useHeightFogOnSky ? 1f : 0f);
            material.SetFloat(HeightFogIntensity, heightFogIntensity);

            material.SetFloat(DistanceHeightBlend, distanceHeightBlend);
        }

        private void UpdateDistanceLut() {
            if (distanceGradient == null) return;

            if (_lutDepth != null) {
                DestroyImmediate(_lutDepth);
            }

            const int width = 256;
            const int height = 1;
            _lutDepth = new Texture2D(width, height, TextureFormat.RGBA32, /*mipChain=*/false) {
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear
            };

            //22b5f7ed-989d-49d1-90d9-c62d76c3081a

            for (float x = 0; x < width; x++) {
                Color color = distanceGradient.Evaluate(x / (width - 1));
                for (float y = 0; y < height; y++) {
                    _lutDepth.SetPixel(Mathf.CeilToInt(x), Mathf.CeilToInt(y), color);
                }
            }

            _lutDepth.Apply();
        }

        private void UpdateHeightLut() {
            if (heightGradient == null) return;

            if (_lutHeight != null) {
                DestroyImmediate(_lutHeight);
            }

            const int width = 256;
            const int height = 1;
            _lutHeight = new Texture2D(width, height, TextureFormat.RGBA32, /*mipChain=*/false) {
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = FilterMode.Bilinear
            };

            for (float x = 0; x < width; x++) {
                Color color = heightGradient.Evaluate(x / (width - 1));
                for (float y = 0; y < height; y++) {
                    _lutHeight.SetPixel(Mathf.CeilToInt(x), Mathf.CeilToInt(y), color);
                }
            }

            _lutHeight.Apply();
        }
    }
}