using System.Collections.Generic;
using UnityEngine;

public sealed class PartyBattleUIVisualizer : MonoBehaviour
{
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private HeroBattleUIPresenter heroPresenter;
    [SerializeField] private GameEvent setupAfter;
    [SerializeField] private GameEvent onCompleted;
    [SerializeField] private float ySpacing;
    [SerializeField] private float xSpacing;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    [ReadOnly, SerializeField] private List<HeroBattleUIPresenter> active = new List<HeroBattleUIPresenter>();

    private Party Party => partyArea.Party;
    
    private void OnEnable()
    {
        Message.Subscribe<MemberUnconscious>(ResolveUnconscious, this);
        setupAfter.Subscribe(Setup, this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        setupAfter.Unsubscribe(this);
    }

    void Setup()
    {
        var position = transform.position;
        for (var i = 0; i < Party.Heroes.Length; i++)
        {
            var h = Instantiate(heroPresenter,
                new Vector3(position.x + xOffset + xSpacing * i, position.y +yOffset + ySpacing * i, position.z), Quaternion.identity, gameObject.transform); 
            h.Set(Party.Heroes[i]);
            active.Add(h);
        }
        onCompleted.Publish();
    }

    private void ResolveUnconscious(MemberUnconscious m)
    {
        if (!m.Member.TeamType.Equals(TeamType.Party)) return;
        
        for (var i = 0; i < 3; i++)
            if (Party.Heroes[i].name.Equals(m.Member.Name))
                active[i].gameObject.SetActive(false);
    }
}
