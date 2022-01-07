using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoard
{
    public void Step();
    public List<IUnit> GetUnitsOnBoard();
    public Vector3Int GetUnitLocation(System.Guid unitId);
    public Grid GetGrid();
}
