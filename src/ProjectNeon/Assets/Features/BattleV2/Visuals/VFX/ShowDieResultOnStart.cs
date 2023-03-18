using UnityEngine;

public class ShowDieResultOnStart : MonoBehaviour
{
    private void Start() => Message.Publish(new ShowDieResult());
}