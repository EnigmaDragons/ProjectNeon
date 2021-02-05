using UnityEngine;

public class FollowMouseDemo : MonoBehaviour
{
    public ArrowRenderer arrowRenderer;
    public float distanceFromScreen = 5f;

    public void SetRenderer(ArrowRenderer value)
    {
        if (arrowRenderer)
            arrowRenderer.gameObject.SetActive(false);

        arrowRenderer = value;

        if (arrowRenderer)
            arrowRenderer.gameObject.SetActive(true);
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = distanceFromScreen;

        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        arrowRenderer.SetPositions(transform.position, worldMousePosition);
    }
}
