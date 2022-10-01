using System.Collections.Generic;
using UnityEngine;

public class InitMetrics : MonoBehaviour
{
    [SerializeField] private StringReference appName;
    [SerializeField] private StringReference version;
    [SerializeField] private BoolReference isDemo;
    [SerializeField] private bool sendEventOnLaunch = true;
    
    private void Awake()
    {
        AllMetrics.Init(version, InstallId.Get(), isDemo ? "Demo" : "");
        InitErrorReporting();
        if (sendEventOnLaunch)
            AllMetrics.PublishInteractedWith("Game Launched");
    }

    private void InitErrorReporting()
    {
        var recentLogsQueue = new Queue<string>();
        var maxRecentQueueLogs = 15;
        ErrorReport.Init(appName, version, recentLogsQueue);
        Log.AddSink(s =>
        {
            while (recentLogsQueue.Count > maxRecentQueueLogs - 1)
                recentLogsQueue.Dequeue();
            recentLogsQueue.Enqueue(s);
        });
    }
}
