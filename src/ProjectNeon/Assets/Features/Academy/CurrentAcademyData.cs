using System;
using System.Linq;

public class CurrentAcademyData
{
    private static Stored<AcademyData> _stored = new MemoryStored<AcademyData>(new AcademyData());
    private static AcademyData _current = new AcademyData();

    public static AcademyData Data => _current;

    public static void Init(Stored<AcademyData> stored)
    {
        _stored = stored;
        _current = _stored.Get();
    }

    public static void Mutate(Action<AcademyData> mutate)
    {
        Write(d =>
        {
            mutate(d);
            return d;
        });
    }
    
    private static void Write(Func<AcademyData, AcademyData> transform)
    {
        var before = Data.ToSnapshot();
        _stored.Write(transform);
        _current = transform(_current);
        Message.Publish(new AcademyDataUpdated(before, _current.ToSnapshot()));
    }

    public static void Clear() => Write(_ => new AcademyData());

    public static void Skip() => Write(a =>
    {
        a.TutorialData.CompletedTutorialNames = a.TutorialData.CompletedTutorialNames
            .Concat(AcademyData.RequiredLicenseTutorials)
            .Concat(AcademyData.SimpleTutorialPanels)
            .Distinct()
            .ToArray();
        return a;
    });
}
