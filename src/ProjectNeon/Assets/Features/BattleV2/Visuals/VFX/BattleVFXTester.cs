using UnityEngine;

public sealed class BattleVFXTester : MonoBehaviour
{
    [SerializeField] private StringReference fx;

    private Camera _camera;

    private void Awake() => _camera = Camera.main;
    
    
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Log.Info("Clicked Mouse Button");
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out var hit, 500f))
            Message.Publish(new PlayRawBattleEffect(fx, hit.point));
    }
}
