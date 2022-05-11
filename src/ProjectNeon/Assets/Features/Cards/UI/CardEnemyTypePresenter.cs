using TMPro;
using UnityEngine;

public class CardEnemyTypePresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    private bool _hasInit;
    
    public void Hide() => gameObject.SetActive(false);

    public void Show()
    {
        if (_hasInit)
            gameObject.SetActive(true);
    }

    public void Init(string type)
    {
        label.text = type;
        _hasInit = true;
        Show();
    }
}