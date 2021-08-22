using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/StoryEventPool2")]
public class StoryEventPool2 : ScriptableObject
{
    [SerializeField] private StoryEvent2[] events;

    public StoryEvent2[] AllEvents => events.ToArray();
}