using UnityEngine;

public class Vector3Variable : ScriptableObject
{
    [SerializeField]
    private Vector3 value = Vector3.zero;

    public Vector3 Value
    {
        get { return value; }
        set { this.value = value; }
    }

    public void SetVector3(Vector3 vector3) => value = vector3;
}
