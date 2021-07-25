using UnityEngine;

public class ShowIfHasCurrentGame : OnMessage<RefreshMainMenu>
{
    [SerializeField] private GameObject target;

    private void Start() => Render();
    
    private void Render() => target.SetActive(CurrentGameData.HasActiveGame);
    
    protected override void Execute(RefreshMainMenu msg) => Render();
}