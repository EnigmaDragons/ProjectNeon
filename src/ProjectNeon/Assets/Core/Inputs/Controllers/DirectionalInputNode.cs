using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DirectionalInputNode
{
    [SerializeField] public GameObject Up;
    [SerializeField] public GameObject Left; 
    [SerializeField] public GameObject Selectable; 
    [SerializeField] public GameObject Right;
    [SerializeField] public GameObject Down;
                            
    public GameObject GetObjectInDirection(InputDirection direction)
    {
        if (direction == InputDirection.Up) return Up;
        if (direction == InputDirection.Left) return Left;
        if (direction == InputDirection.Right) return Right;
        if (direction == InputDirection.Down) return Down;
        return Selectable;
    }
}