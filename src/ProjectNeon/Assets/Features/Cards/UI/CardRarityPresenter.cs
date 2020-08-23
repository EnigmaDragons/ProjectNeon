
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRarityPresenter : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite starter;
    [SerializeField] private Sprite common;
    [SerializeField] private Sprite uncommon;
    [SerializeField] private Sprite rare;
    [SerializeField] private Sprite epic;

    private Dictionary<Rarity, Sprite> _sprites;

    private void Awake()
    {
        _sprites = new Dictionary<Rarity, Sprite>
        {
            {Rarity.Starter, starter},
            {Rarity.Common, common},
            {Rarity.Uncommon, uncommon},
            {Rarity.Rare, rare},
            {Rarity.Epic, epic}
        };
    }
        
    public void Set(Rarity r)
    {
        image.sprite = _sprites[r];
    }
}
