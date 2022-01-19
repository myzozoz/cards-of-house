using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShotData
{
    public List<Shot> Shots;

    public ShotData(List<Shot> shots)
    {
        Shots = shots;
    }
}
