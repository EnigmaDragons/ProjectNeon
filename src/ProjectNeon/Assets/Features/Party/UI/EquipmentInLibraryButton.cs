using UnityEngine;

public class EquipmentInLibraryButton : MonoBehaviour
{
    [SerializeField] private EquipmentPresenter presenter;
    [SerializeField] private DeckBuilderState deckBuilderState;
    [SerializeField] private PartyAdventureState partyAdventureState;

    public void Init(Equipment e, bool canAdd)
    {
        if (canAdd)
            presenter.Set(e, () => Equip(e));
        else
            presenter.Set(e, () => {});
    }

    private void Equip(Equipment e)
    {
        if (e.Slot == EquipmentSlot.Weapon && deckBuilderState.SelectedHeroesDeck.Hero.Equipment.Weapon.IsPresent)
            partyAdventureState.UnequipFrom(deckBuilderState.SelectedHeroesDeck.Hero.Equipment.Weapon.Value, deckBuilderState.SelectedHeroesDeck.Hero);
        if (e.Slot == EquipmentSlot.Armor && deckBuilderState.SelectedHeroesDeck.Hero.Equipment.Armor.IsPresent)
            partyAdventureState.UnequipFrom(deckBuilderState.SelectedHeroesDeck.Hero.Equipment.Armor.Value, deckBuilderState.SelectedHeroesDeck.Hero);
        partyAdventureState.EquipTo(e, deckBuilderState.SelectedHeroesDeck.Hero);
    }
}