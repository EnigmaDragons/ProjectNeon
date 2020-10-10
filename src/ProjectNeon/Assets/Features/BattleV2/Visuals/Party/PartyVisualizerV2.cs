using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PartyVisualizerV2 : OnMessage<CharacterAnimationRequested, MemberUnconscious, HighlightCardOwner>
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject hero1;
    [SerializeField] private GameObject hero2;
    [SerializeField] private GameObject hero3;
    [SerializeField] private Material defaultSpriteMaterial;
    [SerializeField] private Material cardOwnerMaterial;

    private readonly List<GameObject> _heroes = new List<GameObject>();
    private readonly Dictionary<HeroCharacter, Animator> _animators = new Dictionary<HeroCharacter, Animator>();
    private readonly Dictionary<HeroCharacter, SpriteRenderer> _renderers = new Dictionary<HeroCharacter, SpriteRenderer>();
    private readonly Dictionary<HeroCharacter, DamageNumbersController> _damagesNew  = new Dictionary<HeroCharacter, DamageNumbersController>();
    
    public IEnumerator Setup()
    {
        _heroes.ForEach(Destroy);
        _heroes.Clear();
        
        var heroes = state.Party.BaseHeroes;
        if (heroes.Length > 0)
            SetupHero(hero1, heroes[0], 5);
        if (heroes.Length > 1)
            SetupHero(hero2, heroes[1], 9);
        if (heroes.Length > 2)
            SetupHero(hero3, heroes[2], 1);
        state.PartyArea.WithUiPositions(new[] { hero1.transform, hero2.transform, hero3.transform });
        yield break;
    }

    public void AfterBattleStateInitialized()
    {
        _damagesNew.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
    }

    private void SetupHero(GameObject heroOrigin, HeroCharacter hero, int visualOrder)
    {
        var hasBody = !hero.Body.name.Equals("BodyPlaceholder");
        if (hasBody)
        {
             var character = Instantiate(hero.Body, heroOrigin.transform.position, Quaternion.identity, heroOrigin.transform);
             _heroes.Add(character);
             _animators[hero] = character.GetComponentInChildren<Animator>();
             _renderers[hero] = character.GetComponentInChildren<SpriteRenderer>();
             _renderers[hero].material = defaultSpriteMaterial;
             
             var damageEffectController = character.GetComponentInChildren<DamageNumbersController>();
             if (damageEffectController == null)
                 Debug.LogWarning($"{hero.Name} is missing a DamageNumbersController");
             else
                 _damagesNew[hero] = damageEffectController;

             character.GetComponentInChildren<SpriteRenderer>().sortingOrder = visualOrder;
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
            Debug.LogWarning($"No Animator found for {state.GetHeroById(e.MemberId).Name}");
        else
            StartCoroutine(animator.PlayAnimationUntilFinished(e.Animation.AnimationName, elapsed =>
            {
                BattleLog.Write($"Finished {e.Animation} Animation in {elapsed} seconds.");
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

    protected override void Execute(HighlightCardOwner msg)
    { 
        _renderers.ForEach(kv =>
        {
            kv.Value.material = msg.Member.Name == kv.Key.Name
                ? cardOwnerMaterial
                : defaultSpriteMaterial;
        });
    }

    private IEnumerator ExecuteAfterDelay(Action a, float delay)
    {
        yield return new WaitForSeconds(delay);
        a();
    }
}
