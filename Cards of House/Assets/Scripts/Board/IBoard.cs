using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IBoard: IStage
{
    public void Simulate(Command command);
    public void SubmitCommand(int team, Command command);
    public void SubmitTarget(int team, TargetType targetType);
    public List<IUnit> GetUnitsOnBoard();
    public List<IUnit> GetUnitsOnBoard(int team);
    public List<ITarget> GetTargetsOnBoard();
    public List<ITarget> GetTargetsOnBoard(int team);
    public Vector3Int GetUnitLocation(System.Guid unitId);
    public Grid GetGrid();
    public Tilemap GetTilemap();
    public bool IsFree(Vector3Int loc);
    public void ResetPathTiles();
    public void Deregister(System.Guid unitId);
    public void SpawnUnit(GameObject unit, Vector3Int spawnLocation);
    public void SetCam(ICameraController camController);
    public Transform GetSpawnCenterTransform();
    public void EnableSpawnSelection();
    public void DisableSpawnSelection();
    public void TrySelectSpawnTile(SpawnRim tile);
    public void UpdateSpawns();
    public int Round { get; }
    public void RegisterAvatarDeath(AvatarUnit au);
    public bool ReserveTile(Vector3Int t);
    public bool ReleaseTile(Vector3Int t);
}
