using System;
using UnityEngine;

[Serializable]
public class SegmentRangeBattlefieldSet
{
    [SerializeField] private int startsAtSegmentIndex = 0;
    [SerializeField] private BattlefieldSet battlefieldSet;

    public int StartsAtSegmentIndex => startsAtSegmentIndex;
    public BattlefieldSet Set => battlefieldSet;
    public GameObject GetNext() => battlefieldSet.GetNext();
}
