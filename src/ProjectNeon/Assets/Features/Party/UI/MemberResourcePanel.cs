using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemberResourcePanel : MonoBehaviour
{
    [SerializeField] private Image resource1Icon;
    [SerializeField] private TextMeshProUGUI resource1Text;
    [SerializeField] private Image resource2Icon;
    [SerializeField] private TextMeshProUGUI resource2Text;
    [SerializeField] private Color buffColor = Color.green;
    [SerializeField] private Color debuffColor = Color.red;

    public MemberResourcePanel Initialized(Hero h)
    {
        var resourceTypes = h.Stats.ResourceTypes;
        var baseResourceTypes = h.BaseStats.ResourceTypes;
        
        resource1Icon.gameObject.SetActive(resourceTypes.Length > 0);
        resource1Text.gameObject.SetActive(resourceTypes.Length > 0);
        if (resourceTypes.Length > 0 && baseResourceTypes.Length > 0)
        {
            
            resource1Icon.sprite = resourceTypes[0].Icon;
            resource1Text.text = $"{resourceTypes[0].StartingAmount}/{resourceTypes[0].MaxAmount}";
            resource1Text.color = ColorFor(((resourceTypes[0].StartingAmount - baseResourceTypes[0].StartingAmount) * 10) + (resourceTypes[0].MaxAmount - baseResourceTypes[0].MaxAmount));
        }
        
        var hasSecondResource = resourceTypes.Length > 1 && baseResourceTypes.Length > 1;
        resource2Icon.gameObject.SetActive(hasSecondResource);
        resource2Text.gameObject.SetActive(hasSecondResource);
        if (hasSecondResource)
        {
            resource2Icon.sprite = resourceTypes[1].Icon;
            resource2Text.text = $"{resourceTypes[1].StartingAmount}/{resourceTypes[1].MaxAmount}";
            resource2Text.color = ColorFor(((resourceTypes[1].StartingAmount - baseResourceTypes[1].StartingAmount) * 10) + (resourceTypes[1].MaxAmount - baseResourceTypes[1].MaxAmount));
        }
        return this;
    }
    
    private Color ColorFor(float delta)
    {
        if (delta < 0)
            return debuffColor;
        if (delta > 0)
            return buffColor;
        return Color.white;
    }
}