using I2.Loc;
using TMPro;
using UnityEngine;

public class MapNodeDescription : MonoBehaviour
{
    [SerializeField] private Localize nameLabel;
    [SerializeField] private Localize detailLabel;

    public void Init(string nodeName, string nodeDetail)
    {
        nameLabel.SetTerm(nodeName);
        detailLabel.SetTerm(nodeDetail);
    }
}
