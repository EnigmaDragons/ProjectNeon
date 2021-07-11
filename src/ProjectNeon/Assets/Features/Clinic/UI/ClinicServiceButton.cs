using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicServiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Image credIcon;
    [SerializeField] private Button button;

    public void Init(ClinicServiceButtonData data, PartyAdventureState party)
    {
        title.text = data.Name;
        description.text = data.Description;
        cost.text = data.Cost.ToString();
        button.interactable = data.Enabled;
        if (party.Credits >= data.Cost)
            button.onClick.AddListener(() =>
            {
                party.UpdateCreditsBy(-data.Cost);
                data.Action();
                Message.Publish(new UpdateClinic());
            });
        if (data.Cost == 0)
        {
            cost.gameObject.SetActive(false);
            credIcon.gameObject.SetActive(false);
        }
    }
}