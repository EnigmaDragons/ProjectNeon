using UnityEngine;

public class CharacterSpawnMarker : MonoBehaviour
{
    [SerializeField] private StringReference characterAlias;
    [SerializeField] private Vector3 shadowAdjustment = Vector3.zero;
    [SerializeField] private Vector3 rotationAdjustment = Vector3.zero;

    public string Alias => characterAlias;

    public GameObject SpawnTo(GameObject o)
    {
        Clear();
        var g = Instantiate(o, transform);
        
        if (shadowAdjustment != Vector3.zero)
        {
            var shadow = g.transform.Find("HeroShadow");
            if (shadow != null)
            {
                var unshift = (Universal2DAngleUnshift)shadow.GetComponentInChildren(typeof(Universal2DAngleUnshift));
                if (unshift != null)
                    unshift.enabled = false;
                var originalAngles = shadow.localRotation.eulerAngles;
                shadow.localRotation = Quaternion.Euler(shadowAdjustment + originalAngles);
            }
        }

        g.gameObject.transform.localRotation = Quaternion.Euler(rotationAdjustment + g.gameObject.transform.localRotation.eulerAngles);
        
        return g;
    }

    public void Clear() => transform.DestroyAllChildrenImmediate();
}
