using UnityEngine;

public class CardV2 : ScriptableObject
{
    [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private StringVariable typeDescription;
    [SerializeField] private StringVariable onlyPlayableByClass;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private Card classSpecial;
    [SerializeField] private CardActionV2[] sequencedActions;
}