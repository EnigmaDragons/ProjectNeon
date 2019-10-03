using UnityEngine;

public sealed class PartyBattleUIVisualizer : MonoBehaviour
{
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private HeroBattleUIPresenter heroPresenter;
    [SerializeField] private GameEvent onCompleted;
    [SerializeField] private float ySpacing;
    [SerializeField] private float xSpacing;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    void Start()
    {
        var party = partyArea.Party;
        var position = transform.position;
        for (var i = 0; i < 3; i++)
        {
            var h = Instantiate(heroPresenter,
                new Vector3(position.x + xOffset + xSpacing * i, position.y +yOffset + ySpacing * i, position.z), Quaternion.identity, gameObject.transform); 
            h.Set(party.Heroes[i]);
        }
        onCompleted.Publish();
    }
}
