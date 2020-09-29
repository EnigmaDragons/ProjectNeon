using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Adventure/Adventure")]
public class Adventure : ScriptableObject
{
    [SerializeField] private Stage[] stages;
    [SerializeField] private string adventureTitle;
    [SerializeField, TextArea(4, 10)] private string story;

    // @todo #1:15min Design. What happens when the adventure is won?

    public string Title => adventureTitle;
    public string Story => story;
    public Stage[] Stages => stages.ToArray();
}
