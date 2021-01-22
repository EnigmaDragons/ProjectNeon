using NUnit.Framework;

namespace Z_Tests.Features.Effects
{
    public sealed class LifestealTests
    {
        [Test]
        public void Lifesteal_4Counters_HealsAttackerFullAmountOfAttack()
        {
            var attacker = TestMembers.Create(s => s
                .With(StatType.Attack, 2)
                .With(StatType.MaxHP, 10));
            
            var target = TestMembers.With(StatType.MaxHP, 10);
            
            attacker.State.Adjust(TemporalStatType.Lifesteal, 4); // 25% per lifesteal counter
            attacker.State.SetHp(2);
            
            TestEffects.Apply(new EffectData
            {
                EffectType = EffectType.Attack,
                FloatAmount = new FloatReference(1)
            }, attacker, target);

            Assert.AreEqual(0, attacker.State[TemporalStatType.Lifesteal]);
            Assert.AreEqual(4, attacker.CurrentHp());
            Assert.AreEqual(8, target.CurrentHp());
        }
        
        [Test]
        public void Lifesteal_1Counter_HealsAttackerQuarterOfDamagerDealt()
        {
            var attacker = TestMembers.Create(s => s
                .With(StatType.Attack, 4)
                .With(StatType.MaxHP, 10));
            
            var target = TestMembers.With(StatType.MaxHP, 10);
            
            attacker.State.Adjust(TemporalStatType.Lifesteal, 1); // 25% per lifesteal counter
            attacker.State.SetHp(2);
            
            TestEffects.Apply(new EffectData
            {
                EffectType = EffectType.Attack,
                FloatAmount = new FloatReference(1)
            }, attacker, target);

            Assert.AreEqual(0, attacker.State[TemporalStatType.Lifesteal]);
            Assert.AreEqual(3, attacker.CurrentHp());
            Assert.AreEqual(6, target.CurrentHp());
        }
    }
}
