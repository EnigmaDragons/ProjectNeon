using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
    public class BodyTypeGroup : MonoBehaviour
    {
        /// <summary>
        /// PackageItem's template.
        /// </summary>
        [Tooltip("PackageItem's template")]
        public PackageItem tPackage;

        /// <summary>
        /// BodyTypeItem's template.
        /// </summary>
        [Tooltip("BodyTypeItem's template")]
        public BodyTypeItem tBodyTypeItem;

        /// <summary>
        /// Current selected BodyTypeItem.
        /// </summary>
        [Tooltip("Current selected BodyTypeItem")]
        [ReadOnly]
        public BodyTypeItem selectedItem;

        /// <summary>
        /// Selection Transform used for highlighting current selected BodyTypeItem.
        /// </summary>
        [Tooltip("Selection Transform used for highlighting current selected BodyTypeItem")]
        public Transform selectionObj;

        /// <summary>
        /// UICreator found in this BodyTypeGroup's parent Transform.
        /// </summary>
        public UICreator CreatorUI { get; private set; }

        private List<Color> _bodycolors = new List<Color>();
        
        /// <summary>
        /// Initialize BodyTypeGroup and populate the items.
        /// </summary>
        public void Initialize()
        {
            this.CreatorUI = this.transform.GetComponentInParent<UICreator>();
            if (this.CreatorUI == null)
                return;

            Transform itemparent = getItemParent();
            Dictionary<string, List<Part>> bodyskins = getAvailableBodySkins();

            clearItems();
            selectionObj.gameObject.SetActive(false);

            //..add null
            spawnItem(BodyType.Female, null, itemparent);
            spawnItem(BodyType.Male, null, itemparent);
            //add null..

            foreach (string packagename in bodyskins.Keys)
            {
                PackageItem packitem = Instantiate<PackageItem>(tPackage, itemparent);
                packitem.Initialize(packagename);
                foreach (Part bodyskin in bodyskins[packagename])
                {
                    foreach (BodyType bodytype in bodyskin.supportedBody)
                        spawnItem(bodytype, bodyskin, itemparent);
                }
            }

            _bodycolors = getBodyColors();
        }

        private void spawnItem(BodyType bodyType, Part bodySkin, Transform itemParent)
        {
            Part assignedpart = CreatorUI.character.GetAssignedPart(SlotCategory.BodySkin);
            if (bodySkin == null) //spawn NULL
            {
                BodyTypeItem partitem = Instantiate<BodyTypeItem>(tBodyTypeItem, itemParent);
                partitem.Initialize(bodyType, null);
                if (CreatorUI.character.bodyType == bodyType && assignedpart == null)
                {
                    selectedItem = partitem;
                    if (this.gameObject.activeInHierarchy)
                    {
                        StopCoroutine("ie_initselected");
                        StartCoroutine("ie_initselected", selectedItem);
                    }
                }
            }
            else
            {
                BodyTypeItem partitem = Instantiate<BodyTypeItem>(tBodyTypeItem, itemParent);
                partitem.Initialize(bodyType, bodySkin);
                if (CreatorUI.character.bodyType == bodyType && assignedpart == bodySkin)
                {
                    selectedItem = partitem;
                    if (this.gameObject.activeInHierarchy)
                    {
                        StopCoroutine("ie_initselected");
                        StartCoroutine("ie_initselected", selectedItem);
                    }
                }
            }
        }

        private Transform getItemParent()
        {
            Transform val = this.transform.Find("Part List/Content");
            return val != null ? val : this.transform;
        }

        void OnEnable()
        {
            Initialize();
            UIBodyColor uibodycolor = CreatorUI.GetComponentInChildren<UIBodyColor>(true);
            if (uibodycolor != null)
                uibodycolor.gameObject.SetActive(true);
        }
        
        IEnumerator ie_initselected(BodyTypeItem item)
        {
            yield return null;
            SelectItem(item);
        }

        private void clearItems()
        {
            PackageItem[] packages = this.transform.GetComponentsInChildren<PackageItem>(true);
            for (int i = packages.Length - 1; i >= 0; i--)
            {
                GameObject go = packages[i].gameObject;
                packages[i] = null;
                Destroy(go);
            }

            BodyTypeItem[] items = this.transform.GetComponentsInChildren<BodyTypeItem>(true);
            for (int i = items.Length - 1; i >= 0; i--)
            {
                GameObject go = items[i].gameObject;
                items[i] = null;
                Destroy(go);
            }
        }

        private Dictionary<string, List<BodyType>> getAvailableBodyTypes()
        {
            try
            {
                Dictionary<string, List<BodyType>> val = new Dictionary<string, List<BodyType>>();
                foreach (BodyTypeData d in CreatorUI.character.setupData.bodyTypeData)
                {
                    if (!val.ContainsKey(d.packageName))
                        val.Add(d.packageName, new List<BodyType>());

                    val[d.packageName].Add(d.bodyType);
                }

                return val;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return new Dictionary<string, List<BodyType>>();
            }
        }

        private Dictionary<string, List<Part>> getAvailableBodySkins()
        {
            try
            {
                Dictionary<string, List<Part>> val = new Dictionary<string, List<Part>>();
                List<Part> parts = PartList.Static.FindParts(SlotCategory.BodySkin);
                foreach (Part part in parts)
                {
                    //if (!part.supportedBody.Contains(CreatorUI.character.bodyType))
                    //    continue;

                    if (!val.ContainsKey(part.packageName))
                        val.Add(part.packageName, new List<Part>());

                    val[part.packageName].Add(part);
                }

                return val;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                return new Dictionary<string, List<Part>>();
            }
        }

        /// <summary>
        /// Updates selectedItem to a given value and updates UICreator's character body type.
        /// </summary>
        /// <param name="item">BodyTypeItem to be selected.</param>
        public void SelectItem(BodyTypeItem item)
        {
            selectedItem = item;
            selectionObj.gameObject.SetActive(true);
            selectionObj.position = item.transform.position;

            if (CreatorUI.character.bodyType != item.bodyType)
                CreatorUI.character.SetBodyType(item.bodyType);

            Part skin = CreatorUI.character.GetAssignedPart(SlotCategory.BodySkin);
            if (skin != item.bodySkin)
                CreatorUI.character.EquipPart(SlotCategory.BodySkin, item.bodySkin);
        }

        /// <summary>
        /// Assign selectedItem with random BodyTypeItem found in this Transform's children.
        /// </summary>
        public void RandomizeBodyType()
        {
            if (CreatorUI == null)
                return;
            
            BodyTypeItem[] items = this.transform.GetComponentsInChildren<BodyTypeItem>(true);
            if (items.Length > 0)
                SelectItem(items[Random.Range(0, items.Length)]);

            if (_bodycolors.Count > 0)
            {
                Color selectedcolor = _bodycolors[Random.Range(0, _bodycolors.Count)];
                CreatorUI.character.SkinColor = selectedcolor;
                UIBodyColor uibodycolor = CreatorUI.GetComponentInChildren<UIBodyColor>(true);
                if (uibodycolor != null)
                    uibodycolor.colorImg.color = selectedcolor;
            }
        }

        private List<Color> getBodyColors()
        {
            List<Color> val = new List<Color>();
            foreach (ColorPalette p in CreatorUI.colorUI.colorPalette.colorPalettes)
            {
                if (p.paletteName.Contains("Skin"))
                {
                    foreach (Color c in p.colors)
                        val.Add(c);
                }
            }

            return val;
        }
    }
}