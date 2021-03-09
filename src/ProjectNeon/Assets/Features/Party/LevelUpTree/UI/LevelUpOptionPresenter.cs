using TMPro;
using UnityEngine;

public sealed class LevelUpOptionPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    
    public LevelUpOptionPresenter Initialized(HeroLevelUpOption o)
    {
        text.text = o.Description;
        return this;
    }
}
