﻿using UnityEngine;

public class Universal2DAngleShift : MonoBehaviour
{
    public const float angle = 26;

    private void Awake()
    {
        transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }

    public static Quaternion Euler => Quaternion.Euler(angle, 0, 0);
    
    public void Revert() => transform.localRotation = Quaternion.identity;
}