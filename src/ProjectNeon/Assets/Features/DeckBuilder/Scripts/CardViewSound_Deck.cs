using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewSound_Deck : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string Event;
    [FMODUnity.EventRef]
    public string Event2;
    public void OnEnable()
    {

        FMODUnity.RuntimeManager.PlayOneShotAttached(Event, gameObject);
    }

    public void OnDisable()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(Event2, gameObject);
    }
}
