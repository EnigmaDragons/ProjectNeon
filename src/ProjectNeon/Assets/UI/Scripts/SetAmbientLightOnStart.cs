using UnityEngine;

public class SetAmbientLightOnStart : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private float intensity;
    
    private void Start()
    {
        RenderSettings.ambientLight = color;
        RenderSettings.ambientIntensity = intensity;
        DynamicGI.UpdateEnvironment();
    }   
}