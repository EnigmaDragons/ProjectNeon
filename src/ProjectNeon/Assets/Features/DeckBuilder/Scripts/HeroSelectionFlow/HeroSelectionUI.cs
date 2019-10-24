using System.Collections.Generic;
using UnityEngine;

public class HeroSelectionUI : MonoBehaviour
{
    private const float Padding = 10;

    [SerializeField] private Party party;
    [SerializeField] private SelectHeroButton selectHeroButtonTemplate;
    [SerializeField] private Transform parent;
    [SerializeField] private DeckBuilderNavigation navigation;

    private void Start()
    {
        var buttons = new List<RectTransform>();
        party.Heroes.ForEach(x =>
        {
            var button = Instantiate(selectHeroButtonTemplate, parent);
            button.GetComponent<SelectHeroButton>().Init(x, navigation);
            buttons.Add(button.GetComponent<RectTransform>());
        });
        for (var i = 0; i < buttons.Count; i++)
            buttons[i].anchoredPosition = new Vector2((i - (buttons.Count / 2f - 0.5f)) * (buttons[i].sizeDelta.x + Padding), buttons[i].anchoredPosition.y);
    }
}
