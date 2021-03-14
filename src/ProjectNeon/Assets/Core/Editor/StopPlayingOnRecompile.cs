using UnityEditor;
 
[InitializeOnLoad]
public class StopPlayingOnRecompile
{
    static StopPlayingOnRecompile()
    {
        EditorApplication.update -= StopPlayingIfRecompiling;
        EditorApplication.update += StopPlayingIfRecompiling;
    }
 
    static void StopPlayingIfRecompiling()
    {
        if(EditorApplication.isCompiling && EditorApplication.isPlaying) 
            EditorApplication.isPlaying = false;
    }
}