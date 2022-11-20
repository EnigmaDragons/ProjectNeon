using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyResourceInfoPresenter : MonoBehaviour
{
    [SerializeField] private GameObject startingResourcesParent;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI startingResourcesLabel;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject resourcesPerTurnParent;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI resourcesPerTurnLabel;
    [SerializeField] private Image resourceIcon2;

    public void Init(EnemyInstance e)
    {
        var resources = e.Stats.ResourceTypes;
        if (resources.Length == 0)
        {
            startingResourcesParent.SetActive(false);
            resourcesPerTurnParent.SetActive(false);
        }
        else
        {
            var primaryResource = resources[0];
            startingResourcesParent.SetActive(primaryResource.StartingAmount > 0);
            resourcesPerTurnParent.SetActive(e.ResourceGainPerTurn > 0);
            startingResourcesLabel.text = e.StartingResourceAmount.ToString();
            icon.sprite = primaryResource.Icon;
            resourceIcon2.sprite = primaryResource.Icon;
            resourcesPerTurnLabel.text = e.ResourceGainPerTurn.ToString();
        }
    }
}