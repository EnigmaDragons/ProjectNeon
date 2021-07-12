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
    public bool Enabled { get; set; }

    public ClinicServiceButtonData(string name, string description, int cost, Action action)
    {
        Name = name;
        Description = description;
        Cost = cost;
        Action = action;
    }
}