using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit: ITarget
{
    public void Execute();
    public System.Guid GetId();
}
