using TMPro;
using UnityEngine;

public class EquipmentInLibraryButton : MonoBehaviour
{
    [SerializeField] private EquipmentPresenter presenter;
    [SerializeField] private DeckBuilderState deckBuilderState;
    [SerializeField] private PartyAdventureState partyAdventureState;
    [SerializeField] private GameObject darken;
    [SerializeField] private TextMeshProUGUI countLabel;
    
    public void InitInfoOnly(Equipment e) => presenter.Set(e, () => { });

    public void Init(Equipment e, int totalCount, int availableCount, bool canAdd)
    {
        var countLabelText = totalCount > 1 ? $"{availableCount}/{totalCount}" : "";
        if (canAdd)
        {
            presenter.Set(e, () => Equip(e));
            countLabel.text = countLabelText;
            darken.SetActive(false);
        }
        else
        {
            presenter.Set(e, () => {});
            countLabel.text = countLabelText;
            darken.SetActive(true);
        }
    }

    private void Equip(Equipment e)
    {
        if (e.Slot == EquipmentSlot.Weapon && deckBuilderState.SelectedHeroesDeck.Hero.Equipment.Weapon.IsPresent)
            partyAdventureState.UnequipFrom(deckBuilderState.SelectedHeroesDeck.Hero.Equipment.Weapon.Value, deckBuilderState.SelectedHeroesDeck.Hero);
        if (e.Slot == EquipmentSlot.Armor && deckBuilderState.SelectedHeroesDeck.Hero.Equipment.Armor.IsPresent)
            partyAdventureState.UnequipFrom(deckBuilderState.SelectedHeroesDeck.Hero.Equipment.Armor.Value, deckBuilderState.SelectedHeroesDeck.Hero);
        partyAdventureState.EquipTo(e, deckBuilderState.SelectedHeroesDeck.Hero);
        Message.Publish(new EquipmentPickerCurrentGearChanged());
    }
}