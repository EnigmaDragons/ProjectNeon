using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
    public class EmotePartGroup : MonoBehaviour
    {
        public EmotePartItem emotePartItem;
		public PackageItem packageItem;
		public EmotionType emotionType;
		public EmoteIndex emoteIndex;

        [ReadOnly]
        public EmotePartItem selectedItem;
        
        public SlotCategory slotCategory;
        public Transform selectionObj;
        public UIEmote emoteUI;
        public void Initialize()
        {
            this.emoteUI = this.transform.GetComponentInParent<UIEmote>();
            if (this.emoteUI == null)
                return;
			emotionType = emoteUI.activeEmotion;
			emoteIndex = emoteUI.character.emotes.getIndex(emotionType);

            Transform itemparent = this.transform.Find("List/Content");
            Dictionary<string, List<Part>> parts = getAvailableParts();
            Part assignedpart = emoteIndex.getPart(slotCategory);

            clearItems();
            selectionObj.gameObject.SetActive(false);

            //..add null
            EmotePartItem partitem = Instantiate<EmotePartItem>(emotePartItem, itemparent);
            partitem.Initialize(null);
            if (assignedpart == null)
            {
                selectedItem = partitem;
                if (this.gameObject.activeInHierarchy)
                {
                    StopCoroutine("ie_initselected");
                    StartCoroutine("ie_initselected", selectedItem);
                }
            }
            //add null..

            foreach (string packagename in parts.Keys)
            {
                PackageItem packitem = Instantiate<PackageItem>(packageItem, itemparent);
                packitem.Initialize(packagename);

                foreach (Part part in parts[packagename])
                {
                    partitem = Instantiate<EmotePartItem>(emotePartItem, itemparent);
                    partitem.Initialize(part);
                    if (part == assignedpart)
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
        }

        void OnEnable()
        {
            Initialize();
        }

        IEnumerator ie_initselected(EmotePartItem item)
        {
            yield return null;
            SelectItem(item);
        }

        private Dictionary<string, List<Part>> getAvailableParts()
        {
            try
            {
                Dictionary<string, List<Part>> val = new Dictionary<string, List<Part>>();
                List<Part> parts = PartList.Static.FindParts(slotCategory);
                foreach (Part part in parts)
                {
                    if (!part.supportedBody.Contains(emoteUI.character.bodyType))
                        continue;

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

        public void SelectItem(EmotePartItem item)
        {
            selectedItem = item;
            selectionObj.gameObject.SetActive(true);
            selectionObj.position = item.transform.position;
        }

        private void clearItems()
        {
			Transform itemParent = this.transform.Find("List/Content");
			foreach (Transform t in itemParent) 
			{
				if (t == selectionObj) continue;
				Destroy(t.gameObject);
			}
        }
    }
}