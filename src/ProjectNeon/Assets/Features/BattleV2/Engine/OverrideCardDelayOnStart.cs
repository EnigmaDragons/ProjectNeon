using UnityEngine;

public class OverrideCardDelayOnStart : MonoBehaviour
{
    [SerializeField] private TeamType team;
    [SerializeField] private float delay;
    
    private void Start()
    {
        Message.Publish(new OverrideCardDelay(team, delay));
    }
}