using UnityEngine;

public class DemoItem : MonoBehaviour
{
    static Vector3 lookAtZ = new Vector3(0,0,1);

    void Update()
    {
        transform.LookAt(transform.position + lookAtZ);
    }
}
