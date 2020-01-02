using System.Linq;
using UnityEngine;

public sealed class GameEventKeyTrigger : MonoBehaviour
{
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private GameEvent onPress;

    private void Update()
    {
        if (keys.Any(Input.GetKeyDown))
            onPress.Publish();
    }
}
