using System;
using CI.HttpClient;
using UnityEngine;

public class LoadLatestPatchNotesOnAwake : MonoBehaviour
{
    [SerializeField] private LongText text;

    private HttpClient _client;

    private void Awake()
    {
        _client = new HttpClient();
        _client.Get(new Uri("https://raw.githubusercontent.com/EnigmaDragons/ProjectNeon/master/patchnotes/latest.md"), 
            HttpCompletionOption.AllResponseContent, OnResponse);
    }
    
    private void OnResponse(HttpResponseMessage resp)
    {
        if (!resp.IsSuccessStatusCode)
            Log.Warn($"Failed to load Latest Patch Notes: {resp.StatusCode}");
        else
            text.Init(resp.ReadAsString() 
                      + Environment.NewLine 
                      + Environment.NewLine);
        
    }
}