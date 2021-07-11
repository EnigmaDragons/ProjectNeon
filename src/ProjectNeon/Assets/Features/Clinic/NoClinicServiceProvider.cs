public class NoClinicServiceProvider : ClinicServiceProvider
{
    public string GetTitle() => "";
    public ClinicServiceButtonData[] GetOptions() => new ClinicServiceButtonData[0];
}