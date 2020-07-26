using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyVisualizerV2 : OnMessage<CharacterAnimationRequested>
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
        if (heroes.Length > 0)
            SetupHero(hero1, heroes[0]);
        if (heroes.Length > 1)
            SetupHero(hero2, heroes[1]);
        if (heroes.Length > 2)
            SetupHero(hero3, heroes[2]);
        battleState.PartyArea.WithUiPositions(new[] { hero1.transform, hero2.transform, hero3.transform });
        yield break;
    }

    public void AfterBattleStateInitialized()
    {
        _damage.ForEach(x => x.Value.Init(battleState.GetMemberByHero(x.Key)));
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
            StartCoroutine(PlayAnimation(animator, e.Animation));
    }

    private IEnumerator PlayAnimation(Animator animator, string animationName)
    {
        var elapsed = 0f; 
        var layer = 0;
        animator.Play(animationName, layer);
        
        yield return new WaitForSeconds(0.1f);
        elapsed += 0.1f;
        
        bool AnimationIsActive() => animator.GetCurrentAnimatorStateInfo(layer).IsName(animationName);
        bool AnimationIsStillPlaying() => animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f;
        while (AnimationIsActive() && AnimationIsStillPlaying())
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        BattleLog.Write($"Finished {animationName} in {elapsed} seconds.");
        Message.Publish(new Finished<CharacterAnimationRequested>());
    }
}
