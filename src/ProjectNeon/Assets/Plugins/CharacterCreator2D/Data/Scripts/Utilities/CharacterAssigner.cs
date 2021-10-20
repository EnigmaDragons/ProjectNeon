namespace CharacterCreator2D.Utilities
{
    public class CharacterAssigner
    {
        public virtual void AssignMaterialToRenderer(CharacterViewer character, SlotCategory slotCategory) { }

        public virtual void AssignPartToRenderer(CharacterViewer character, SlotCategory slotCategory) { }

        public virtual void AssignColorToRenderer(CharacterViewer character, SlotCategory slotCategory) { }

        public virtual void AssignTintColorToRenderer(CharacterViewer character) { }

        public virtual void AssignSkinColorToRenderer(CharacterViewer character) { }
    }
}
