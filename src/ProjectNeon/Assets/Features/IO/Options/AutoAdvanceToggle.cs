using UnityEngine;
using UnityEngine.UI;

public class AutoAdvanceToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        toggle.SetIsOnWithoutNotify(CurrentGameOptions.Data.UseAutoAdvance);
        toggle.onValueChanged.AddListener(CurrentGameOptions.SetAutoAdvance);
    }
}
