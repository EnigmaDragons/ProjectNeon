using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/DeterminedNodeInfo")]
public class DeterminedNodeInfo : ScriptableObject
{
    [SerializeField] private Library library;
    
    [SerializeField] public Maybe<BaseHero[]> PickHeroes;
    [SerializeField] public Maybe<CardTypeData[]> CardShopSelection;
    [SerializeField] public Maybe<ImplantData[]> Implants;
    [SerializeField] public Maybe<int[]> BlessingIds;
    [SerializeField] public Maybe<CardTypeData[]> CardRewardOptions;

    public void InitNewAdventure()
    {
        PickHeroes = Maybe<BaseHero[]>.Missing();
        CardShopSelection = Maybe<CardTypeData[]>.Missing();
        Implants = Maybe<ImplantData[]>.Missing();
        BlessingIds = Maybe<int[]>.Missing();
        CardRewardOptions = Maybe<CardTypeData[]>.Missing();
    }
    
    public DeterminedData GetData()
        => new DeterminedData
            {
                PickHeroIds = PickHeroes == null || PickHeroes.IsMissing ? Maybe<int[]>.Missing() : PickHeroes.Value.Select(x => x.Id).ToArray(),
                CardShopSelectionIds = CardShopSelection == null || CardShopSelection.IsMissing ? Maybe<int[]>.Missing() : CardShopSelection.Value.Select(x => x.Id).ToArray(),
                Implants = Implants == null || Implants.IsMissing ? Maybe<ImplantData[]>.Missing() : Implants.Value.Select(x => x).ToArray(),
                BlessingIds = BlessingIds == null || BlessingIds.IsMissing ? Maybe<int[]>.Missing() : BlessingIds.Value.Select(x => x).ToArray(),
                CardRewardIds = CardRewardOptions == null || CardRewardOptions.IsMissing ? Maybe<int[]>.Missing() : CardRewardOptions.Value.Select(x => x.Id).ToArray(),
            };

    public bool SetData(DeterminedData data)
    {
        try
        {
            var pickHeroIds = data?.PickHeroIds ?? Maybe<int[]>.Missing();
            PickHeroes = pickHeroIds.IsMissing ? Maybe<BaseHero[]>.Missing() : pickHeroIds.Value.Select(id => library.UnlockedHeroes.First(hero => id == hero.Id)).ToArray();
            var cardIds = data?.CardShopSelectionIds ?? Maybe<int[]>.Missing();
            CardShopSelection = cardIds.IsMissing ? Maybe<CardTypeData[]>.Missing() : cardIds.Value.Select(x => library.GetCardById(x).Value).ToArray();
            var implants = data?.Implants ?? Maybe<ImplantData[]>.Missing();
            Implants = implants.IsMissing ? Maybe<ImplantData[]>.Missing() : implants.Value.Select(x => x).ToArray();
            var blessingIds = data?.BlessingIds ?? Maybe<int[]>.Missing();
            BlessingIds = blessingIds.IsMissing ? Maybe<int[]>.Missing() : blessingIds.Value.Select(x => x).ToArray();
            var cardRewardIds = data?.CardRewardIds ?? Maybe<int[]>.Missing();
            CardShopSelection = cardRewardIds.IsMissing ? Maybe<CardTypeData[]>.Missing() : cardRewardIds.Value.Select(x => library.GetCardById(x).Value).ToArray();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            InitNewAdventure();
            return false;
        }
    }
}