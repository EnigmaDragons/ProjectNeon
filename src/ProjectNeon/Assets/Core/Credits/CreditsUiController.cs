
using UnityEngine;

public class CreditsUiController : OnMessage<ToggleCredits>
{
    [SerializeField] private GameObject obj;

    private void Awake() => obj.SetActive(false);
    
    protected override void Execute(ToggleCredits msg) => obj.SetActive(!obj.activeSelf);
}