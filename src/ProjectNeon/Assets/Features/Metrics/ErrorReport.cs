using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CI.HttpClient;
using UnityEngine;

public static class ErrorReport
{
    private static readonly HttpClient Client = new HttpClient();

    private static readonly string _securedUrl =
        "aHR0cHM6Ly9wcm9kLTE4Lndlc3R1czIubG9naWMuYXp1cmUuY29tOjQ0My93b3JrZmxvd3MvNDVlZmIwYTc3MjllNGI1NzhhYzcwOWY4NjBhMjRhNTQvdHJpZ2dlcnMvbWFudWFsL3BhdGhzL2ludm9rZT9hcGktdmVyc2lvbj0yMDE2LTEwLTAxJnNwPSUyRnRyaWdnZXJzJTJGbWFudWFsJTJGcnVuJnN2PTEuMCZzaWc9bDVUY2lXV3VDYXRqN2pwYVI0aGJEM2VJaHJlakc4U0d1ZFFGNXNrZGJjNA==";

    private static string _appName = "Not Initialized";
    private static string _version = "Not Initialized";
    private static Queue<string> _recentLogs = new Queue<string>();
    private static bool _isEditor = false;
    private static string[] _ignoreIfContainsInEditor = {  "Coroutine couldn't be started" };

public static void Init(string appName, string version, Queue<string> recentLogs)
    {
        _appName = appName;
        _version = version;
        _recentLogs = recentLogs;
#if UNITY_EDITOR
        _isEditor = true;
#endif
    }
    
    public static void Test() => throw new Exception("Test Exception");

    public static void Send(string errorMessage)
    {
#if UNITY_EDITOR
        if (_ignoreIfContainsInEditor.Any(errorMessage.Contains))
            return;
#endif

        Client.Post(
            new Uri(_securedUrl.FromBase64(), UriKind.Absolute),
            new StringContent(
                JsonUtility.ToJson(new ErrorDetail
                {
                    appName = WithEditorInfoAppended(_appName),
                    version = "v" + _version,
                    errorMessage = errorMessage,
                    recentLogs = string.Join("<br>", _recentLogs.ToList())
                }),
                Encoding.UTF8,
                "application/json"),
            HttpCompletionOption.AllResponseContent,
            OnResponse);
    }

    private static string WithEditorInfoAppended(string appName) => _isEditor ? $"{appName} Editor" : appName;

    private static void OnResponse(HttpResponseMessage resp)
    {
        if (!resp.IsSuccessStatusCode)
            Log.Error($"Failed to submit Error Message: {resp.StatusCode}");
    }

    [Serializable]
    public class ErrorDetail
    {
        public string appName;
        public string version;
        public string errorMessage;
        public string recentLogs = "";
    }
}
