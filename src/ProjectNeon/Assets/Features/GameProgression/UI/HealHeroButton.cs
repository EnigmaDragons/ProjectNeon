using UnityEngine;
using UnityEngine.UI;

public sealed class HealHeroButton : MonoBehaviour
{
    [SerializeField] private Party party;
    [SerializeField] private Button button;
    [SerializeField] private int cost;

    private Hero _hero;
    
    private void Awake() => button.onClick.AddListener(HealToFullIfCanAfford);

    public void Init(Hero h)
    {
        _hero = h;
    }

    private void HealToFullIfCanAfford()
    {
        if (party.Credits >= cost)
        {
            party.UpdateCreditsBy(-cost);
            party.HealHeroToFull(_hero);
            gameObject.SetActive(false);
        }
    }
}
