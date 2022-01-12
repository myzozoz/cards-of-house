using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BoardControllerScript : MonoBehaviour, IBoard
{
    public int width;
    public int depth;
    public int forbiddenTiles;
    public TileBase defaultTile;
    public TileBase forbiddenTile;

    private Grid grid;
    private Tilemap tilemap;
    private Dictionary<System.Guid, IUnit> units;
    private Dictionary<System.Guid, ITarget> targets;
    private Queue<System.Guid> actionQueue;
    private HashSet<System.Guid> deregisterBuffer;
    // Start is called before the first frame update
    void Start()
    {
        grid = transform.GetComponent<Grid>();
        tilemap = transform.GetComponentInChildren<Tilemap>();
        units = new Dictionary<System.Guid, IUnit>();
        targets = new Dictionary<System.Guid, ITarget>();
        actionQueue = new Queue<System.Guid>();
        deregisterBuffer = new HashSet<System.Guid>();
        if (tilemap == null)
        {
            Debug.Log("No Tilemap found, please check that one such exists");
        }
        UpdateUnits();
        UpdateTargets();
        foreach (IUnit unit in units.Values)
        {
            actionQueue.Enqueue(unit.GetId());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //AddRandomForbiddenTiles(forbiddenTiles);
            Debug.Log($"Units in queue {actionQueue.Count}");
            Debug.Log($"Ids in deregister buffer {deregisterBuffer.Count}");
        }

    }

    public void Step()
    {
        System.Guid currentId = actionQueue.Dequeue();
        if (!deregisterBuffer.Contains(currentId)) {
            IUnit current = units[currentId];
            //Debug.Log("Now in turn: " + current);
            //Do actions
            current.Execute();
            //Back to the queue
            actionQueue.Enqueue(currentId);
        } else {
            UpdateTargets();
            UpdateUnits();
            deregisterBuffer.Remove(currentId);
        }
        
    }

    private void OverrideBoxFill2D(Vector3Int start, TileBase tile, int startX, int startY, int endX, int endY)
    {
        int dirX = startX < endX ? 1 : -1;
        int dirY = startY < endY ? 1 : -1;
        int cols = 1 + Mathf.Abs(endX - startX);
        int rows = 1 + Mathf.Abs(endY - startY);

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3Int pos = start + new Vector3Int(startX, startY, 0) + new Vector3Int(dirX * x, dirY * y, 0);
                tilemap.SetTile(pos, tile);
            }
        }
    }

    private void AddRandomForbiddenTiles(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3Int pos = new Vector3Int(Random.Range(0, width), Random.Range(1, depth - 1), 0);
            while (tilemap.GetTile(pos).name == forbiddenTile.name)
            {
                Debug.Log("Tile already at " + pos);
                pos = new Vector3Int(Random.Range(0, width), Random.Range(1, depth - 1), 0);
                Debug.Log("New pos: " + pos);
            }

            Debug.Log("Setting tile at " + pos);
            tilemap.SetTile(pos, forbiddenTile);
        }
    }

    private void InitializeMap()
    {
        OverrideBoxFill2D(new Vector3Int(0, 0, 0), defaultTile, 0, 0, width - 1, depth - 1);
        AddRandomForbiddenTiles(forbiddenTiles);
    }

    private void UpdateUnits()
    {
        units.Clear();
        List<IUnit> unitList = new List<IUnit>(transform.GetComponentsInChildren<IUnit>());
        foreach (IUnit unit in unitList)
        {
            if (unit != null && unit.IsAlive() && unit.CanAct())
            {
                units.Add(unit.GetId(), unit);
            }
        }
    }

    private void UpdateTargets()
    {
        targets.Clear();
        List<ITarget> targetList = new List<ITarget>(transform.GetComponentsInChildren<ITarget>());
        foreach (ITarget target in targetList)
        {
            if (target != null && target.IsAlive())
            {
                targets.Add(target.GetId(), target);
            }
        }
        Debug.Log($"A total of {targetList.Count} targets found");
    }

    public List<IUnit> GetUnitsOnBoard()
    {
        return new List<IUnit>(units.Values);
    }

    public List<ITarget> GetTargetsOnBoard()
    {
        return new List<ITarget>(targets.Values);
    }

    public Vector3Int GetUnitLocation(System.Guid unitId)
    {
        return units[unitId].GetLocation();
    }

    public Grid GetGrid()
    {
        return grid;
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }

    public bool IsFree(Vector3Int loc)
    {
        if (!tilemap.HasTile(loc))
        {
            return false;
        }

        if (tilemap.GetTile(loc).name == "Forbidden")
        {
            return false;
        }

        foreach (ITarget t in targets.Values)
        {
            if (t.GetLocation() == loc)
            {
                return false;
            }
        }

        return true;
    }

    public void ResetPathTiles()
    {
        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int temp = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(temp) && tilemap.GetTile(temp).name == "Path")
                {
                    tilemap.SetTile(temp, defaultTile);
                }
            }
        }
    }

    public void Deregister(System.Guid unitId)
    {
        deregisterBuffer.Add(unitId);
        UpdateUnits();
        UpdateTargets();
    }
}
