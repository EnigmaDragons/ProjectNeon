using UnityEngine;

public class ShowBetaCompetitionSecretObjective : MonoBehaviour
{
    [SerializeField] private GameObject ifSeen;
    [SerializeField] private GameObject ifUnseen;

    private void Awake()
    {
        var seen = CurrentProgressionData.Data.HasSeenAlgeronFinalBoss;
        ifSeen.SetActive(seen);
        ifUnseen.SetActive(!seen);
    }
}
