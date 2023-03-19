using System;
using System.Globalization;
using UnityEngine;

public class BetaTimeLockController : MonoBehaviour
{
    [SerializeField] private GameObject[] toEnable;
    [SerializeField] private GameObject[] toDisable;
    [SerializeField] private BoolReference isBeta;
    [SerializeField] private string utcMinimumTime;
    [SerializeField] private string utcMaximumTime;

    private bool _locked;

    private void Awake()
    {
        if (!isBeta.Value)
            return;
        if (!DateTime.TryParseExact(utcMinimumTime, "yyyy/MM/dd HH:mm", new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal, out DateTime minimumTime))
            return;
        if (!DateTime.TryParseExact(utcMaximumTime, "yyyy/MM/dd HH:mm", new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal, out DateTime maximumTime))
            return;
        var now = DateTime.UtcNow;
        if (now >= minimumTime && now <= maximumTime)
            return;
        toEnable.ForEach(x => x.SetActive(true));
        toDisable.ForEach(x => x.SetActive(false));
        _locked = true;
    }
    
    private void Update()
    {
        if (_locked)
        {
            toEnable.ForEach(x => x.SetActive(true));
            toDisable.ForEach(x => x.SetActive(false));
        }
    }
}