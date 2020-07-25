using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyVisualizerV2 : OnBattleEvent<CharacterAnimationRequested>
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private GameObject hero1;
    [SerializeField] private GameObject hero2;
    [SerializeField] private GameObject hero3;

    private readonly List<GameObject> _heroes = new List<GameObject>();
    private readonly Dictionary<Hero, Animator> _animators = new Dictionary<Hero, Animator>();
    private readonly Dictionary<Hero, DamageEffect> _damage  = new Dictionary<Hero, DamageEffect>();
    
    public IEnumerator Setup()
    {
        _heroes.ForEach(Destroy);
        _heroes.Clear();
        
        var heroes = battleState.Party.Heroes;
        SetupHero(hero1, heroes[0]);
        SetupHero(hero2, heroes[1]);
        SetupHero(hero3, heroes[2]);
        battleState.PartyArea.WithUiPositions(new[] { hero1.transform, hero2.transform, hero3.transform });
        _damage.ForEach(x => x.Value.Init(battleState.GetMemberByHero(x.Key)));
        yield break;
    }

    private void SetupHero(GameObject heroOrigin, Hero hero)
    {
        var hasBody = !hero.Body.name.Equals("BodyPlaceholder");
        if (hasBody)
        {
             var character = Instantiate(hero.Body, heroOrigin.transform.position, Quaternion.identity, heroOrigin.transform);
             _heroes.Add(character);
             _animators[hero] = character.GetComponentInChildren<Animator>();
             
             var damageEffect = character.GetComponentInChildren<DamageEffect>();
             if (damageEffect != null)
                 _damage[hero] = damageEffect;
             else
                 Debug.LogWarning($"{hero.name} is missing DamageEffect");
        }
        else
        {
            heroOrigin.GetComponent<SpriteRenderer>().sprite = hero.Bust;
        }
    }

    protected override void Execute(CharacterAnimationRequested e)
    {
        if (!battleState.IsHero(e.MemberId)) return;
        
        var hero = battleState.GetHeroById(e.MemberId);
        var animator = _animators[hero];
        if (animator == null)
            Debug.LogWarning($"No Animator found for {battleState.GetHeroById(e.MemberId).name}");
        else
            animator.Play(e.Animation);
    }
}
