using UnityEngine;

public class DebugPause : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Break();
    }
}
