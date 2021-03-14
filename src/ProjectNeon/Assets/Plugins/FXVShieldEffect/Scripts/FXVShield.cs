using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace FXV
{
    public enum FXVRenderSidesOptions
    {
        OUTSIDE_ONLY = 0,
        OUTSIDE_AND_INSIDE,
        INSIDE_ONLY
    };

    public class FXVShield : MonoBehaviour
    {
        public bool shieldActive = true;
        public float shieldActivationSpeed = 1.0f;
        private float shieldActivationRim = 0.2f;

        public float hitEffectDuration = 0.5f;

        public Light shieldLight;
        public Material hitMaterial;
        public Color hitColor;
        public bool autoHitPatternScale = true;

        public FXVRenderSidesOptions renderSides = FXVRenderSidesOptions.OUTSIDE_ONLY;

        private Color lightColor;

        private GameObject inside = null;
        private Renderer insideRenderer = null;

        private Material baseMaterial;
        private Material activationMaterial;
        private Material postprocessMaterial;
        private Material postprocessActivationMaterial;

        private Collider myCollider;
        private CommandBuffer cmdBuffer;
        private Renderer myRenderer;

        private float shieldActivationTime;
        private float shieldActivationDir;

        private int activationTimeProperty;
        private int shieldDirectionProperty;

        private int mainColorProperty;
        private int texColorProperty;
        private int patternColorProperty;

        private int currentHitIndex = 1;

        private static Dictionary<string, Mesh> insideMeshes = new Dictionary<string, Mesh>();

        void Awake()
        {
            myRenderer = GetComponent<Renderer>();
            activationTimeProperty = Shader.PropertyToID("_ActivationTime");
            shieldDirectionProperty = Shader.PropertyToID("_ShieldDirection");

            mainColorProperty = Shader.PropertyToID("_Color");
            texColorProperty = Shader.PropertyToID("_TextureColor");
            patternColorProperty = Shader.PropertyToID("_PatternColor");

            if (Camera.main)
            {
                FXVShieldPostprocess shieldPostrocess = Camera.main.GetComponent<FXVShieldPostprocess>();
                if (shieldPostrocess)
                    shieldPostrocess.AddShield(this);
            }

            shieldActivationDir = 0.0f;

            if (shieldLight)
                lightColor = shieldLight.color;

            myCollider = transform.GetComponent<Collider>();

            if (shieldActive)
            {
                shieldActivationTime = 1.0f;
                myCollider.enabled = true;
            }
            else
            {
                shieldActivationTime = 0.0f;
                myCollider.enabled = false;
            }

            if (shieldLight)
                shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);


            if (renderSides == FXVRenderSidesOptions.OUTSIDE_AND_INSIDE || renderSides == FXVRenderSidesOptions.INSIDE_ONLY)
            {
                inside = new GameObject("inside");
                inside.transform.parent = transform;
                inside.transform.localPosition = Vector3.zero;
                inside.transform.localScale = Vector3.one;
                inside.transform.localRotation = Quaternion.identity;
                inside.layer = gameObject.layer;

                inside.AddComponent<MeshRenderer>();
                MeshFilter mf = inside.AddComponent<MeshFilter>();

                Mesh baseMesh = gameObject.GetComponent<MeshFilter>().mesh;

                string insideMeshName = baseMesh.name + "_inside";

                Mesh flippedMesh = null;
                if (insideMeshes.ContainsKey(insideMeshName))
                    flippedMesh = insideMeshes[insideMeshName];
                else
                {
                    //Debug.Log("create inside mesh " + insideMeshName);

                    flippedMesh = Instantiate(baseMesh);

                    Vector3[] normals = flippedMesh.normals;
                    for (int i = 0; i < normals.Length; i++)
                        normals[i] = -normals[i];
                    flippedMesh.normals = normals;

                    for (int m = 0; m < flippedMesh.subMeshCount; m++)
                    {
                        int[] triangles = flippedMesh.GetTriangles(m);
                        for (int i = 0; i < triangles.Length; i += 3)
                        {
                            int temp = triangles[i + 0];
                            triangles[i + 0] = triangles[i + 1];
                            triangles[i + 1] = temp;
                        }
                        flippedMesh.SetTriangles(triangles, m);
                    }

                    insideMeshes.Add(insideMeshName, flippedMesh);
                }

                mf.mesh = flippedMesh;

                insideRenderer = inside.GetComponent<Renderer>();
                insideRenderer.sortingOrder = insideRenderer.sortingOrder - 1;

                MeshCollider mc = inside.AddComponent<MeshCollider>();
                mc.sharedMesh = flippedMesh;
            }

            if (renderSides == FXVRenderSidesOptions.INSIDE_ONLY)
                myRenderer.enabled = false;

            SetMaterial(myRenderer.material);
            SetHitMaterial(hitMaterial);
        }

        void OnDestroy()
        {
            if (Camera.main)
            {
                FXVShieldPostprocess shieldPostrocess = Camera.main.GetComponent<FXVShieldPostprocess>();
                if (shieldPostrocess)
                    shieldPostrocess.RemoveShield(this);
            }

            if (inside)
            {
                Destroy(inside.GetComponent<MeshFilter>().mesh);
            }

            if (baseMaterial)
                Destroy(baseMaterial);
            if (postprocessMaterial)
                Destroy(postprocessMaterial);
            if (postprocessActivationMaterial)
                Destroy(postprocessActivationMaterial);
            if (activationMaterial)
                Destroy(activationMaterial);
        }

        public void SetHitMaterial(Material newMat)
        {
            hitMaterial = newMat;

        }

        public void SetMaterial(Material newMat)
        {
            if (baseMaterial)
                Destroy(baseMaterial);
            if (postprocessMaterial)
                Destroy(postprocessMaterial);
            if (postprocessActivationMaterial)
                Destroy(postprocessActivationMaterial);
            if (activationMaterial)
                Destroy(activationMaterial);

            baseMaterial = new Material(newMat);
            baseMaterial.SetFloat(activationTimeProperty, 1.0f);

            postprocessMaterial = new Material(baseMaterial);
            List<string> keywords = new List<string>(postprocessMaterial.shaderKeywords);
            if (keywords.Contains("USE_REFRACTION"))
                keywords.Remove("USE_REFRACTION");
            if (keywords.Contains("ACTIVATION_EFFECT_ON"))
                keywords.Remove("ACTIVATION_EFFECT_ON");
            postprocessMaterial.shaderKeywords = keywords.ToArray();
            postprocessMaterial.SetVector(shieldDirectionProperty, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));

            postprocessActivationMaterial = new Material(baseMaterial);
            keywords = new List<string>(postprocessActivationMaterial.shaderKeywords);
            if (keywords.Contains("USE_REFRACTION"))
                keywords.Remove("USE_REFRACTION");
            postprocessActivationMaterial.shaderKeywords = keywords.ToArray();
            postprocessActivationMaterial.SetVector(shieldDirectionProperty, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));

            activationMaterial = new Material(baseMaterial);
            shieldActivationRim = activationMaterial.GetFloat("_ActivationRim");

            keywords = new List<string>(baseMaterial.shaderKeywords);
            if (keywords.Contains("ACTIVATION_EFFECT_ON"))
            {
                //Debug.Log("remove ACTIVATION_EFFECT_ON");
                keywords.Remove("ACTIVATION_EFFECT_ON");
            }
            baseMaterial.shaderKeywords = keywords.ToArray();
            myRenderer.sharedMaterial = baseMaterial;

            if (insideRenderer)
                insideRenderer.sharedMaterial = baseMaterial;

            SetShieldEffectDirection(new Vector3(1.0f, 0.0f, 0.0f));
        }

        public void SetMainColor(Color c)
        {
            activationMaterial.color = c;
            baseMaterial.color = c;
            myRenderer.sharedMaterial.color = c;
            postprocessMaterial.color = c;
            postprocessActivationMaterial.color = c;
        }

        public void SetTextureColor(Color c)
        {
            activationMaterial.SetColor(texColorProperty, c);
            baseMaterial.SetColor(texColorProperty, c);
            myRenderer.sharedMaterial.SetColor(texColorProperty, c);
            postprocessMaterial.SetColor(texColorProperty, c);
            postprocessActivationMaterial.SetColor(texColorProperty, c);
        }

        public void SetPatternColor(Color c)
        {
            activationMaterial.SetColor(patternColorProperty, c);
            baseMaterial.SetColor(patternColorProperty, c);
            myRenderer.sharedMaterial.SetColor(patternColorProperty, c);
            postprocessMaterial.SetColor(patternColorProperty, c);
            postprocessActivationMaterial.SetColor(patternColorProperty, c);
        }

        public void SetHitColor(Color c)
        {
            hitColor = c;
        }

        void Update()
        {
            if (shieldActivationDir > 0.0f)
            {
                shieldActivationTime += shieldActivationSpeed * Time.deltaTime;
                if (shieldActivationTime >= 1.0f)
                {
                    shieldActivationTime = 1.0f;
                    shieldActivationDir = 0.0f;
                    myRenderer.sharedMaterial = baseMaterial;
                    if (insideRenderer)
                        insideRenderer.sharedMaterial = baseMaterial;
                }

                if (shieldLight)
                    shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);
            }
            else if (shieldActivationDir < 0.0f)
            {
                shieldActivationTime -= shieldActivationSpeed * Time.deltaTime;
                if (shieldActivationTime <= -shieldActivationRim)
                {
                    shieldActivationTime = -shieldActivationRim;
                    shieldActivationDir = 0.0f;
                    myRenderer.enabled = false;
                    myRenderer.sharedMaterial = baseMaterial;
                    if (insideRenderer)
                    {
                        insideRenderer.enabled = false;
                        insideRenderer.sharedMaterial = baseMaterial;
                    }
                }

                if (shieldLight)
                    shieldLight.color = Color.Lerp(Color.black, lightColor, shieldActivationTime);
            }

            myRenderer.sharedMaterial.SetFloat(activationTimeProperty, shieldActivationTime);
            postprocessActivationMaterial.SetFloat(activationTimeProperty, shieldActivationTime);
        }

        public bool GetIsShieldActive()
        {
            return (shieldActivationTime == 1.0f) || (shieldActivationDir == 1.0f);
        }

        public bool GetIsDuringActivationAnim()
        {
            return shieldActivationDir != 0.0f;
        }

        public void SetShieldActive(bool active, bool animated = true)
        {
            if (animated)
            {
                shieldActivationDir = (active) ? 1.0f : -1.0f;
                if (activationMaterial)
                {
                    activationMaterial.SetFloat("_ActivationRim", shieldActivationRim);
                    activationMaterial.SetFloat(activationTimeProperty, shieldActivationTime);

                    postprocessActivationMaterial.SetFloat("_ActivationRim", shieldActivationRim);
                    postprocessActivationMaterial.SetFloat(activationTimeProperty, shieldActivationTime);

                    myRenderer.sharedMaterial = activationMaterial;
                    if (insideRenderer)
                        insideRenderer.sharedMaterial = activationMaterial;
                }

                if (active)
                {
                    myRenderer.enabled = (renderSides != FXVRenderSidesOptions.INSIDE_ONLY);
                    if (insideRenderer)
                        insideRenderer.enabled = true;
                }
            }
            else
            {
                shieldActivationTime = (active) ? 1.0f : 0.0f;
                shieldActivationDir = 0.0f;
                myRenderer.enabled = active;
                if (insideRenderer)
                    insideRenderer.enabled = active;
            }

            myCollider.enabled = active;
        }

        public void SetShieldEffectDirection(Vector3 dir)
        {
            Vector4 dir4 = new Vector4(dir.x, dir.y, dir.z, 0.0f);
            myRenderer.material.SetVector(shieldDirectionProperty, dir4);
            baseMaterial.SetVector(shieldDirectionProperty, dir4);
            activationMaterial.SetVector(shieldDirectionProperty, dir4);
            postprocessMaterial.SetVector(shieldDirectionProperty, dir4);
            postprocessActivationMaterial.SetVector(shieldDirectionProperty, dir4);
        }

        public void OnHit(Vector3 hitPos, float hitScale)
        {
            AddHitMeshAtPos(gameObject.GetComponent<MeshFilter>().mesh, hitPos, hitScale);

            if (renderSides == FXVRenderSidesOptions.OUTSIDE_AND_INSIDE || renderSides == FXVRenderSidesOptions.INSIDE_ONLY)
                AddHitMeshAtPos(insideRenderer.GetComponent<MeshFilter>().mesh, hitPos, hitScale);
        }

        private void AddHitMeshAtPos(Mesh mesh, Vector3 hitPos, float hitScale)
        {
            GameObject hitObject = new GameObject("hitFX");
            hitObject.transform.parent = transform;
            hitObject.transform.position = transform.position;
            hitObject.transform.localScale = Vector3.one;
            hitObject.transform.rotation = transform.rotation;

            Vector3 hitLocalSpace = transform.InverseTransformPoint(hitPos);

            Vector3 dir = hitLocalSpace.normalized;
            Vector3 tan1 = Vector3.up - dir * Vector3.Dot(dir, Vector3.up);
            tan1.Normalize();
            Vector3 tan2 = Vector3.Cross(dir, tan1);

            MeshRenderer mr = hitObject.AddComponent<MeshRenderer>();
            MeshFilter mf = hitObject.AddComponent<MeshFilter>();

            mf.mesh = mesh;
            mr.material = new Material(hitMaterial);

            mr.material.SetVector("_HitPos", hitLocalSpace);
            mr.material.SetVector("_HitTan1", tan1);
            mr.material.SetVector("_HitTan2", tan2);
            mr.material.SetFloat("_HitRadius", hitScale);
            mr.material.SetVector("_WorldScale", transform.lossyScale);
            mr.material.SetFloat("_HitShieldCovering", 1.0f);
            mr.material.renderQueue = mr.material.renderQueue + currentHitIndex;

            if (autoHitPatternScale)
            {
                if (myRenderer.material.HasProperty("_PatternScale"))
                    mr.material.SetFloat("_PatternScale", myRenderer.material.GetFloat("_PatternScale"));
                else
                    autoHitPatternScale = false;
            }
            mr.material.color = hitColor;

            FXVShieldHit hit = hitObject.AddComponent<FXVShieldHit>();
            hit.StartHitFX(hitEffectDuration);

            currentHitIndex++;
            if (currentHitIndex > 100)
                currentHitIndex = 1;
        }

        public void RenderPostprocess(CommandBuffer cmd)
        {
            if (myRenderer && gameObject.activeSelf && gameObject.activeInHierarchy)
            {
                if (myRenderer.enabled)
                    cmd.DrawRenderer(myRenderer, GetPostprocessMaterial());
                if (insideRenderer && insideRenderer.enabled)
                    cmd.DrawRenderer(insideRenderer, GetPostprocessMaterial());
            }
        }

        public Material GetPostprocessMaterial()
        {
            if (GetIsDuringActivationAnim())
                return postprocessActivationMaterial;

            return postprocessMaterial;
        }

        public Renderer GetRenderer()
        {
            return myRenderer;
        }
    }
}