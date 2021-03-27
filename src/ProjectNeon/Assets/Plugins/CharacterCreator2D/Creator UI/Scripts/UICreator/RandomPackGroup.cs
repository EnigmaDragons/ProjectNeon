using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class RandomPackGroup : MonoBehaviour
    {
        public RandomPackToggle TToggle;
        public Transform contentParent;
        [ReadOnly]
        public List<string> activePacks;

        private const string _ALLPACKNAME = "All Packs";
        private List<RandomPackToggle> _toggles;

        private bool _notallpack = false;

        private void Start()
        {
            initialize();
        }

        private void initialize()
        {
            clearToggles();
            activePacks = new List<string>();
            List<string> togglenames = getToggleNames();
            foreach (string tname in togglenames)
                addToggle(tname);
            UpdateToggle(_ALLPACKNAME, true);
        }

        private void clearToggles()
        {
            if (_toggles != null && _toggles.Count > 0)
            {
                for (int i = _toggles.Count - 1; i >= 0; i--)
                    Destroy(_toggles[i].gameObject);
            }
            _toggles = new List<RandomPackToggle>();
        }

        private void addToggle(string toggleName)
        {
            RandomPackToggle ttoggle = Instantiate<RandomPackToggle>(TToggle, contentParent);
            ttoggle.Initialize(toggleName);
            _toggles.Add(ttoggle);
        }

        private List<string> getToggleNames()
        {
            List<string> val = new List<string>();
            val.Add(_ALLPACKNAME);
            foreach (PartPack pack in PartList.Static.partPacks)
            {
                foreach (Part p in pack.parts)
                {
                    if (!val.Contains(p.packageName))
                        val.Add(p.packageName);
                }
            }
            return val;
        }

        public void UpdateToggle(string toggleName, bool isOn)
        {
            if (isOn)
                addActivePack(toggleName);
            else
                removeActivePack(toggleName);
        }

        private void addActivePack(string packName)
        {
            if (packName == _ALLPACKNAME)
            {
                foreach (RandomPackToggle toggle in _toggles)
                {
                    if (toggle.packName != _ALLPACKNAME)
                    {
                        toggle.toggle.isOn = true;
                        toggle.Toggle(true);
                    }
                }
                return;
            }

            if (!activePacks.Contains(packName))
                activePacks.Add(packName);
            if (activePacks.Count >= _toggles.Count - 1)
                _toggles[0].toggle.isOn = true;
        }

        private void removeActivePack(string packName)
        {
            if (packName == _ALLPACKNAME && !_notallpack)
            {
                foreach (RandomPackToggle toggle in _toggles)
                {
                    if (toggle.packName != _ALLPACKNAME)
                    {
                        toggle.toggle.isOn = false;
                        toggle.Toggle(false);
                    }
                }
                return;
            }

            if (activePacks.Contains(packName))
                activePacks.Remove(packName);
                
            _notallpack = true;
            _toggles[0].toggle.isOn = false;
            _notallpack = false;
        }

        public List<string> GetExcludedPacks()
        {
            List<string> val = new List<string>();
            foreach (RandomPackToggle toggle in _toggles)
            {
                if (toggle.packName != _ALLPACKNAME && !activePacks.Contains(toggle.packName))
                    val.Add(toggle.packName);
            }
            return val;
        }
    }
}