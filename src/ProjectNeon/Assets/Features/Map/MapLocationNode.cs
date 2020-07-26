using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class MapLocationNode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;
    [SerializeField] private Button button;

    private CurrentGameMap _map;
    private MapLocation _location;
    
    public void Init(CurrentGameMap map, MapLocation l)
    {
        _map = map;
        _location = l;
        nameLabel.text = l.DisplayName;
        descriptionLabel.text = l.Description;
    }
    
    //private void Awake() => button.onClick.AddListener(TravelToLocation);

    //private void TravelToLocation() => Message.Publish(new GoToLocation { Location = _location });
}