using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/StoryEventPool")]
public class StoryEventPool : ScriptableObject
{
    [SerializeField] private StoryEvent[] events;

    public StoryEvent[] AllEvents => events.ToArray();
}
