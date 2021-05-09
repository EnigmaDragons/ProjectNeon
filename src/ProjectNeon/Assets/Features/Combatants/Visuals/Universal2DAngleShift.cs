using UnityEngine;

public class Universal2DAngleShift : MonoBehaviour
{
    public const float angle = 26;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(angle, 0, 0);
    }
}