using System.Collections.Generic;
using UnityEngine;

public class PartyAdventureUiSummary : MonoBehaviour
{
    [SerializeField] private Party party;
    [SerializeField] private HeroHpPresenter heroPresenter;
    [SerializeField] private float ySpacing;
    [SerializeField] private float xSpacing;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    
    [ReadOnly, SerializeField] private List<HeroHpPresenter> active = new List<HeroHpPresenter>();
    
    private void Awake()
    {
        active.ForEach(Destroy);
        active.Clear();
        
        var position = transform.position;
        for (var i = 0; i < party.Heroes.Length; i++)
        {
            var h = Instantiate(heroPresenter,
                new Vector3(position.x + xOffset + xSpacing * i, position.y + yOffset + ySpacing * i, position.z), Quaternion.identity, gameObject.transform); 
            h.Init(party.Heroes[i], party.Hp[i]);
            active.Add(h);
        }
    }
}
