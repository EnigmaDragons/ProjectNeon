using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIHealthAlchemy
{
    [ExecuteInEditMode]
    public class ElemsHealthBar : HealthBarLogic
    {
        public Transform parent;
        public bool isInvert = false;

        protected float _Value;
        [SerializeField]
        public override float Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    this.value = value;
                    /*int index = 0;
                    if (value > 0.5f)
                        index = Mathf.FloorToInt(value * parent.childCount);
                    else
                        index = Mathf.CeilToInt(value * parent.childCount);*/
                    int index = Mathf.CeilToInt(value * (parent.childCount - 1));
                    if (value == 1)
                        index = parent.childCount;
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        if(isInvert)
                            parent.GetChild(i).gameObject.SetActive((parent.childCount - i - 1) < index);
                        else
                            parent.GetChild(i).gameObject.SetActive(i < index);
                    }
                }
            }
        }
        [Range(0.0f, 1.0f)]
        [SerializeField]
        protected float value;

        private void Update()
        {
            Value = value;
        }
    }
}
