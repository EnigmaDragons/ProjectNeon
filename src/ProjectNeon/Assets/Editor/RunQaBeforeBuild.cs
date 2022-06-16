#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;

public class RunQaBeforeBuild : IPreprocessBuild
{
    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        var succeeded = QualityAssurance.Go();
        if (!succeeded)
            throw new BuildFailedException("Neon Quality Assurance Checks Failed");
    }
}
#endif
