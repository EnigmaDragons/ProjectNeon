using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ConstantlyOffsetMaterialTexture : MonoBehaviour
{
    [SerializeField] private float xSpeed = 0.5f;
    [SerializeField] private float ySpeed = 0.5f;
    
    private Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        _renderer.material.SetTextureOffset("_MainTex", new Vector2(Time.unscaledTime * xSpeed, Time.unscaledTime * ySpeed));
    }
}
