using UnityEngine;

public class CharacterAnimationTester : MonoBehaviour
{
    [SerializeField] private BaseHero hero;
    [SerializeField] private GameObject heroLocation;
    [SerializeField] private CharacterAnimationType animationType;
    [SerializeField] private CharacterAnimationSoundSet soundSetOverride;

    private int _memberId = -1;
    private BaseHero _hero;
    
    public void Play()
    {
        SetupHero();
        Message.Publish(new CharacterAnimationRequested2(_memberId, animationType)
        {
            Source = _hero.AsMemberForLibrary(),
            Target = new NoTarget(),
            Card = Maybe<Card>.Missing(),
            Condition = Maybe<EffectCondition>.Missing(),
            XPaidAmount = ResourceQuantity.None
        });
    }

    private void SetupHero()
    {
        if (_hero == hero)
            return;
        
        heroLocation.DestroyAllChildren();
        _hero = hero;
        var obj = Instantiate(hero.Body, heroLocation.transform);
        var ccAnimator = obj.GetComponentInChildren<CharacterCreatorAnimationController>();
        ccAnimator.Init(_memberId, hero.Animations, TeamType.Party);
        var sounds = obj.GetComponentInChildren<CharacterAnimationSoundPlayer>();
        if (sounds != null)
        {
            sounds.Init(_memberId, soundSetOverride != null ? soundSetOverride : hero.AnimationSounds, obj.transform);
            sounds.SetAlwaysActive(true);
        }
    }
}
