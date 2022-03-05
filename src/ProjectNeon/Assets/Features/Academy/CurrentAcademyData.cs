using System;

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
    
    public static void Write(Func<AcademyData, AcademyData> transform)
    {
        _stored.Write(transform);
        _current = transform(_current);
    }

    public static void Save() => _stored.Write(_ => _current);
    public static void Clear() => Write(_ => new AcademyData());
}
