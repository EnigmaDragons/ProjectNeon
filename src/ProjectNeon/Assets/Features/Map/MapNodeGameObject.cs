using UnityEngine;
using UnityEngine.UI;

public class MapNodeGameObject : MonoBehaviour
{
    [SerializeField] private Button button;

    public void Init(bool canTravelToo)
    {
        button.enabled = canTravelToo;
    } 
}