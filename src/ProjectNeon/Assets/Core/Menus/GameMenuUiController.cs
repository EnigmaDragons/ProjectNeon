
using UnityEngine;

public class GameMenuUiController : OnMessage<ToggleGameMenu>
{
    [SerializeField] private GameObject target;

    protected override void Execute(ToggleGameMenu msg) => target.SetActive(!target.activeSelf);
}