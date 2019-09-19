using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    #region Parametrs
    [SerializeField] Image barImage;
    [SerializeField] Text barTextValue;

    int maxHP = 100;

    public int currentHP { get; private set; }
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

    public delegate void Death();
    public event Death OnDeath;
    #endregion
    #region Unity methods
    private void Start()
    {
        currentHP = maxHP;
        UpdateUI();
    }
    #endregion
    #region Public methods
    public void UpdateMaxHP(int _maxHP)
    {
        maxHP += _maxHP;
        UpdateUI();
    }
    public void ChangeHP(int value)
    {
        currentHP += value;
        CorectHPValue();
        UpdateUI();
        CheckDeath();
    }
    #endregion
    #region Private methods
    void CorectHPValue()
    {
        if (currentHP < 0) currentHP = 0;
        if (currentHP > maxHP) currentHP = maxHP;
    }
    void UpdateUI()
    {
        ChangeImage();
        ChangeText();
    }
    void ChangeImage()
    {
        barImage.fillAmount = currentHP * 1f / maxHP * 1f;
    }
    void ChangeText()
    {
        barTextValue.text = $"{currentHP}/{maxHP}";
    }
    void CheckDeath()
    {
        if (currentHP != 0) return;
        if (OnDeath != null) OnDeath.Invoke();
    }
    #endregion
}
