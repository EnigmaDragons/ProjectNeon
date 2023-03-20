using I2.Loc;
using TMPro;
using UnityEngine;
using System.Linq;

public class MapNodeDescription : MonoBehaviour
{
    [SerializeField] private Localize nameLabel;
    [SerializeField] private Localize detailLabel;
    [SerializeField] private CurrentAdventure adventure;
    
    public void Init(string nodeName, string nodeDetail, StageSegment stageSegment)
    {
        var bossStages = adventure.Adventure.StagesV5[0].Segments.Where(x => x.MapNodeType == MapNodeType.Boss).ToArray();
        if (stageSegment.MapNodeType == MapNodeType.Boss && adventure.Adventure.IsV5)
        {
            var count = bossStages.Length;
            var bossStage = bossStages.FirstIndexOf(x => x == stageSegment);
            nameLabel.SetFinalText(nodeName.ToLocalized().SafeFormat($"{bossStage + 1}/{count}"));
            detailLabel.SetTerm(nodeDetail);   
        }
        else
        {
            nameLabel.SetTerm(nodeName);
            detailLabel.SetTerm(nodeDetail);   
        }
    }

    public void Init(string nodeName, string nodeDetail)
    {
        nameLabel.SetTerm(nodeName);
        detailLabel.SetTerm(nodeDetail);   
    }
}
