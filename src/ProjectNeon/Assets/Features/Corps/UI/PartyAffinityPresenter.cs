using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyAffinityPresenter : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private AllCorps corps;
    [SerializeField] private GameObject corp1Parent;
    [SerializeField] private TextMeshProUGUI corp1AffinityLabel;
    [SerializeField] private CorpUiBase[] corp1Ui;
    [SerializeField] private Image corp1Logo;
    [SerializeField] private GameObject corp2Parent;
    [SerializeField] private TextMeshProUGUI corp2AffinityLabel;
    [SerializeField] private Image corp2Logo;
    [SerializeField] private CorpUiBase[] corp2Ui;

    protected override void AfterEnable() => Init();
    protected override void Execute(PartyAdventureStateChanged msg) => Init();
    
    private void Init() => Init(party.GetCorpAffinity(corps.GetMap()));
    private void Init(PartyCorpAffinity affinity)
    {
        var topAffinities = affinity.Where(a => ((int)a.Value) > 0)
            .OrderByDescending(a => (int)a.Value)
            .Take(2).ToArray();
        affinity.DevLogInfo();
        
        corp1Parent.SetActive(true);
        if (topAffinities.Length > 0)
        {
            var corp = topAffinities[0].Key;
            corp1AffinityLabel.text = topAffinities[0].Value.ToString();
            corp1Logo.sprite = corp.Logo;
            corp1Ui.ForEach(u => u.Init(corp));
        }
        else
        {
            var corp = corps.Unaffiliated;
            corp1AffinityLabel.text = corp.Name;
            corp1Logo.sprite = corp.Logo;
            corp1Ui.ForEach(u => u.Init(corp));
        }

        corp2Parent.SetActive(topAffinities.Length > 1);
        if (topAffinities.Length > 1)
        {
            var corp = topAffinities[1].Key;
            corp2AffinityLabel.text = topAffinities[1].Value.ToString();
            corp2Logo.sprite = corp.Logo;
            corp2Ui.ForEach(u => u.Init(corp));
        }
    }
}