using UnityEngine;

public class CharacterAnimationTester : MonoBehaviour
{
    [SerializeField] private GameObject heroLocation;
    [SerializeField] private GameObject enemyLocation;
    [SerializeField] private GameObject notEnabledForSocialMedia;
    
    [SerializeField] private BaseHero hero;
    [SerializeField] private bool useHero;
    [SerializeField] private Enemy enemy;
    
    [SerializeField] private CharacterAnimationType animationType;
    [SerializeField] private CharacterAnimationSoundSet soundSetOverride;

    private int _memberId = -1;
    private BaseHero _hero;
    private Enemy _enemy;
    private GameObject _character;
    
    public void Play()
    {
        SetupCharacter();

        var source = useHero ? _hero.AsMemberForLibrary() : enemy.ForStage(0).AsMember(_memberId);
        Message.Publish(new CharacterAnimationRequested2(_memberId, animationType)
        {
            Source = source,
            Target = new NoTarget(),
            Card = Maybe<Card>.Missing(),
            Condition = Maybe<EffectCondition>.Missing(),
            XPaidAmount = ResourceQuantity.None
        });
    }

    private void SetupCharacter()
    {
        if (useHero)
            SetupHero();
        else
            SetupEnemy();
    }

    private void SetupEnemy()
    {
        if (_enemy == enemy)
            return;
    
        ClearStage();
        if (!enemy.IsAllowedForSocialMedia)
        {
            ShowNotAllowed();
            return;
        }

        _enemy = enemy;
        _character = Instantiate(enemy.Prefab, enemyLocation.transform);
        InitCharacterAnimations(enemy.Animations, TeamType.Enemies);
        InitCharacterSounds(enemy.AnimationSounds);
    }
    
    private void SetupHero()
    {
        if (_hero == hero)
            return;
        
        ClearStage();
        if (!hero.IsAllowedForSocialMedia)
        {
            ShowNotAllowed();
            return;
        }

        _hero = hero;
        _character = Instantiate(hero.Body, heroLocation.transform);
        InitCharacterAnimations(hero.Animations, TeamType.Party);
        InitCharacterSounds(hero.AnimationSounds);
    }

    private void InitCharacterAnimations(CharacterAnimations anims, TeamType team)
    {
        var ccAnimator = _character.GetComponentInChildren<CharacterCreatorAnimationController>();
        ccAnimator.Init(_memberId, anims, team);
    }

    private void InitCharacterSounds(CharacterAnimationSoundSet soundSet)
    {
        var sounds = _character.GetComponentInChildren<CharacterAnimationSoundPlayer>();
        if (sounds != null)
        {
            sounds.Init(_memberId, soundSetOverride != null ? soundSetOverride : soundSet, _character.transform);
            sounds.SetAlwaysActive(true);
        }
    }
    
    private void ClearStage()
    {
        _enemy = null;
        _hero = null;
        heroLocation.DestroyAllChildren();
        enemyLocation.DestroyAllChildren();
        if (notEnabledForSocialMedia != null)
            notEnabledForSocialMedia.SetActive(false);
    }

    private void ShowNotAllowed()
    {
        if (notEnabledForSocialMedia != null)
            notEnabledForSocialMedia.SetActive(true);
    }
}
