using System;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/BattleRewardState")]
public class BattleRewardState : ScriptableObject
{
    [SerializeField, ReadOnly] private int rewardCredits;
    [SerializeField, ReadOnly] private CardType[] rewardCards = Array.Empty<CardType>();
    [SerializeField, ReadOnly] private StaticEquipment[] rewardEquipments = Array.Empty<StaticEquipment>();
    [SerializeField, ReadOnly] private int rewardXp = 0;
    
    public int RewardCredits => rewardCredits;
    public int RewardXp => rewardXp;
    public CardType[] RewardCards => rewardCards.ToArray();
    public StaticEquipment[] RewardEquipments => rewardEquipments.ToArray();
    
    public void Init()
    {
        rewardCredits = 0;
        rewardCards = Array.Empty<CardType>();
        rewardEquipments = Array.Empty<StaticEquipment>();
        rewardXp = 0;
    }

    public void Add(string rewardName, int amount)
    {
        if (rewardName.Equals("Credits"))
            AddRewardCredits(amount);
        if (rewardName.Equals("Xp"))
            AddRewardXp(amount);
    }
    public void AddRewardCredits(int amount) => UpdateState(() => rewardCredits += amount);
    public void AddRewardXp(int xp) => UpdateState(() => rewardXp += xp); 
    public void SetRewardCards(params CardType[] cards) => UpdateState(() => rewardCards = cards);
    public void SetRewardEquipment(params StaticEquipment[] equipments) => UpdateState(() => rewardEquipments = equipments);

    private void UpdateState(Action a)
    {
        a();
    }
    
    
    public static BattleRewardState InMemory() => (BattleRewardState)FormatterServices.GetUninitializedObject(typeof(BattleRewardState));
}
