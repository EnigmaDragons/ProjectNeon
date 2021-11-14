using System.Linq;
using UnityEngine;

public class SpawnPartyToMarkers : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Party party;
    [SerializeField] private bool executeOnAwake;

    private void Awake()
    {
        if (executeOnAwake)
            Execute(target != null ? target : gameObject);
    }

    public void Execute(GameObject setting)
    {
        var markers = setting.GetComponentsInChildren<CharacterSpawnMarker>().ToDictionary(c => c.Alias, c => c);
        markers.ForEach(m => m.Value.Clear());
        var heroes = party.Heroes;
        for (var i = 0; i < heroes.Length; i++)
            if (markers.TryGetValue($"Hero{i + 1}", out var c))
                SpawnAndInit(heroes[i], c);
    }

    private void SpawnAndInit(BaseHero hero, CharacterSpawnMarker c)
    {
        var h = c.SpawnTo(hero.Body);
        var ch = h.GetComponentInChildren<CutsceneCharacter>();
        ch.Init(c.Alias);
    }
}
