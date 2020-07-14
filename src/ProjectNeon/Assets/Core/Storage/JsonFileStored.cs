using System;
using System.IO;
using UnityEngine;

public sealed class JsonFileStored<T> : Stored<T>
{
    private readonly string _filename;
    private readonly Func<T> _getDefaultValue;
    private T _item;
    
    public JsonFileStored(string filename, Func<T> getDefaultValue)
    {
        _filename = filename;
        _getDefaultValue = getDefaultValue;
    }
    
    public T Get()
    {
        if (_item == null)
            InitItem();
        return _item;
    }

    private void InitItem()
    {
        try
        {
            if (!File.Exists(_filename))
                _item = _getDefaultValue();
            else
                using (var reader = new StreamReader(_filename))
                    _item = JsonUtility.FromJson<T>(reader.ReadToEnd());
        }
        catch (Exception e)
        {
            Debug.LogError($"Reading {typeof(T)} from file {_filename} - {e}");
            _item = _getDefaultValue();
        }
    }

    public void Write(Func<T, T> transform)
    {
        _item = transform(Get());
        try
        {
            using (var writer = new StreamWriter(_filename))
                writer.Write(JsonUtility.ToJson(_item));
        }
        catch (Exception e)
        {
            Debug.LogError($"Writing {typeof(T)} to file {_filename} - {e}"); 
        }
    }
}
