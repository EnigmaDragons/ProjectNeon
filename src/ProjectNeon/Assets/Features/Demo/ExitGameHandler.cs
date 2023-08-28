using UnityEngine;

public class ExitGameHandler : OnMessage<ExitGameRequested>
{
    [SerializeField] private BoolReference isDemo;
    
    protected override void Execute(ExitGameRequested msg)
    {
        if (!isDemo)
            ExitGame();
        else
        {
            Message.Publish(new HideNamedTarget("InGameMenu"));
            Message.Publish(new ToggleNamedTarget("WishlistUI"));
        }
    }
    
    public void ExitGame()
    {     

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
#elif UNITY_STANDALONE_WIN
        Application.Quit();
#else
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
}
