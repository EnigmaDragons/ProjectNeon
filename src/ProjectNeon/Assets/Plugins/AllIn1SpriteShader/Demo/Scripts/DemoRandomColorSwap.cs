using UnityEngine;

public class DemoRandomColorSwap : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    private Material mat;
    private Texture texture;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            mat = GetComponent<Renderer>().material;
            mat.SetFloat("_Alpha", 1f);
            mat.SetColor("_Color", new Color(0.5f, 1f, 0f, 1f));
            mat.SetTexture("_MainTex", texture);

            InvokeRepeating("NewColor", 0.0f, 0.6f);
        }
    }

    void NewColor()
    {
        mat.SetColor("_ColorSwapRed", gradient.Evaluate(Random.value));
        mat.SetColor("_ColorSwapGreen", gradient.Evaluate(Random.value));
        mat.SetColor("_ColorSwapBlue", gradient.Evaluate(Random.value));
    }
}
