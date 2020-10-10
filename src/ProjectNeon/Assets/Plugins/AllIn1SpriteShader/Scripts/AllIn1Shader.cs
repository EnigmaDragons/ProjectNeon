using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
[AddComponentMenu("AllIn1SpriteShader/AddAllIn1Shader")]
public class AllIn1Shader : MonoBehaviour
{
    private Material currMaterial, prevMaterial;
    private bool matAssigned = false, destroyed = false;
    private enum AfterSetAction { Clear, CopyMaterial, Reset};

#if UNITY_EDITOR
    private static float timeLastReload = -1f;
    private void Start()
    {
        if(timeLastReload < 0) timeLastReload = Time.time;
    }

    private void Update()
    {
        if (matAssigned || Application.isPlaying || !gameObject.activeSelf) return;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Renderer r = GetComponent<Renderer>();
            if (r.sharedMaterial == null) return;
            if (r.sharedMaterial.name.Contains("Default")) MakeNewMaterial();
            else matAssigned = true;
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                if (img.material.name.Contains("Default")) MakeNewMaterial();
                else matAssigned = true;
            }
        }
    }
#endif

    public void MakeNewMaterial()
    {
        SetMaterial(AfterSetAction.Clear);
    }

    public void MakeCopy()
    {
        SetMaterial(AfterSetAction.CopyMaterial);
    }

    private void ResetAllProperties()
    {
        SetMaterial(AfterSetAction.Reset);
    }

    private void SetMaterial(AfterSetAction action)
    {
        Shader allIn1Shader = Resources.Load("AllIn1SpriteShader", typeof(Shader)) as Shader;
        if (!Application.isPlaying && Application.isEditor && allIn1Shader != null)
        {
            bool rendererExists = false;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                rendererExists = true;
                prevMaterial = new Material(GetComponent<Renderer>().sharedMaterial);
                currMaterial = new Material(allIn1Shader);
                GetComponent<Renderer>().sharedMaterial = currMaterial;
                GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
                matAssigned = true;
                DoAfterSetAction(action);
            }
            else
            {
                Image img = GetComponent<Image>();
                if (img != null)
                {
                    rendererExists = true;
                    prevMaterial = new Material(img.material);
                    currMaterial = new Material(allIn1Shader);
                    img.material = currMaterial;
                    img.material.hideFlags = HideFlags.None;
                    matAssigned = true;
                    DoAfterSetAction(action);
                }
            }
            if (!rendererExists)
            {
                MissingRenderer();
                return;
            }
            else
            {
                SetSceneDirty();
            }
        }
        else if (allIn1Shader == null)
        {
            Debug.LogError("Make sure the AllIn1SpriteShader file is inside the Resource folder!");
        }
    }

    private void DoAfterSetAction(AfterSetAction action)
    {
        switch (action)
        {
            case AfterSetAction.Clear:
                ClearAllKeywords();
                break;
            case AfterSetAction.CopyMaterial:
                currMaterial.CopyPropertiesFromMaterial(prevMaterial);
                break;
        }
    }

    public void TryCreateNew()
    {
        bool rendererExists = false;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            rendererExists = true;
            Renderer r = GetComponent<Renderer>();
            if (r != null && r.sharedMaterial != null && r.sharedMaterial.name.Contains("AllIn1"))
            {
                ResetAllProperties();
                ClearAllKeywords();
            }
            else
            {
                CleanMaterial();
                MakeNewMaterial();
            }
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                rendererExists = true;
                if (img.material.name.Contains("AllIn1"))
                {
                    ResetAllProperties();
                    ClearAllKeywords();
                }
                else MakeNewMaterial();
            }
        }
        if (!rendererExists)
        {
            MissingRenderer();
        }
        SetSceneDirty();
    }

    public void ClearAllKeywords()
    {
        SetKeyword("RECTSIZE_ON");
        SetKeyword("OFFSETUV_ON");
        SetKeyword("CLIPPING_ON");
        SetKeyword("POLARUV_ON");
        SetKeyword("TWISTUV_ON");
        SetKeyword("ROTATEUV_ON");
        SetKeyword("FISHEYE_ON");
        SetKeyword("PINCH_ON");
        SetKeyword("SHAKEUV_ON");
        SetKeyword("WAVEUV_ON");
        SetKeyword("ROUNDWAVEUV_ON");
        SetKeyword("DOODLE_ON");
        SetKeyword("ZOOMUV_ON");
        SetKeyword("FADE_ON");
        SetKeyword("TEXTURESCROLL_ON");
        SetKeyword("GLOW_ON");
        SetKeyword("OUTBASE_ON");
        SetKeyword("ONLYOUTLINE_ON");
        SetKeyword("OUTTEX_ON");
        SetKeyword("OUTDIST_ON");
        SetKeyword("DISTORT_ON");
        SetKeyword("WIND_ON");
        SetKeyword("GRADIENT_ON");
        SetKeyword("COLORSWAP_ON");
        SetKeyword("HSV_ON");
        SetKeyword("HITEFFECT_ON");
        SetKeyword("PIXELATE_ON");
        SetKeyword("NEGATIVE_ON");
        SetKeyword("COLORRAMP_ON");
        SetKeyword("GREYSCALE_ON");
        SetKeyword("POSTERIZE_ON");
        SetKeyword("BLUR_ON");
        SetKeyword("MOTIONBLUR_ON");
        SetKeyword("GHOST_ON");
        SetKeyword("INNEROUTLINE_ON");
        SetKeyword("ONLYINNEROUTLINE_ON");
        SetKeyword("HOLOGRAM_ON");
        SetKeyword("CHROMABERR_ON");
        SetKeyword("GLITCH_ON");
        SetKeyword("FLICKER_ON");
        SetKeyword("SHADOW_ON");
        SetKeyword("ALPHACUTOFF_ON");
        SetKeyword("CHANGECOLOR_ON");
        SetSceneDirty();
    }

    private void SetKeyword(string keyword, bool state = false)
    {
        if (destroyed) return;
        if (currMaterial == null)
        {
            FindCurrMaterial();
            if (currMaterial == null)
            {
                MissingRenderer();
                return;
            }
        }
        if (!state) currMaterial.DisableKeyword(keyword);
        else currMaterial.EnableKeyword(keyword);
    }

    private void FindCurrMaterial()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            currMaterial = GetComponent<Renderer>().sharedMaterial;
            matAssigned = true;
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                currMaterial = img.material;
                matAssigned = true;
            }
        }
    }

    public void CleanMaterial()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Sprites/Default"));
            matAssigned = false;
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                img.material = new Material(Shader.Find("Sprites/Default"));
                matAssigned = false;
            }
        }
        SetSceneDirty();
    }

    public void SaveMaterial()
    {
        #if UNITY_EDITOR
        string path = "Assets/AllIn1SpriteShader/Materials/";
        if (PlayerPrefs.HasKey("All1ShaderMaterials")) path = PlayerPrefs.GetString("All1ShaderMaterials") + "/";
        if (!System.IO.Directory.Exists(path))
        {
            EditorUtility.DisplayDialog("The desired folder doesn't exist",
                   "Go to Window -> AllIn1ShaderWindow and set a valid folder", "Ok");
            return;
        }
        path += gameObject.name;
        string fullPath = path + ".mat";
        if (System.IO.File.Exists(fullPath))
        {
            SaveMaterialWithOtherName(path);
        }
        else DoSaving(fullPath);
        SetSceneDirty();
        #endif
    }
    private void SaveMaterialWithOtherName(string path, int i = 1)
    {
        int number = i;
        string newPath = path + "_" + number.ToString();
        string fullPath = newPath + ".mat";
        if (System.IO.File.Exists(fullPath))
        {
            number++;
            SaveMaterialWithOtherName(path, number);
        }
        else
        {
            DoSaving(fullPath);
        }
    }

    private void DoSaving(string fileName)
    {
        #if UNITY_EDITOR
        bool rendererExists = false;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Material matToSave = null;
        Material createdMat = null;
        if (sr != null)
        {
            rendererExists = true;
            matToSave = GetComponent<Renderer>().sharedMaterial;
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                rendererExists = true;
                matToSave = img.material;
            }
        }
        if (!rendererExists)
        {
            MissingRenderer();
            return;
        }
        else
        {
            createdMat = new Material(matToSave);
            AssetDatabase.CreateAsset(createdMat, fileName);
            Debug.Log(fileName + " has been saved!");
        }
        if (sr != null)
        {
            sr.material = createdMat;
        }
        else
        {
            Image img = GetComponent<Image>();
            img.material = createdMat;
        }
        #endif
    }

    private void SetSceneDirty()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        #endif
    }

    private void MissingRenderer()
    {
        #if UNITY_EDITOR
        EditorUtility.DisplayDialog("Missing Renderer", "This GameObject (" +
                            gameObject.name + ") has no Sprite Renderer or UI Image component. This AllIn1Shader component will be removed.", "Ok");
        destroyed = true;
        DestroyImmediate(this);
        return;
        #endif
    }

    public void ToggleSetAtlasUvs(bool activate)
    {
        SetAtlasUvs atlasUvs = GetComponent<SetAtlasUvs>();
        if (activate)
        {
            if (atlasUvs == null) atlasUvs = gameObject.AddComponent<SetAtlasUvs>();
            atlasUvs.GetAndSetUVs();
            SetKeyword("ATLAS_ON", true);
        }
        else
        {
            if (atlasUvs != null)
            {
                atlasUvs.ResetAtlasUvs();
                DestroyImmediate(atlasUvs);
            }
            SetKeyword("ATLAS_ON", false);
        }
        SetSceneDirty();
    }
}