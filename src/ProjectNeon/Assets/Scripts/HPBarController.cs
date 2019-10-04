using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField] Image barImage;
    [SerializeField] TextMeshProUGUI barTextValue;

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
        barImage.fillAmount = CurrentHP * 1f / maxHP * 1f;
    }
    void ChangeText()
    {
        barTextValue.text = $"{CurrentHP}/{maxHP}";
    }

    public void Init(int maxHp)
    {
        MaxHP = maxHp;
        CurrentHP = maxHp;
        UpdateUI();
    }
}
