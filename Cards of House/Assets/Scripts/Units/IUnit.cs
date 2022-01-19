using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit: ITarget
{
    public void Execute();
    public bool CanAct();
    public Vector3 GetPosition();
    public Transform GetTransform();
}
