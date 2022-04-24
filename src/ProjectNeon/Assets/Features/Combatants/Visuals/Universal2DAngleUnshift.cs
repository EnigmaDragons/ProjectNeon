using UnityEngine;

public class Universal2DAngleUnshift : MonoBehaviour
{
    [SerializeField] private bool useLocalRotation = true;
    
    private void Awake()
    {
        var original = useLocalRotation ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
        if (useLocalRotation)
            transform.localRotation = Quaternion.Euler(-Universal2DAngleShift.angle, original.y, original.z);
        else
            transform.rotation = Quaternion.Euler(-Universal2DAngleShift.angle, original.y, original.z);
    }
}