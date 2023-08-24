using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattleArrowsController : OnMessage<ShowEnemyDetails>
{
    [SerializeField] private BattleState battle;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Button previous;
    [SerializeField] private Button next;
    
    private Maybe<Member> _member = Maybe<Member>.Missing();

    private void Awake()
    {
        previous.onClick.AddListener(() =>
        {
            if (_member.IsMissing)
                return;
            var members = battle.Members.Where(x => x.Value.IsConscious()).OrderBy(x => x.Key).ToArray();
            var index = members.FirstIndexOf(x => x.Key == _member.Value.Id);
            if (index == 0)
                index = members.Length - 1;
            else if (index > 0)
                index = index - 1;
            else
                index = 0;
            ShowMember(members[index].Value);
        });
        next.onClick.AddListener(() =>
        {
            if (_member.IsMissing)
                return;
            var members = battle.Members.Where(x => x.Value.IsConscious()).OrderBy(x => x.Key).ToArray();
            var index = members.FirstIndexOf(x => x.Key == _member.Value.Id);
            if (index == members.Length - 1)
                index = 0;
            else if (index > -1)
                index = index + 1;
            else
                index = 0;
            ShowMember(members[index].Value);
        });
    }

    private void ShowMember(Member member)
    {
        if (member.TeamType == TeamType.Party)
        {
            var hero = party.Heroes.FirstOrMaybe(hero => hero.NameTerm.Equals(member.NameTerm));
            Message.Publish(new HideTooltip());
            Message.Publish(new HideEnemyDetails());
            Message.Publish(new ShowHeroDetailsView(hero.Value, member));
        }
        else
        {
            var enemy = battle.GetEnemyById(member.Id);
            Message.Publish(new ShowEnemyDetails(enemy, member));
        }
    }
    
    protected override void Execute(ShowEnemyDetails msg)
    {
        _member = msg.Member;
    }
}