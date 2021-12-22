using UnityEngine;

public sealed class BattleVFXTester : MonoBehaviour
{
    [SerializeField] private StringReference fx;

    private Camera _camera;

    private void Awake() => _camera = Camera.main;
    
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
            return;

        Log.Info("Clicked Mouse Button");
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, 500f))
        {
            Log.Info(hit.point.ToString());
            Message.Publish(new PlayRawBattleEffect(fx, hit.point + new Vector3(0, 1.2f, 0), Input.GetMouseButtonDown(1)));
        }
        else
            Log.Info("Didn't Hit Anything");
    }
}
