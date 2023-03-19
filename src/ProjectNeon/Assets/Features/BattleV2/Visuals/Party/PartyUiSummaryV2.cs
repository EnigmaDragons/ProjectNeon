using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyUiSummaryV2 : MonoBehaviour
{
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private HeroBattleUIPresenter heroPresenter;
    [SerializeField] private float ySpacing;
    [SerializeField] private float xSpacing;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    [ReadOnly, SerializeField] private List<HeroBattleUIPresenter> active = new List<HeroBattleUIPresenter>();

    private Party Party => partyArea.Party;

    private bool _primaryStatVisible = true;
    private bool _shieldsVisible = true;
    private bool _resourcesVisible = true;
    
    private void Awake()
    {
        Message.Subscribe<MemberUnconscious>(ResolveUnconscious, this);
        Message.Subscribe<SetHeroesUiVisibility>(Execute, this);
    }

    private void OnDestroy()
    {
        Message.Unsubscribe(this);
    }

    private void Execute(SetHeroesUiVisibility msg)
    {
        if (msg.Component == BattleUiElement.PrimaryStat)
            _primaryStatVisible = msg.ShouldShow;
        if (msg.Component == BattleUiElement.PlayerShields)
            _shieldsVisible = msg.ShouldShow;
        if (msg.Component == BattleUiElement.PlayerResources)
            _resourcesVisible = msg.ShouldShow;
    }

    public void Setup()
    {
        active.ForEach(Destroy);
        active.Clear();

        var heroes = Party.Heroes;
        var position = transform.position;
        for (var i = 0; i < heroes.Length; i++)
        {
            var h = Instantiate(heroPresenter,
                new Vector3(position.x + xOffset + xSpacing * i, position.y +yOffset + ySpacing * i, position.z), Quaternion.identity, gameObject.transform); 
            h.Set(heroes[ReverseIndex(i)], _primaryStatVisible, _resourcesVisible, _shieldsVisible);
            active.Add(h);
        }
    }

    private int ReverseIndex(int i) 
        => Party.Heroes.Length - 1 - i;
    
    private void ResolveUnconscious(MemberUnconscious m)
    {
        if (!m.Member.TeamType.Equals(TeamType.Party)) return;
        
        active
            .Where(a => a.Contains(m.Member.NameTerm))
            .ForEach(a => a.gameObject.SetActive(false));
    }
}
