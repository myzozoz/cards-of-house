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
    private Dictionary<System.Guid,IUnit> units;
    private Queue<System.Guid> actionQueue;
    // Start is called before the first frame update
    void Start()
    {
        grid = transform.GetComponent<Grid>();
        tilemap = transform.GetComponentInChildren<Tilemap>();
        units = new Dictionary<System.Guid, IUnit>();
        actionQueue = new Queue<System.Guid>();
        if (tilemap == null)
        {
            Debug.Log("No Tilemap found, please check that one such exists");
        }
        List<IUnit> unitList = new List<IUnit>(transform.GetComponentsInChildren<IUnit>());
        foreach (IUnit unit in unitList)
        {
            if (unit != null)
            {
                units.Add(unit.GetId(), unit);
                actionQueue.Enqueue(unit.GetId());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //AddRandomForbiddenTiles(forbiddenTiles);
        }

    }

    public void Step()
    {
        IUnit current = units[actionQueue.Dequeue()];
        Debug.Log("Now in turn: " + current);
        //Do actions
        current.Execute();
        //Back to the queue
        actionQueue.Enqueue(current.GetId());
        Debug.Log("Next up: " + actionQueue.Peek());
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

    public List<IUnit> GetUnitsOnBoard()
    {
        return new List<IUnit>(units.Values);
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

    public void ResetPathTiles()
    {
        Debug.Log(tilemap.cellBounds.xMin + ", " + tilemap.cellBounds.xMax + ", " + tilemap.cellBounds.yMin + ", " + tilemap.cellBounds.yMax);
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
}
