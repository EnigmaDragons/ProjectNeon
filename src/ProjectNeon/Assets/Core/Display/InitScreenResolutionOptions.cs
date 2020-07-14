using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
     
public sealed class InitScreenResolutionOptions : MonoBehaviour
{
    [SerializeField] private DisplaySettings display;
    [SerializeField] private TMP_Dropdown dropdownMenu;
    [SerializeField] private int minScreenWidth = 800;
    [SerializeField] private Vector2Int screenRatio = new Vector2Int(16, 9);

    private Resolution[] _resolutions;

    private void Awake()
    {
        var comparer = new ResolutionEqualityComparer();
        display.InitWithoutChanging();
        _resolutions = Screen.resolutions
            .Where(x => x.width % screenRatio.x == 0 && x.height % screenRatio.y == 0)
            .Where(x => x.width > minScreenWidth)
            .Distinct(comparer)
            .Reverse()
            .ToArray();
        dropdownMenu.onValueChanged.AddListener(SetResolution);
        dropdownMenu.options.Clear();
        var current = display.CurrentScreenSize;
        for (var i = 0; i < _resolutions.Length; i++)
        {
            var valString = ResToString(_resolutions[i]);
            dropdownMenu.options.Add(new TMP_Dropdown.OptionData(valString));
            if (comparer.Equals(_resolutions[i], current))
            {
                dropdownMenu.value = i;
                Debug.Log($"Matching Resolution Option is {i}");
            }
        }

        dropdownMenu.RefreshShownValue();
    }
    
    private string ResToString(Resolution res) => res.width + " x " + res.height;
    private void SetResolution(int index) => display.SetResolution(_resolutions[index]);

    private class ResolutionEqualityComparer : IEqualityComparer<Resolution>
    {
        public bool Equals(Resolution x, Resolution y) => x.height == y.height && x.width == y.width;
        public bool Equals(Resolution x, Vector2Int y) => x.height == y.x && x.width == y.y;

        public int GetHashCode(Resolution obj) => int.Parse($"{obj.height}{obj.width}");
    }
}
