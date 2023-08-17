using UnityEngine;

public class LowQualityComponent : OnMessage<LowQualityModeChanged>
{
    [SerializeField] private Sprite lowQualitySprite;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject[] standardQualityObjects;
    [SerializeField] private GameObject[] lowQualityObjects;
    [SerializeField] private MonoBehaviour[] standardQualityScripts;
    [SerializeField] private MonoBehaviour[] lowQualityScripts;

    private void Awake()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        var lowQualityModeEnabled = CurrentLowQualityMode.IsEnabled;
        foreach (var o in standardQualityObjects)
            o.SetActive(!lowQualityModeEnabled);
        foreach (var o in lowQualityObjects)
            o.SetActive(lowQualityModeEnabled);
        
        foreach (var o in standardQualityScripts)
            o.enabled = !lowQualityModeEnabled;
        foreach (var o in lowQualityScripts)
            o.enabled = lowQualityModeEnabled;
        
        if (lowQualityModeEnabled && lowQualitySprite != null && spriteRenderer != null)
            spriteRenderer.sprite = lowQualitySprite;
    }

    protected override void Execute(LowQualityModeChanged msg) => UpdateState();
}