using System.Collections;
using UnityEngine;

public class EnableAfterDelay : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private FloatReference delay = new FloatReference(1f);

    private void Awake() => StartCoroutine(Execute());
    
    private IEnumerator Execute()
    {
        yield return new WaitForSeconds(delay);
        target.SetActive(true);
    }
}
