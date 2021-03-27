using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
	public class MenuButton : MonoBehaviour {

		public string openMenu;
		public bool isPartMenu;
		public SlotCategory slotCategory;
		public WeaponCategory weaponCategory;

		Button mainbutton;
		UICreator uicreator;

		void Start () 
		{
			uicreator = GetComponentInParent<UICreator>();
			mainbutton = GetComponent<Button>();
			mainbutton.onClick.AddListener(OpenMenu);
            
			Button[] buttons = GetComponentsInChildren<Button>();
			foreach (Button b in buttons)
			{
				if (b.name == "Randomize")
					b.onClick.AddListener(Randomize);
				if (b.name == "Clear")
					b.onClick.AddListener(Clear);
			}
		}

        void OpenMenu()
        {
            if (!isPartMenu)
                uicreator.OpenMenu(openMenu);
            else
                uicreator.OpenPartMenu(openMenu, slotCategory);
        }

		void Randomize ()
		{
			if (openMenu == "Body Slider")
			{
				uicreator.RandomizeBodySliders();
				return;
			}

            if (openMenu == "Body Type")
            {
                uicreator.RandomizeBody();
                uicreator.RandomizePart(SlotCategory.BodySkin);
                uicreator.RandomizeSkinColor();
                return;
            }

			if (!isPartMenu)
				return;
			if (slotCategory == SlotCategory.MainHand || slotCategory == SlotCategory.OffHand)
			{
				uicreator.RandomizeWeapon(slotCategory, weaponCategory);
				uicreator.RandomizeColor(slotCategory);
			}
			else
			{
				uicreator.RandomizePart(slotCategory);
				uicreator.RandomizeColor(slotCategory);
			}
		}
		
		void Clear ()
		{
			if (!isPartMenu)
				return;
			uicreator.character.EquipPart(slotCategory, "");
			uicreator.character.SetPartColor(slotCategory, Color.gray, Color.gray, Color.gray);
		}
	}
}
