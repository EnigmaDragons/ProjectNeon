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
    [SerializeField] private TextMeshProUGUI classLabel;

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
        Render();
    }

    private void Render()
    {
        bustImage.sprite = _heroes.Current.Bust;
        nameLocalize.SetTerm(_heroes.Current.NameTerm());
        classLabel.text = _heroes.Current.Class;
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