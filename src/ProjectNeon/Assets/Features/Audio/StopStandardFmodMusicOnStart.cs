
using UnityEngine;

public class StopStandardFmodMusicOnStart : MonoBehaviour
{
    private void Start() => Message.Publish(new StopStandardFmodMusic());
}