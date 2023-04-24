using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/ResourceSpriteMap")]
public class ResourceSpriteMap : ScriptableObject
{
    [SerializeField] private List<string> resourceNames;
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private Sprite defaultIcon;

    public Sprite Get(string resourceName)
    {
        var index = resourceNames.IndexOf(resourceName);
        return index == -1 ? defaultIcon : icons[index];
    }
}