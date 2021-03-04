using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Funly.DSS {

  // Custom editor for starry sky to compile in/out features and prepare data files.
  public class StarrySkyShaderEditor : ShaderGUI {
    private int imageSize = 256;
    private string progressTitle = "Dynamic Starry Sky";
    private string progressMessage = "Rebuilding star system...";
    private const int starLayersCount = 3;
    private int busyRenderingCount;

    // Material holding the shader.
    private Material material;

    // Metadata for tracking renderers and progress.
    private class StarRendererData : System.Object {
      public float progress;
      public bool layerEnabled;
      public float density;
      public string name;
      public string layerId;
      public bool isRendering;
      public float starRadius;
    }

    private Dictionary<BaseStarDataRenderer, StarRendererData> starData;
    private Dictionary<string, BaseStarDataRenderer> renderers;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
      material = materialEditor.target as Material;

      string[] keywords = material.shaderKeywords;
      List<MaterialProperty> allProps = new List<MaterialProperty>(properties);
      List<MaterialProperty> renderProps = new List<MaterialProperty>(properties);

      // Initialize the renderers.
      if (renderers == null) {
        BuildRenderers();
      }

      // Sync the tracking data on first update.
      if (starData == null) {
        UpdateAllTrackingData(allProps);
      }

      // Make rebuild button red to help user not miss it after changes.
      GUIStyle style = new GUIStyle(GUI.skin.button);
      if (DoesStarSystemNeedRebuilding(allProps)) {
        style.normal.textColor = Color.red;
      }

      // Rebuild our data images if necessary.
      if (GUILayout.Button("Rebuild Star System", style)) {
        if (busyRenderingCount == 0) {
          RenderStarSystem(allProps);
        }
      }

      // Skybox checkbox.
      bool useGradientSky = EditorGUILayout.Toggle("Use Gradient Background",
        DoesKeywordExists(keywords, "GRADIENT_BACKGROUND"));
      // enable or disable the keyword based on checkbox
      if (useGradientSky) {
        material.EnableKeyword("GRADIENT_BACKGROUND");
        RemovePropertiesWithString(renderProps, "_MainTex");
      } else {
        material.DisableKeyword("GRADIENT_BACKGROUND");
        RemovePropertiesWithString(renderProps, "_Gradient");
      }

      // Star Layer 1 Enable.
      bool starLayer1Enabled = CheckToggleFeature(
        material,
        keywords,
        renderProps,
        "Enable Star Layer 1",
        "STAR_LAYER_1",
        "_StarLayer1");
      SetStarLayerEnabled("StarLayer1", starLayer1Enabled);

      // Star Layer 2 Enable.
      bool starLayer2Enabled = CheckToggleFeature(
        material,
        keywords,
        renderProps,
        "Enable Star Layer 2",
        "STAR_LAYER_2",
        "_StarLayer2");
      SetStarLayerEnabled("StarLayer2", starLayer2Enabled);

      // Star Layer 3 Enable.
      bool starLayer3Enabled = CheckToggleFeature(
        material,
        keywords,
        renderProps,
        "Enable Star Layer 3",
        "STAR_LAYER_3",
        "_StarLayer3");
      SetStarLayerEnabled("StarLayer3", starLayer3Enabled);

      // Moon checkbox.
      bool useMoon = EditorGUILayout.Toggle("Enable Moon",
        DoesKeywordExists(keywords, "MOON"));
      if (useMoon) {
        material.EnableKeyword("MOON");
        UpdateMoonData(allProps);
      } else {
        material.DisableKeyword("MOON");
        RemovePropertiesWithString(renderProps, "_Moon");
      }

      // Hide all the precomputed data fields.
      RemovePropertiesWithString(renderProps, "Data");

      // Display the shader props in the UI.
      foreach (MaterialProperty prop in renderProps) {
        materialEditor.ShaderProperty(prop, prop.displayName);
      }
    }

    void UpdateMoonData(List<MaterialProperty> allProps)
    {
      MaterialProperty heightProp = GetPropertyWithName(allProps, "_MoonHeight");
      if (heightProp == null) {
        Debug.LogError("Missing moon height property");
        return;
      }

      MaterialProperty angleProp = GetPropertyWithName(allProps, "_MoonAngle");
      if (angleProp == null) {
        Debug.LogError("Missing moon angle property");
        return;
      }

      // Convert our percent properties to spherical point.
      float moonYPosition = SphereUtility.PercentToHeight(heightProp.floatValue);
      float moonAngle = SphereUtility.PercentToRadAngle(angleProp.floatValue);
      Vector3 moonPoint = SphereUtility.SphericalToPoint(moonYPosition, moonAngle);

      float xRotation = 0;
      float yRotation = 0;

      SphereUtility.CalculateStarRotation(moonPoint, out xRotation, out yRotation);

      MaterialProperty moonPositionProp = GetPropertyWithName(allProps, "_MoonComputedPositionData");
      if (moonPositionProp == null) {
        Debug.LogError("Can't update precomputed moon position without property");
        return;
      }

      moonPositionProp.vectorValue = new Vector4(moonPoint.x, moonPoint.y, moonPoint.z, 0);

      MaterialProperty moonRotationProp = GetPropertyWithName(allProps, "_MoonComputedRotationData");
      if (moonRotationProp == null) {
        Debug.LogError("Can't update precomputed moon rotation without property");
        return;
      }

      moonRotationProp.vectorValue = new Vector4(xRotation, yRotation, 0, 0);
    }

    void AddObjectToMaterial(UnityEngine.Object obj, Material mat)
    {
      string assetPath = AssetDatabase.GetAssetPath(mat);
      AssetDatabase.AddObjectToAsset(obj, assetPath);
      AssetDatabase.ImportAsset(assetPath);
    }

    StarRendererData GetDataForLayer(string layerName)
    {
      return starData[renderers[layerName]];
    }

    void SetStarLayerEnabled(string starLayerName, bool isEnabled)
    {
      starData[renderers[starLayerName]].layerEnabled = isEnabled;
    }

    void UpdateAllTrackingData(List<MaterialProperty> props)
    {
      if (starData == null) {
        starData = new Dictionary<BaseStarDataRenderer, StarRendererData>();
      }

      UpdateStarRendererTrackingData(renderers["StarLayer1"], "1", props);
      UpdateStarRendererTrackingData(renderers["StarLayer2"], "2", props);
      UpdateStarRendererTrackingData(renderers["StarLayer3"], "3", props);
    }

    bool DoesStarSystemNeedRebuilding(List<MaterialProperty> props)
    {
      // First flag everything that needs rendering (used for overall progress calculation).
      foreach (KeyValuePair<string, BaseStarDataRenderer> entry in renderers) {
        if (ShouldRebuildStarLayer(entry.Key, props)) {
          return true;
        }
      }
      return false;
    }

    void RenderStarSystem(List<MaterialProperty> props)
    {
      int renderCount = 0;
      // First flag everything that needs rendering (used for overall progress calculation).
      foreach (KeyValuePair<string, BaseStarDataRenderer> entry in renderers) {
        if (ShouldRebuildStarLayer(entry.Key, props)) {
          renderCount += 1;
        }
      }

      // If render count is 0, just rebuild everything. Else only build what needs it.
      foreach (KeyValuePair<string, BaseStarDataRenderer> entry in renderers) {
        StarRendererData starData = GetDataForLayer(entry.Key);
        if (renderCount == 0) {
          starData.isRendering = true;
        } else {
          starData.isRendering = ShouldRebuildStarLayer(entry.Key, props);
        }
      }

      UpdateAllTrackingData(props);


      // Render out the dirty items
      busyRenderingCount = 0;
      foreach (KeyValuePair<string, BaseStarDataRenderer> entry in renderers) {
        StarRendererData starData = GetDataForLayer(entry.Key);

        // Only render dirty frames if any.. if none.. let the user force rebuild the star system if they like.
        if (starData.isRendering) {
          entry.Value.imageSize = imageSize;
          entry.Value.density = starData.density;
          entry.Value.starRadius = starData.starRadius;
          busyRenderingCount += 1;
          EditorCoroutine.start(entry.Value.ComputeStarData());
        }
      }
    }
    
    bool ShouldRebuildStarLayer(string layerName, List<MaterialProperty> props)
    {
      StarRendererData data = GetDataForLayer(layerName);
      if (data == null) {
        Debug.LogError("Can't check if we should rebuild layer since there's no tracking data?!?");
        return false;
      }

      if (data.layerEnabled == false) {
        return false;
      }

      // Check if density setting changed.
      MaterialProperty densityProp = GetPropertyWithName(props, "_StarLayer" + data.layerId + "Density");
      if (densityProp.floatValue != data.density) {
        return true;
      }

      MaterialProperty dataProp = GetPropertyWithName(props, "_StarLayer" + data.layerId + "DataTex");
      if (dataProp != null && dataProp.textureValue == null) {
        return true;
      }

      MaterialProperty radiusProp = GetPropertyWithName(props, "_StarLayer" + data.layerId + "MaxRadius");
      if (Mathf.Abs(radiusProp.floatValue - data.starRadius) > .01f) {
        return true;
      }

      return false;
    }

    void BuildRenderers()
    {
      renderers = new Dictionary<string, BaseStarDataRenderer>();

      renderers.Add("StarLayer1", GetBestRendererForPlatform());
      renderers.Add("StarLayer2", GetBestRendererForPlatform());
      renderers.Add("StarLayer3", GetBestRendererForPlatform());
    }

    // Refresh the tracking data for a star rendering layer.
    void UpdateStarRendererTrackingData(BaseStarDataRenderer renderer, 
      string layerId, List<MaterialProperty> props)
    {
      StarRendererData data = null;
      if (starData.ContainsKey(renderer) == false) {
        data = new StarRendererData();
        starData[renderer] = data;      
      } else {
        data = starData[renderer];
      }

      MaterialProperty densityProp = GetPropertyWithName(props, "_StarLayer" + layerId + "Density");
      MaterialProperty radiusProp = GetPropertyWithName(props, "_StarLayer" + layerId + "MaxRadius");

      data.density = densityProp.floatValue;
      data.starRadius = radiusProp.floatValue;
      data.name = "Star layer " + layerId;
      data.layerId = layerId;
      data.progress = 0;
    }

    bool CheckToggleFeature(
        Material material,
        string[] keywords,
        List<MaterialProperty> renderProps,
        string toggleName,
        string keyword,
        string namePrefix) {
      EditorGUI.BeginChangeCheck();

      bool isFeatureEnabled = EditorGUILayout.Toggle(
        toggleName,
        DoesKeywordExists(keywords, keyword));

      EditorGUI.EndChangeCheck();

      // enable or disable the keyword based on checkbox
      if (isFeatureEnabled) {
        material.EnableKeyword(keyword);
      } else {
        material.DisableKeyword(keyword);
        RemovePropertiesWithString(renderProps, namePrefix);
      }

      return isFeatureEnabled;
    }

    bool DoesKeywordExists(string[] keywords, string key)
    {
      foreach (string aKey in keywords) {
        if (key == aKey) {
          return true;
        }
      }
      return false;
    }

    MaterialProperty GetPropertyWithName(List<MaterialProperty> props, string name)
    {
      foreach (MaterialProperty prop in props) {
        if (prop.name == name) {
          return prop;
        }
      }
      return null;
    }

    void RemovePropertiesWithString(List<MaterialProperty> props, string match)
    {
      List<MaterialProperty> propsToRemove = new List<MaterialProperty>();

      foreach (MaterialProperty prop in props) {
        if (prop.name.Contains(match)) {
          propsToRemove.Add(prop);
        }
      }

      foreach (MaterialProperty prop in propsToRemove) {
        props.Remove(prop);
      }
    }

    private BaseStarDataRenderer GetBestRendererForPlatform()
    {
      // TODO - Implement a compute shader version that runs faster.
      BaseStarDataRenderer r = new CPUStarDataRenderer();
      r.progressCallback += OnStarRenderingProgress;
      r.completionCallback += OnStarRenderingComplete;

      return r;
    }

    // Appends an identifiier for the star data file names.
    private string FileNameForRendererData(BaseStarDataRenderer renderer, string prefix, string ext)
    {
      StarRendererData data = starData[renderer];
      return prefix + data.layerId + "." + ext;
    }

    private float CalculateOverallRenderProgress()
    {
      float current = 0;
      float total = 0;

      foreach (KeyValuePair<BaseStarDataRenderer, StarRendererData> entry in starData) {
        if (entry.Value.isRendering) {
          total += 1;
          current += entry.Value.progress;
        }
      }

      return current / total;
    }

    private void OnStarRenderingProgress(BaseStarDataRenderer renderer, float progress)
    {
      // Update the tracking data.
      StarRendererData data = starData[renderer];
      data.progress = progress;

      // Update progress bar.
      EditorUtility.DisplayProgressBar(progressTitle, 
        progressMessage, CalculateOverallRenderProgress());
    }

    private void RemoveAllObjectsWithName(string textureName, Material mat)
    {
      string assetPath = AssetDatabase.GetAssetPath(material);
      UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
      foreach (UnityEngine.Object obj in objs) {
        if (obj == null) {
          continue;
        }

        if (obj.name == textureName) {
          UnityEngine.Object.DestroyImmediate(obj, true);
        }
      }

      AssetDatabase.ImportAsset(assetPath);
    }

    private void OnStarRenderingComplete(BaseStarDataRenderer renderer, Texture2D texture, bool success)
    {
      StarRendererData data = starData[renderer];

      texture.name = data.layerId;
      texture.filterMode = FilterMode.Point;
      texture.wrapMode = TextureWrapMode.Clamp;

      RemoveAllObjectsWithName(data.layerId, material);

      AddObjectToMaterial(texture, material);

      // Update the material with the new texture file.
      material.SetTexture("_StarLayer" + data.layerId + "DataTex", texture);

      busyRenderingCount -= 1;

      if (busyRenderingCount <= 0) {
        // Hide our progress bar.
        EditorUtility.ClearProgressBar();
      }
    }
  }

}