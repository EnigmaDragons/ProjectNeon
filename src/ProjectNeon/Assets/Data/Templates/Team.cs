using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : Target
{
    public Team(List<Member> members)
    {
        this.targets = members;
    }
}
