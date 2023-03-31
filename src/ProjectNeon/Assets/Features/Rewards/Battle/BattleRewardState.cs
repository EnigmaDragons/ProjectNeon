﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/BattleRewardState")]
public class BattleRewardState : ScriptableObject
{
    [SerializeField, ReadOnly] private int rewardCredits;
    [SerializeField, ReadOnly] private CardTypeDataBox[] rewardCards = Array.Empty<CardTypeDataBox>();
    [SerializeField, ReadOnly] private EquipmentBox[] rewardEquipments = Array.Empty<EquipmentBox>();
    [SerializeField, ReadOnly] private int rewardXp = 0;
    
    public int RewardCredits => rewardCredits;
    public int RewardXp => rewardXp;
    public CardTypeData[] RewardCards => rewardCards.FromBoxes();
    public Equipment[] RewardEquipments => rewardEquipments.FromBoxes();
    
    public void Init()
    {
        rewardCredits = 0;
        rewardCards = Array.Empty<CardTypeDataBox>();
        rewardEquipments = Array.Empty<EquipmentBox>();
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
    public void SetRewardCards(params CardTypeData[] cards) => UpdateState(() => rewardCards = cards.ToBoxes());
    public void SetRewardEquipment(params Equipment[] equipments) => UpdateState(() => rewardEquipments = equipments.ToBoxes());

    private void UpdateState(Action a)
    {
        a();
    }
    
    
    public static BattleRewardState InMemory() => (BattleRewardState)FormatterServices.GetUninitializedObject(typeof(BattleRewardState));
}
