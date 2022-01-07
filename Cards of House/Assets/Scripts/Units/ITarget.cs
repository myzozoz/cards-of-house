using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    public TargetType GetTargetType();
    public int GetTeam();
    public Vector3Int GetLocation();
}
