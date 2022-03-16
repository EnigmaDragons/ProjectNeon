using UnityEngine;

public class HeroShowcaseTester : MonoBehaviour
{
    [SerializeField] private BaseHero hero;

    public void BeginShowcase() => Message.Publish(new BeginHeroShowcaseRequested(hero));
}