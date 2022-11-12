using I2.Loc;
using UnityEngine;

public class CardEnemyTypePresenter : MonoBehaviour
{
    [SerializeField] private Localize label;
    private bool _hasInit;
    
    public void Hide() => gameObject.SetActive(false);

    public void Show()
    {
        if (_hasInit)
            gameObject.SetActive(true);
    }

    public void Init(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return;
        
        label.SetTerm($"BattleUI/{type}");
        _hasInit = true;
        Show();
    }
}