using UnityEngine;
using UnityEngine.UI;

public class CardSpeedSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void OnEnable()
    {
        slider.value = PlayerPrefs.GetFloat("CardSpeed", 1f);
        slider.onValueChanged.AddListener(SetLevel);
    }

    public void SetLevel(float sliderValue)
    {
        PlayerPrefs.SetFloat("CardSpeed", sliderValue);
        Message.Publish(new CardSpeedChanged(sliderValue));
    }
}