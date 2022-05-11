using System.Linq;
using UnityEngine;

public class EnabledInAdventureVersions : MonoBehaviour
{
    [SerializeField] private CurrentAdventureProgress adventure;
    [SerializeField] private GameAdventureProgressType[] enabledVersions;
    [SerializeField] private GameAdventureProgressType[] disabledVersions;
    [SerializeField] private GameObject[] targets;
    
    private void Awake()
    {
        var impliedTargets = targets.Any() ? targets : new [] { gameObject };

        var type = adventure.AdventureProgress.AdventureType;
        impliedTargets.ForEach(o =>
        {
            if (enabledVersions.Contains(type))
                o.SetActive(true);
            else if (disabledVersions.Contains(type))
                o.SetActive(false);
        });
    }
}
