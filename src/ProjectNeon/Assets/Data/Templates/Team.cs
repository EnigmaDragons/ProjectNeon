using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : Target
{
    public Team Init(List<Member> members)
    {
        this.targets = members;
        return this;
    }
}
