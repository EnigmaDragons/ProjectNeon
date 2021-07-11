using System;

public interface ClinicServiceProvider
{
    string GetTitle();
    ClinicServiceButtonData[] GetOptions();
}

public class ClinicServiceButtonData
{
    public string Name { get; }
    public string Description { get; }
    public int Cost { get; }
    public Action Action { get; }
    public bool Enabled { get; }

    public ClinicServiceButtonData(string name, string description, int cost, Action action, bool enabled)
    {
        Name = name;
        Description = description;
        Cost = cost;
        Action = action;
        Enabled = enabled;
    }
}