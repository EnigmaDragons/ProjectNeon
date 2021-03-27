using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
    public class UIToggler : MonoBehaviour
    {
        public Image icon;
        public bool active = true;
        public Sprite expandedIcon;
        public Sprite collapsedIcon;
        public GameObject[] objects;
        public bool saveLastActiveObjects;
        private Button button;

        private bool[] m_LastActiveObjects;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Toggle);
            m_LastActiveObjects = new bool[objects.Length];
        }

        public void Toggle()
        {
            active = !active;
            //foreach (GameObject go in objects) go.SetActive(active);
            ToggleObjects(active);
            if (icon == null)
            {
                return;
            }
            if (active)
            {
                icon.sprite = expandedIcon;
            }
            else
            {
                icon.sprite = collapsedIcon;
            }
        }

        private void ToggleObjects(bool active)
        {
            if (active)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    GameObject go = objects[i];
                    if (saveLastActiveObjects)
                    {
                        go.SetActive(m_LastActiveObjects[i]);
                    }
                    else
                    {
                        go.SetActive(true);
                    }
                }
            }
            else
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    GameObject go = objects[i];
                    m_LastActiveObjects[i] = go.activeSelf;
                    go.SetActive(false);
                }
            }
        }
    }
}
