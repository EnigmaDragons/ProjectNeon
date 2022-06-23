using UnityEngine;

public class EnabledIfAdventureAllowsMapDeckbuilding : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private CurrentAdventure adventure;

    private void Awake() 
        => target.SetActive(adventure.Adventure == null || adventure.Adventure.MapDeckbuildingEnabled);
}
