using System;
using System.Text;
using CI.HttpClient;
using UnityEngine;

public static class AllMetrics
{
    private static readonly HttpClient Client = new HttpClient();
    
    private static string _securedUrl = "aHR0cHM6Ly9wcm9kLTA5Lndlc3R1czIubG9naWMuYXp1cmUuY29tOjQ0My93b3JrZmxvd3MvNTNlNjEzMDQ3NjMwNGNmN2E4N2M5MjZlMzNjYjlhNDYvdHJpZ2dlcnMvbWFudWFsL3BhdGhzL2ludm9rZT9hcGktdmVyc2lvbj0yMDE2LTEwLTAxJnNwPSUyRnRyaWdnZXJzJTJGbWFudWFsJTJGcnVuJnN2PTEuMCZzaWc9eUlCTnNZOXo4NkI2QnJwWUhBSFR0R0NaRVlRSGV4WGpUYjVLVDVCVW9uaw==";
    private static string _installId = "Not Initialized";
    private static string _runId = "Not Initialized";
    private static string _version = "Not Initialized";
    private static bool _isEditor = false;
    
    public static void Init(string version, string installId)
    {
        _version = version;
        _installId = installId;
#if UNITY_EDITOR
        _isEditor = true;
#endif
    }

    public static void SetRunId(string runId) => _runId = runId;

    public static void PublishSelectedParty(string adventureName, string[] selectedHeroes)
        => Send("selectedSquad", new SquadSelectedData {adventureName = adventureName, heroNames = selectedHeroes});

    public static void PublishCardRewardSelection(string selectedCardName, string[] optionNames)
        => Send("rewardCardSelected", new OptionSelectionData {selected = selectedCardName, options = optionNames});

    public static void PublishGearRewardSelection(string selectedGearNameOrDescription, string[] optionNameOrDescriptions)
        => Send("rewardGearSelected", new OptionSelectionData {selected = selectedGearNameOrDescription, options = optionNameOrDescriptions});

    public static void PublishLevelUpOptionSelection(string heroName, int level, string selectedDescription, string[] optionsDescription)
        => Send("heroLevelUp", new HeroLevelUpSelectionData {heroName = heroName, level = level, selection = selectedDescription, options = optionsDescription});
    
    public static void PublishMapNodeSelection(int mapProgress, string selectedMapNodeName, string[] mapNodeOptions)
        => Send("mapNodeSelected", new MapNodeSelectionData { progress = mapProgress, selected = selectedMapNodeName, options = mapNodeOptions});

    public static void PublishBattleSummary(BattleSummaryReport report)
        => Send("battleSummary", report);

    public static void PublishGameLost()
        => Send("gameLost", new NoEventData());

    public static void PublishGameWon()
        => Send("gameWon", new NoEventData());
    
    private static void Send(string eventName, object payload)
        => Send(new GeneralMetric(eventName, JsonUtility.ToJson(payload)));
    
    private static void Send(GeneralMetric m)
    {
        if (!_isEditor)
            Client.Post(
                new Uri(_securedUrl.FromBase64(), UriKind.Absolute),
                new StringContent(
                    JsonUtility.ToJson(new GeneralMetricData
                    {
                        gameVersion = WithEditorInfoAppended(_version),
                        installId = _installId,
                        runId = _runId,
                        eventType = m.EventType,
                        @event = m.Event
                    }),
                    Encoding.UTF8,
                    "application/json"),
                HttpCompletionOption.AllResponseContent,
                OnResponse);
    }

    private static string WithEditorInfoAppended(string version) => _isEditor ? $"{version} Editor" : version;

    private static void OnResponse(HttpResponseMessage resp)
    {
        if (!resp.IsSuccessStatusCode)
            Log.Error($"Failed to submit Error Message: {resp.StatusCode}");
    }

    [Serializable]
    private class GeneralMetricData
    {
        public string gameVersion;
        public string installId;
        public string runId;
        public string eventType;
        public string @event;
    }

    [Serializable]
    private class SquadSelectedData
    {
        public string adventureName;
        public string[] heroNames;
    }

    [Serializable]
    private class OptionSelectionData
    {
        public string selected;
        public string[] options;
    }

    [Serializable]
    private class HeroLevelUpSelectionData
    {
        public string heroName;
        public int level;
        public string selection;
        public string[] options;
    }

    [Serializable]
    private class MapNodeSelectionData
    {
        public int progress;
        public string selected;
        public string[] options;
    }
    
    [Serializable]
    private class NoEventData {}
}
