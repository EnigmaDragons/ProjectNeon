using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    public float timer, refresh, avgFramerate;
    
    private const string Display = "{0} FPS";
 
    private void Update()
    {
        //Change smoothDeltaTime to deltaTime or fixedDeltaTime to see the difference
        var timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;
 
        if (timer <= 0) 
            avgFramerate = (int) (1f / timelapse);
        label.text = string.Format(Display, avgFramerate.ToString());
    }
}