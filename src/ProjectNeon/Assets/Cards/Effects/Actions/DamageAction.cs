using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]

/**
 * @todo #54:30min Create Target element so we can execute each Action on it. Then
 *  add it to all actions inside Cards/Effects/Actions and complete the actions implementations
 */
public class DamageAction : ScriptableObject
{
    [SerializeField]
    private int quantity;
}
