#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class RunQaBeforeBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        var succeeded = QualityAssurance.Go();
        if (!succeeded)
            throw new BuildFailedException("Neon Quality Assurance Checks Failed");
    }
}
#endif
