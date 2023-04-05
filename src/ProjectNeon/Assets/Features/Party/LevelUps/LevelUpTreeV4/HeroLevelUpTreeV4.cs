using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUpsV4/LevelUpTree")]
public class HeroLevelUpTreeV4 : ScriptableObject
{
    [SerializeField] private CardType[] paragons;
    [SerializeField] private HeroLevelUpRewardV4[] rewards;
    
    public int MaxLevel => rewards.Length + 1;
    public int TotalHpGrowth => rewards.Sum(r => r.HpGain);
    public CardType[] ParagonCards => paragons.ToArray();
    
#if UNITY_EDITOR
    public HeroLevelUpRewardV4[] EditorRewards => rewards;
#endif

    public HeroLevelUpRewardV4 ForLevel(int level)
    {
        var index = level - 2; // First Level Up happens when you hit level 2. So, the 0 index corresponds to Level 2.
        return rewards[index];
    }
}