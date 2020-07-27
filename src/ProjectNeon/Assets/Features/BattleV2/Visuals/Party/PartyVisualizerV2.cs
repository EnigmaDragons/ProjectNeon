using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PartyVisualizerV2 : OnMessage<CharacterAnimationRequested, MemberUnconscious>
{
    [SerializeField] private BattleState state;
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
        
        var heroes = state.Party.Heroes;
        if (heroes.Length > 0)
            SetupHero(hero1, heroes[0]);
        if (heroes.Length > 1)
            SetupHero(hero2, heroes[1]);
        if (heroes.Length > 2)
            SetupHero(hero3, heroes[2]);
        state.PartyArea.WithUiPositions(new[] { hero1.transform, hero2.transform, hero3.transform });
        yield break;
    }

    public void AfterBattleStateInitialized()
    {
        _damage.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
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
        if (!state.IsHero(e.MemberId)) return;
        
        var hero = state.GetHeroById(e.MemberId);
        var animator = _animators[hero];
        if (animator == null)
            Debug.LogWarning($"No Animator found for {state.GetHeroById(e.MemberId).name}");
        else
            StartCoroutine(animator.PlayAnimationUntilFinished(e.Animation, elapsed =>
            {
                BattleLog.Write($"Finished {e.Animation} in {elapsed} seconds.");
                Message.Publish(new Finished<CharacterAnimationRequested>());
            }));
    }
    
    protected override void Execute(MemberUnconscious m)
    {
        if (!m.Member.TeamType.Equals(TeamType.Party)) return;
        
        // TODO: Handle custom unconscious animation 

        var t = state.GetTransform(m.Member.Id);
        t.DOPunchScale(new Vector3(8, 8, 8), 2, 1);
        t.DOSpiral(2);
        StartCoroutine(ExecuteAfterDelay(() => t.gameObject.SetActive(false), 2));
    }
    
    private IEnumerator ExecuteAfterDelay(Action a, float delay)
    {
        yield return new WaitForSeconds(delay);
        a();
    }
}
