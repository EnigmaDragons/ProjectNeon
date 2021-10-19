using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UICharaAnimExportPNG : MonoBehaviour
    {
        public Dropdown dropDown;
        public int animationLayer;
        public AnimationList[] animationList;

        private int _sanim;
        private Animator animator;
        private RuntimeAnimatorController defaultAnimator;
        public RuntimeAnimatorController atlasAnimator;

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
            defaultAnimator = animator.runtimeAnimatorController;
            _sanim = 0;
            SelectAnimation(_sanim);
        }

        private void OnEnable() 
        {
            SelectAnimation(_sanim);
        }

        private void OnDisable()
        {
            animator.runtimeAnimatorController = defaultAnimator;
        }

        public void SelectAnimation(int index)
        {
            if (index < 0 || index >= states.Count || animator == null)
                return;
            _sanim = index;            

            if (states[index] == "Atlas")
            {
                animator.runtimeAnimatorController = atlasAnimator;
                string atlasName = "Atlas ";
                if(animator.GetComponent<CharacterViewer>().bodyType == BodyType.Male)
                    atlasName += "Male";
                else
                    atlasName += "Female";
                animator.Play(atlasName);
            }
            else if (animator.runtimeAnimatorController == atlasAnimator)
            {
                animator.runtimeAnimatorController = defaultAnimator;
                animator.Play(states[index], animationLayer);
            }
            else
            {
                animator.Play(states[index], animationLayer);
            }
        }
    }
}