using UnityEngine;

public class OnDisabledAlsoDisable : MonoBehaviour
{
    [SerializeField] private GameObject subject;
    [SerializeField] private GameObject[] targets;

    private void Update()
    {
        if (subject.activeSelf)
            return;

        foreach (var t in targets)
        {
            if (t.activeSelf)
                t.SetActive(false);
        }
    }
}
