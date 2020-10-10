using UnityEngine;
using UnityEngine.UI;

public class RandomSeed : MonoBehaviour
{
    //If you want to randomize UI Images, you'll need to create different materials
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Renderer r = GetComponent<Renderer>();
            if (r != null && r.material != null)
            {
                r.material.SetFloat("_RandomSeed", Random.Range(0, 1000f));
            }
            else Debug.LogError("Missing Renderer or Material: " + gameObject.name);
        }
        else
        {
            Image i = GetComponent<Image>();
            if (i != null)
            {
                if (i.material != null)
                {
                    i.material.SetFloat("_RandomSeed", Random.Range(0, 1000f));
                }
                else Debug.LogError("Missing Material on UI Image: " + gameObject.name);
            }
            else Debug.LogError("Missing Sprite Renderer or UI Image on: " + gameObject.name);
        }
    }
}