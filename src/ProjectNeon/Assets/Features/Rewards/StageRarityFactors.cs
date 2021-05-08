using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage Rarity Factor")]
public class StageRarityFactors : ScriptableObject, RarityFactors
{
    [SerializeField] private int commonFactor;
    [SerializeField] private int uncommonFactor;
    [SerializeField] private int rareFactor;
    [SerializeField] private int epicFactor;

    public int this[Rarity r] => r switch
    {
        Rarity.Common => commonFactor,
        Rarity.Uncommon => uncommonFactor,
        Rarity.Rare => rareFactor,
        Rarity.Epic => epicFactor,
        _ => 0
    };
}