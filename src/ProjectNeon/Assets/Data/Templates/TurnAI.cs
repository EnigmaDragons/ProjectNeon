﻿using UnityEngine;

public abstract class TurnAI : ScriptableObject
{
    public abstract PlayedCard Play(int memberId);
}
