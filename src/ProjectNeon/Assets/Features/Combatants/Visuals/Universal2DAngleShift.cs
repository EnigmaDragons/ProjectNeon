using UnityEngine;

public class Universal2DAngleShift : MonoBehaviour
{
    [SerializeField] private bool useLocalRotation = true;
    
    public const float angle = 26;
    public static Quaternion Euler => Quaternion.Euler(angle, 0, 0);

    private void Awake() => SetRotation(Quaternion.Euler(angle, 0, 0));
    public void UseIdentityRotation() => transform.rotation = Quaternion.identity;

    private void SetRotation(Quaternion q)
    {
        if (useLocalRotation)
            transform.localRotation = q;
        else
            transform.rotation = q;
    }
}