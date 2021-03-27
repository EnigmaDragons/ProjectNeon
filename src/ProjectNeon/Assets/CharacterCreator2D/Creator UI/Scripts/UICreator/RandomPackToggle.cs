using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class RandomPackToggle : MonoBehaviour
    {
        [ReadOnly]
        public string packName;
        public Toggle toggle;

        private RandomPackGroup _group;

        public void Initialize(string packName)
        {
            this.packName = packName;
            transform.GetComponentInChildren<Text>(true).text = packName;
            if (_group == null)
                _group = this.GetComponentInParent<RandomPackGroup>();
        }

        public void Toggle(bool isOn)
        {
            _group.UpdateToggle(packName, isOn);
        }
    }
}