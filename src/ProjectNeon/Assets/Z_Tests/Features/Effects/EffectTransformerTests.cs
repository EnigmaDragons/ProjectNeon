using System;
using NUnit.Framework;

public class EffectTransformerTests
{
    [Test]
    public void ApplyToNonCard_EffectNotTransformed()
    {
        var member = TestMembers.Any();
        var transformer = new TestEffectTransformer(1, (_, __) => true);
        member.State.AddEffectTransformer(transformer);
        var effect = new EffectData() { EffectType = EffectType.Nothing };
        
        TestEffects.Apply(effect, member, member, Maybe<Card>.Missing());

        Assert.AreEqual(0, member.Attack());
    }

    [Test]
    public void ApplyToCard_EffectTransformed()
        => TestTransformer(true, 1, 1, 1);

    [Test]
    public void TwoEffectsAndOneCardAndOneUse_BothEffectsTrasnformed()
        => TestTransformer(true, 2, 1, 1, 1);

    [Test]
    public void TwoCardsAndOneUse_OnlyOneEffectTransformed()
        => TestTransformer(true, 1, 1, 1, 2);

    [Test]
    public void ShouldNotTransform_EffectNotTransformed()
        => TestTransformer(false, 0, 1, 1);

    private void TestTransformer(bool shouldTransform, int expectedEffectsModified, int uses, params int[] cardIds)
    {
        var member = TestMembers.Any();
        var transformer = new TestEffectTransformer(uses, (_, __) => shouldTransform);
        member.State.AddEffectTransformer(transformer);
        var effect = new EffectData() { EffectType = EffectType.Nothing };
        
        foreach (var id in cardIds)
            TestEffects.Apply(effect, member, member, new Card(id, member, new CardType()));

        Assert.AreEqual(expectedEffectsModified, member.Attack());
    }
}

public class TestEffectTransformer : EffectTransformerBase
{
    public TestEffectTransformer(int uses, Func<EffectData, EffectContext, bool> shouldTransform) : base(false, -1, uses, shouldTransform, (effect, context) =>
    {
        return new EffectData
        {
            EffectType = EffectType.AdjustStatAdditivelyFormula,
            EffectScope = new StringReference("Attack"),
            NumberOfTurns = new IntReference(-1),
            Formula = "1"
        };
    }) {}
    
    public override StatusDetail Status => new StatusDetail(StatusTag.None, "");
}
