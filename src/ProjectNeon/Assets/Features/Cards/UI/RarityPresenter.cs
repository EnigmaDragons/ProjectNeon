using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RarityPresenter : MonoBehaviour
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
            {Rarity.Basic, starter},
            {Rarity.Common, common},
            {Rarity.Uncommon, uncommon},
            {Rarity.Rare, rare},
            {Rarity.Epic, epic}
        };
    }
        
    public void Set(Rarity r)
    {
        if (_sprites == null)
            Awake();
        
        image.sprite = _sprites[r];
    }
}
