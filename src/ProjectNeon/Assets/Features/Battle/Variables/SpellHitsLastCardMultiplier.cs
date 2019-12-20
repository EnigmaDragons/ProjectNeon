using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHitsLastCardMultiplier : FloatVariable
{
    [SerializeField] CardResolutionZone resolutionZone;

    public new float Value => resolutionZone.SpellHitsOnLastCard * 0.1f;

}
