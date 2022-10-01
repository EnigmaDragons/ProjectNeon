using UnityEngine;

[CreateAssetMenu(menuName = "GameState/Clinic")]
public class ClinicState : ScriptableObject
{
    public Corp Corp { get; set; }
    public bool IsTutorial { get; set; }
}