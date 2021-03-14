using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXV
{
    public class FXVDemoMobile : MonoBehaviour
    {
        public GameObject[] shieldRoots;

        public Material[] materials;
        public Material[] hitMaterials;
        public Color[] hitColors;

        public Text numShieldsLabel;

        public Text materialLabel;

        public Text fpsLabel;

        private int currentNumShields = 1;

        private int currentMaterial = 0;

        void Start()
        {
            UpdateShieldsCount();
            UpdateShieldsMaterials();
        }

        void Update()
        {
            fpsLabel.text = "FPS: " + ((int)(1.0f / Time.smoothDeltaTime)).ToString();

        }

        public void TurnOnOff()
        {
            FXVShield[] shields = GameObject.FindObjectsOfType<FXVShield>();
            foreach (FXVShield s in shields)
            {
                if (s.GetIsShieldActive())
                    s.SetShieldActive(false);
                else
                    s.SetShieldActive(true);

            }
        }

        void UpdateShieldsMaterials()
        {
            for (int i = 0; i < shieldRoots.Length; ++i)
            {
                FXVShield[] shields = shieldRoots[i].GetComponentsInChildren<FXVShield>();
                foreach (FXVShield s in shields)
                {
                    s.SetMaterial(materials[currentMaterial]);
                    s.SetHitMaterial(hitMaterials[currentMaterial]);
                    s.hitColor = hitColors[currentMaterial];
                }
            }

            materialLabel.text = materials[currentMaterial].name;
        }

        public void NextMaterial()
        {
            if (currentMaterial < materials.Length - 1)
                currentMaterial++;

            UpdateShieldsMaterials();
        }

        public void PrevMaterial()
        {
            if (currentMaterial > 0)
                currentMaterial--;

            UpdateShieldsMaterials();
        }

        void UpdateShieldsCount()
        {
            int countShields = 0;
            for (int i = 0; i < shieldRoots.Length; ++i)
            {
                if (i < currentNumShields)
                {
                    shieldRoots[i].SetActive(true);
                    countShields += shieldRoots[i].transform.childCount;
                }
                else
                    shieldRoots[i].SetActive(false);
            }

            numShieldsLabel.text = countShields.ToString();
        }

        public void MoreShields()
        {
            if (currentNumShields < shieldRoots.Length)
                currentNumShields++;

            UpdateShieldsCount();
        }

        public void LessShields()
        {
            if (currentNumShields > 1)
                currentNumShields--;

            UpdateShieldsCount();
        }
    }

}