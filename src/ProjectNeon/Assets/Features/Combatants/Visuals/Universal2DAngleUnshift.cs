using UnityEngine;

public class Universal2DAngleUnshift : MonoBehaviour
{
    private void Awake()
    {
        var original = transform.rotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(-Universal2DAngleShift.angle * 2, original.y, original.z);
    }
}