using UnityEngine;

public sealed class PartyBattleUIVisualizer : MonoBehaviour
{
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private HeroBattleUIPresenter heroPresenter;
    [SerializeField] private GameEvent onCompleted;
    [SerializeField] private float ySpacing;

    void Start()
    {
        var party = partyArea.Party;
        var position = transform.position;
        for (var i = 0; i < 3; i++)
        {
            var h = Instantiate(heroPresenter,
                new Vector3(position.x, position.y + ySpacing * i, position.z), Quaternion.identity, gameObject.transform); 
            h.Set(party.Heroes[i]);
        }
        onCompleted.Publish();
    }
}
