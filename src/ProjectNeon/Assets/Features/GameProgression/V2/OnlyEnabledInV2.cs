using System.Linq;
using UnityEngine;

public class OnlyEnabledInV2 : MonoBehaviour
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private GameObject[] targets;
    
    private void Awake()
    {
        var impliedTargets = targets.Any() ? targets : new [] { gameObject };
        if (!adventure.Adventure.IsV2)
            impliedTargets.ForEach(o => o.SetActive(false));
    }
}
