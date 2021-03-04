using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Funly.DSS {
  // If you need to move/animate the moon at runtime... you need to use this class
  // since it calculates some precomputed data for the shader to keep rendering fast.
  // You can animate by lerp'ing the vertical and horizonal percentage properties. If you
  // are NOT moving the mooon at runtime, you don't need this script.. just adjust the shader
  // properties for the moon, and disable (or remove) this script entirely.
  public class MoonPositionHelper : MonoBehaviour {
    [Tooltip("Adjust vertical position of moon at runtime. This value will override the shader value.")]
    [Range(0, 1)]
    public float verticalPosition;

    [Tooltip("Adjust horizontal position of moon at runtime. This value will override the shader value.")]
    [Range(0, 1)]
    public float horizontalPosition;

    private float currentVerticalPosition;
    private float currentHorizontalPosition;

    public Material moonMaterial;

    private void Start()
    {
      if (moonMaterial == null) {
        // Try and find a decent skybox material to use.
        if (GetComponent<MeshRenderer>()) {
          moonMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        } else if (RenderSettings.skybox != null) {
          moonMaterial = RenderSettings.skybox;
        }
      }

      if (moonMaterial == null) {
        Debug.LogError("Can't use MoonPositionHelper without a reference to skybox material");
        this.gameObject.SetActive(false);
      }


    }

    // Update is called once per frame
    void Update () {
      UpdateMoonData();

      if (currentVerticalPosition != verticalPosition ||
          currentHorizontalPosition != horizontalPosition) {
        if (moonMaterial == null) {
          Debug.LogError("Can't animate moon position without material reference");
          this.gameObject.SetActive(false);
          return;
        }

        UpdateMoonData();
        currentVerticalPosition = verticalPosition;
        currentHorizontalPosition = horizontalPosition;
      }
    }

    public void UpdateMoonData()
    {
      // Convert our percent properties to spherical point.
      float moonYPosition = SphereUtility.PercentToHeight(verticalPosition);
      float moonAngle = SphereUtility.PercentToRadAngle(horizontalPosition);
      Vector3 moonPoint = SphereUtility.SphericalToPoint(moonYPosition, moonAngle);

      float xRotation = 0;
      float yRotation = 0;

      SphereUtility.CalculateStarRotation(moonPoint, out xRotation, out yRotation);


      Vector4 moonPositionData = new Vector4(moonPoint.x, moonPoint.y, moonPoint.z, 0);
      Vector4 moonRotationData = new Vector4(xRotation, yRotation, 0, 0);

      moonMaterial.SetVector("_MoonComputedPositionData", moonPositionData);
      moonMaterial.SetVector("_MoonComputedRotationData", moonRotationData);
    }
  }
}