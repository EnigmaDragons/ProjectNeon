using UnityEngine;

namespace CharacterCreator2D.Utilities
{
    public class CharacterSetter
    {
        public virtual void SetMaterialToSlot(CharacterViewer character, SlotCategory slotCategory, Material material) { }

        public virtual void SetPartToSlot(CharacterViewer character, SlotCategory slotCategory, Part part) { }

        public virtual void SetColorToSlot(CharacterViewer character, SlotCategory slotCategory, string colorCode, Color color) { }
    }
}
