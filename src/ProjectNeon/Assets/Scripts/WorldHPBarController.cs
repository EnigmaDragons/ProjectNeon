using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldHPBarController : MonoBehaviour
{
    [SerializeField] private GameObject bar;
    [SerializeField] private TextMeshPro text;

    int maxHP = 100;

    public int CurrentHP { get; private set; }
    public int MaxHP
    {
        set
        {
            maxHP = value;
            UpdateUI();
        }
        get
        {
            return maxHP;
        }
    }
    private void Start()
    {
        CurrentHP = maxHP;
        UpdateUI();
    }
    public void ChangeMaxHP(int _maxHP)
    {
        maxHP += _maxHP;
        UpdateUI();
    }
    public void ChangeHP(int value)
    {
        CurrentHP += value;
        CorectHPValue();
        UpdateUI();
    }
    void CorectHPValue()
    {
        Mathf.Clamp(CurrentHP, 0, maxHP);
    }
    void UpdateUI()
    {
        ChangeImage();
        ChangeText();
    }
    void ChangeImage()
    {
        var originalScale = bar.transform.localScale;
        bar.transform.localScale = new Vector3(MaxHP > 0 ? CurrentHP / maxHP : 0, originalScale.y, originalScale.z);
    }
    void ChangeText()
    {
        text.text = $"{CurrentHP}/{maxHP}";
    }

    public void Init(int maxHp)
    {
        MaxHP = maxHp;
        CurrentHP = maxHp;
        UpdateUI();
    }
}
