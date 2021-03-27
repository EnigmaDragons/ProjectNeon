using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CharacterCreator2D.UI
{
    public class PartItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// Part loaded by this PartItem.
        /// </summary>
        [Tooltip("Part loaded by this PartItem")]
        [ReadOnly]
        public Part part;

        private PartGroup _group;
        static Part _offhand;
        static Part _mainhand;

        void Awake()
        {
            _group = this.transform.GetComponentInParent<PartGroup>();
        }

        void OnEnable()
        {
            if (_group.slotCategory == SlotCategory.MainHand || _group.slotCategory == SlotCategory.OffHand) 
            {
                _mainhand = _group.CreatorUI.character.slots.GetSlot(SlotCategory.MainHand).assignedPart;
                _offhand = _group.CreatorUI.character.slots.GetSlot(SlotCategory.OffHand).assignedPart;
            }
        }

        /// <summary>
        /// Initialize PartItem according to a given Part.
        /// </summary>
        /// <param name="part">Part to be loaded.</param>
        public void Initialize(Part part)
        {
            this.part = part;
            this.transform.name = part == null ? "NULL" : part.packageName + "_" + part.name;
            this.transform.Find("Text").GetComponent<Text>().text = part == null ? "NULL" : part.name;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_group.selectedItem != null && _group.selectedItem == this)
                return;

            _group.CreatorUI.character.EquipPart(_group.slotCategory, part);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_group.selectedItem != null && _group.selectedItem == this)
                return;

            if (_group.slotCategory == SlotCategory.MainHand || _group.slotCategory == SlotCategory.OffHand)
            {
                _group.CreatorUI.character.EquipPart(SlotCategory.MainHand, _mainhand);
                _group.CreatorUI.character.EquipPart(SlotCategory.OffHand, _offhand);
            }
            else
                _group.CreatorUI.character.EquipPart(_group.slotCategory, _group.selectedItem.part);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_group.selectedItem != null && _group.selectedItem == this)
                return;

            _group.SelectItem(this);

            if (_group.slotCategory == SlotCategory.MainHand || _group.slotCategory == SlotCategory.OffHand) 
            {
                _mainhand = _group.CreatorUI.character.slots.GetSlot(SlotCategory.MainHand).assignedPart;
                _offhand = _group.CreatorUI.character.slots.GetSlot(SlotCategory.OffHand).assignedPart;
            }
        }
    }
}