using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A target for some effect in battlefield
 */
public abstract class Target : ScriptableObject {

    [SerializeField] public List<Member> targets;
}
