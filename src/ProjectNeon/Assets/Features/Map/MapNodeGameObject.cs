using UnityEngine;
using UnityEngine.UI;

public class MapNodeGameObject : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;
    
    public void Init(string nodeId, bool canTravelToo)
    {
        button.interactable = canTravelToo;
        button.onClick.AddListener(() => Message.Publish(new TravelToNode { Node = gameObject, OnArrive = () => segment.Start(), NodeId = nodeId }));
    } 
    
    public void SetCanTravelTo(bool canTravelTo) => button.interactable = canTravelTo;
}