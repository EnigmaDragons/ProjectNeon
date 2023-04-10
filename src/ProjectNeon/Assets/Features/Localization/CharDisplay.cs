#if UNITY_EDITOR
using System.Text;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class CharDisplay : MonoBehaviour
{
    [SerializeField] private Localize characterContainer;
    [SerializeField] private Button button;
    
    private char _character;

    public bool Selected { get; set; }
    
    private void Awake()
    {
        button.onClick.AddListener(Toggle);
    }
    
    public void Init(char character)
    {
        _character = character;
    }

    private void Toggle() => Selected = !Selected;

    public string ToUnicode()
    {
        return "";
    }
}
#endif