using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "Adventure/Adventure")]
public class Adventure : ScriptableObject
{
    [SerializeField] private Stage[] stages;

    // @todo #1:15min Design. What happens when the adventure is won?

    public Stage[] Stages => stages.ToArray();
}
