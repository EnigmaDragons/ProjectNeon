using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Funly.DSS
{
  // Precompute data used by the shader to accelerate rendering.
  public class CPUStarDataRenderer : BaseStarDataRenderer
  {

    public override IEnumerator ComputeStarData()
    {
      SendProgress(0);

      // Create 1 large texture, then render smaller tiles into it.
      Texture2D tex = new Texture2D((int)imageSize, (int)imageSize, TextureFormat.RGBA32, false);

      // Random points on sphere surface to position stars.
      int tileSize = (int)imageSize;
      List<StarPoint> starPoints = GenerateRandomStarsPoints(density, tileSize, tileSize);

      SendProgress(0);

      // Fill in the image.
      for (int yIndex = 0; yIndex < tileSize; yIndex++)
      {
        float yPercent = (float)yIndex / (float)(tileSize - 1);
        float yPosition = SphereUtility.PercentToHeight(yPercent);

        for (int xIndex = 0; xIndex < tileSize; xIndex++)
        {
          float anglePercent = (float)xIndex / (float)(tileSize - 1);
          float angle = SphereUtility.PercentToRadAngle(anglePercent);

          Vector3 currentSpot = SphereUtility.SphericalToPoint(yPosition, angle);

          // Closest star to current spot.
          StarPoint star = NearestStarPoint(currentSpot, starPoints);

          UnityEngine.Color c = new UnityEngine.Color(
            PointAxisToPercent(star.position.x),
            PointAxisToPercent(star.position.y),
            PointAxisToPercent(star.position.z),
            star.noise); // Noise value used to randomize each star.

          tex.SetPixel(xIndex, yIndex, c);
        }

        // Update the GUI progress bar.
        float totalProgress = (float)((yIndex + 1) * tileSize) / (float)(tileSize * tileSize);
        SendProgress(totalProgress);

        yield return null;
      }

      tex.Apply(false);

      SendCompletion(tex, true);

      yield break;
    }

    private float PointAxisToPercent(float axis)
    {
      return Mathf.Clamp01((axis + 1.0f) / 2.0f);
    }

    List<StarPoint> GenerateRandomStarsPoints(float density, int imageWidth, int imageHeight)
    {
      int numStars = Mathf.FloorToInt((float)imageWidth * (float)imageHeight * Mathf.Clamp(density, 0, 1));
      List<StarPoint> stars = new List<StarPoint>(numStars + 1);

      float minDistance = starRadius * 2.01f;

      for (int i = 0; i < numStars; i++)
      {
        Vector3 pointOnSphere = UnityEngine.Random.onUnitSphere;

        bool toClose = false;
        for (int j = 0; j < stars.Count; j++)
        {
          if (Vector3.Distance(stars[j].position, pointOnSphere) < minDistance)
          {
            toClose = true;
            break;
          }
        }

        if (toClose)
        {
          continue;
        }

        StarPoint star = new StarPoint(
          pointOnSphere,
          Random.Range(.5f, 1.0f),
          0,
          0);

        stars.Add(star);
      }

      return stars;
    }

    StarPoint NearestStarPoint(Vector3 spot, List<StarPoint> starPoints)
    {
      StarPoint nearbyPoint = new StarPoint(Vector3.zero, 0, 0, 0);

      if (starPoints == null)
      {
        return nearbyPoint;
      }

      float nearbyDistance = -1.0f;

      for (int i = 0; i < starPoints.Count; i++)
      {
        StarPoint starPoint = starPoints[i];
        float distance = Vector3.Distance(spot, starPoint.position);
        if (nearbyDistance == -1 || distance < nearbyDistance)
        {
          nearbyPoint = starPoint;
          nearbyDistance = distance;
        }
      }

      return nearbyPoint;
    }
  }
}