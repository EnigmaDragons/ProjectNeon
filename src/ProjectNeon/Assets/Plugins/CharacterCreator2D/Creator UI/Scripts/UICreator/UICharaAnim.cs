using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UICharaAnim : MonoBehaviour
    {
        public Dropdown dropDown;
        public int animationLayer;
        public AnimationList[] animationList;

        private int _sanim;
        private Animator animator;

        private List<string> states = new List<string>();

        void Start()
        {            
            dropDown.options.Clear();
            foreach (AnimationList al in animationList)
            {
                for (int i = 0; i < al.stateName.Count; i++)
                {
                    string s = al.stateName[i];
                    states.Add(s);
                    dropDown.options.Add(new Dropdown.OptionData(s));
                }
            }
            dropDown.RefreshShownValue();
            animator = GameObject.FindObjectOfType<CharacterViewer>().GetComponent<Animator>();
            _sanim = 0;
            SelectAnimation(_sanim);
        }

        private void OnEnable() 
        {
            SelectAnimation(_sanim);
        }

        public void SelectAnimation(int index)
        {
            if (index < 0 || index >= states.Count || animator == null)
                return;
            _sanim = index;            
            animator.Play(states[index], animationLayer);
        }
    }
}