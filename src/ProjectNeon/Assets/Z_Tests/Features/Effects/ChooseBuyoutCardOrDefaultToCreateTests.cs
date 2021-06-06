using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;

public class ChooseBuyoutCardOrDefaultToCreateTests
{
    [Test]
    public void ChooseBuyoutCardOrDefaultToCreate_HasBuyoutOptionForTheEnemyAndDefaultInSelection()
    {
        var enemy1 = new InMemoryEnemyType() {Name = "Lavos", PowerLevel = 999, Tier = EnemyTier.Elite};
        var enemy2 = new InMemoryEnemyType() {Name = "Frog", PowerLevel = 1, Tier = EnemyTier.Minion};
        var enemies = new Dictionary<int,EnemyType>{{1,enemy1},{2,enemy2}};
        var member1 = TestMembers.CreateEnemy(s => s.With(StatType.MaxHP, 10f));
        var member2 = TestMembers.CreateEnemy(s => s.With(StatType.MaxHP, 10f));
        var battleMembers = new Dictionary<int, Member>{{1,member1},{2,member2}};
        var templateCard = new InMemoryCard() { Name = "default" };
        var defaultCard = new InMemoryCard() { Name = "template", ActionSequences = new CardActionSequence[] { CardActionSequence.Create(Scope.One, Group.Self, ((CardActionsData)FormatterServices.GetUninitializedObject(typeof(CardActionsData))).Initialized(new CardActionV2(new EffectData { EffectType = EffectType.BuyoutEnemyById })), false) }};
        var allCards = new Dictionary<int, CardTypeData> { { 1, templateCard }, { 2, defaultCard } };
        var effectContext = new EffectContext(TestMembers.Any(), new Single(TestMembers.Any()), Maybe<Card>.Missing(), ResourceQuantity.None, 
            PartyAdventureState.InMemory(), new PlayerState(), battleMembers, CardPlayZones.InMemory, new UnpreventableContext(),
            new SelectionContext(), allCards, 0, 0, enemies);
        
        AllEffects.Apply(new EffectData
        {
            EffectType = EffectType.ChooseBuyoutCardsOrDefault,
            EffectScope = new StringReference("2,1"),
        }, effectContext);
        
        Assert.AreEqual(effectContext.Selections.CardSelectionOptions.Length, 3);
        Assert.True(effectContext.Selections.CardSelectionOptions.Any(x => x.Name == "default"));
        Assert.True(effectContext.Selections.CardSelectionOptions.Any(x => x.Name == "template" 
            && x.ActionSequences[0].cardActions.Actions[0].BattleEffect.EffectType == EffectType.BuyoutEnemyById 
            && x.ActionSequences[0].cardActions.Actions[0].BattleEffect.EffectScope.Value == "1"));
        Assert.True(effectContext.Selections.CardSelectionOptions.Any(x => x.Name == "template" 
            && x.ActionSequences[0].cardActions.Actions[0].BattleEffect.EffectType == EffectType.BuyoutEnemyById 
            && x.ActionSequences[0].cardActions.Actions[0].BattleEffect.EffectScope.Value == "2"));
        Assert.NotNull(effectContext.Selections.OnCardSelected);
    }
}