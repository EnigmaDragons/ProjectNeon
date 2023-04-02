using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroFaceSelector : MonoBehaviour
{
    [SerializeField] private Image bustImage;
    [SerializeField] private Button nextHeroButton;
    [SerializeField] private Button previousHeroButton;
    [SerializeField] private Localize nameLocalize;
    [SerializeField] private Localize classLocalize;

    private IndexSelector<BaseHero> _heroes = new IndexSelector<BaseHero>(Array.Empty<BaseHero>());
    private Action<BaseHero> _onChanged = _ => { };

    private void Awake()
    {
        nextHeroButton.onClick.AddListener(() => MoveNext());
        previousHeroButton.onClick.AddListener(() => MovePrevious());
    }

    public void Init(BaseHero[] heroes, Action<BaseHero> onChanged)
    {
        _heroes = new IndexSelector<BaseHero>(heroes);
        _onChanged = onChanged;
        Render();
    }

    private void Render()
    {
        bustImage.sprite = _heroes.Current.Bust;
        nameLocalize.SetTerm(_heroes.Current.NameTerm());
        classLocalize.SetTerm(_heroes.Current.ClassTerm());
    }

    public void MoveNext()
    {
        _heroes.MoveNext();
        Render();
        _onChanged(_heroes.Current);
    }

    public void MovePrevious()
    {
        _heroes.MovePrevious();
        Render();
        _onChanged(_heroes.Current);
    }
}