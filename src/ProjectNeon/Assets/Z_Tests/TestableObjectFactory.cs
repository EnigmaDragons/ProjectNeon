using System.Runtime.Serialization;

public static class TestableObjectFactory 
{
    public static T Create<T>() => (T)FormatterServices.GetUninitializedObject(typeof(T));
}
