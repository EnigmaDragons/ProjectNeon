using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class HoverCharacter : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Material _originalMaterial;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _originalMaterial = _renderer.material;
    }
    
    public void Set(Material material)
    {
        _renderer.material = material;
    }

    public void Revert()
    {
        _renderer.material = _originalMaterial;
    }
}
