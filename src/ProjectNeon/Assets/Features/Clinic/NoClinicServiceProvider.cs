public class NoClinicServiceProvider : ClinicServiceProvider
{
    public string GetTitleTerm() => "";
    public ClinicServiceButtonData[] GetOptions() => new ClinicServiceButtonData[0];
    public bool RequiresSelection() => false;
}