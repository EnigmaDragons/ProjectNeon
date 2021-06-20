using System;
using UnityEngine;
using UnityEngine.UI;

public class HeroFaceSelector : MonoBehaviour
{
    [SerializeField] private Image bustImage;
    [SerializeField] private Button nextHeroButton;
    [SerializeField] private Button previousHeroButton;

    private IndexSelector<HeroCharacter> _heroes = new IndexSelector<HeroCharacter>(Array.Empty<HeroCharacter>());
    private Action<HeroCharacter> _onChanged = _ => { };

    private void Awake()
    {
        nextHeroButton.onClick.AddListener(() => MoveNext());
        previousHeroButton.onClick.AddListener(() => MovePrevious());
    }

    public void Init(HeroCharacter[] heroes, Action<HeroCharacter> onChanged)
    {
        _heroes = new IndexSelector<HeroCharacter>(heroes);
        _onChanged = onChanged;
        bustImage.sprite = _heroes.Current.Bust;
    }
    
    public void MoveNext()
    {
        _heroes.MoveNext();
        bustImage.sprite = _heroes.Current.Bust;
        _onChanged(_heroes.Current);
    }

    public void MovePrevious()
    {
        _heroes.MovePrevious();
        bustImage.sprite = _heroes.Current.Bust;
        _onChanged(_heroes.Current);
    }
}