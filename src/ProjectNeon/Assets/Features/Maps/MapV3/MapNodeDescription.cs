using TMPro;
using UnityEngine;

public class MapNodeDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI detailLabel;

    public void Init(string nodeName, string nodeDetail)
    {
        nameLabel.text = nodeName;
        detailLabel.text = nodeDetail;
    }
}
