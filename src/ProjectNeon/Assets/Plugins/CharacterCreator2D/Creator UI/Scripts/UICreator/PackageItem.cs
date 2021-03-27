using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class PackageItem : MonoBehaviour
    {
        /// <summary>
        /// Initialize PackageItem withe desired package name.
        /// </summary>
        /// <param name="packageName"></param>
        public void Initialize(string packageName)
        {
            try
            {
                Text nametext = this.transform.Find("Text").GetComponent<Text>();
                nametext.text = packageName;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }
}