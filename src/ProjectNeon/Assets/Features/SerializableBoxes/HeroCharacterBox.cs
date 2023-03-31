using System;

[Serializable]
public class HeroCharacterBox
{
    public BaseHero BaseHero;
    
    public HeroCharacterBox() {}

    public HeroCharacterBox(HeroCharacter heroCharacter)
    {
        Set(heroCharacter);
    }

    public bool IsBoxFilled() => Get() != null;
    
    public HeroCharacter Get()
    {
        if (BaseHero != null)
            return BaseHero;
        else return null;
    }

    public void Set(HeroCharacter heroCharacter)
    {
        if (heroCharacter == null || heroCharacter is InMemoryHeroCharacter)
            return;
        BaseHero = null;
        if (heroCharacter is BaseHero baseHero)
            BaseHero = baseHero;
    }
}