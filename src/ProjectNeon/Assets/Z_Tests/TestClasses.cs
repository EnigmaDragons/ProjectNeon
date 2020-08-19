
public static class TestClasses
{
    public static readonly CharacterClass Soldier = TestableObjectFactory.Create<CharacterClass>().Initialized("Soldier");
    public static readonly CharacterClass Paladin = TestableObjectFactory.Create<CharacterClass>().Initialized("Paladin");
}
