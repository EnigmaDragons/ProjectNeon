using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPresenter : MonoBehaviour
{
    private readonly Dictionary<Difficulty, string> _difficultyNames = new Dictionary<Difficulty, string>
    {
        {Difficulty.Casual, "Casual"},
        {Difficulty.Experienced, "Experienced"}
    };
    private readonly Dictionary<Difficulty, string> _difficultyChanges = new Dictionary<Difficulty, string>()
    {
        {Difficulty.Casual, "-More Hero Health\n-Retry after losses with full health"},
        {Difficulty.Experienced, "-No Modifications"},
    };
    private readonly Dictionary<Difficulty, string> _difficultyDescription = new Dictionary<Difficulty, string>
    {
        {Difficulty.Casual, "Recommended for players that have not played a ton of card games like this, or if you want to experience the story with out having to worry about a reset"},
        {Difficulty.Experienced, "If you have played a lot of card games like this one such as Slay the Spire, Monster Train, or Griftlands, this is the perfect difficulty for you and will put your skills to the test"}
    };
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI changes;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button selectButton;

    public void Init(Difficulty difficulty, Action onSelect)
    {
        nameText.text = _difficultyNames[difficulty];
        changes.text = _difficultyChanges[difficulty];
        description.text = _difficultyDescription[difficulty];
        
        selectButton.onClick.AddListener(() => onSelect());
    }
}