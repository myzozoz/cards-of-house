using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;


public class BoardControllerScript : MonoBehaviour
{
    public int width;
    public int depth;
    public int forbiddenTiles;
    public TileBase defaultTile;
    public TileBase forbiddenTile;

    private Grid grid;
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        grid = transform.GetComponent<Grid>();
        tilemap = transform.GetComponentInChildren<Tilemap>();
        if (tilemap != null)
        {
            Debug.Log("Tilemap found");
            Debug.Log("bounds: " + tilemap.cellBounds);
            Debug.Log("Contains forbidden tile: " + tilemap.ContainsTile(forbiddenTile));
            Debug.Log("bounds: " + tilemap.cellBounds);
            OverrideBoxFill2D(new Vector3Int(0, 0, 0), defaultTile, 0, 0, width-1, depth-1);
            AddRandomForbiddenTiles(forbiddenTiles);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Debug.Log("Escape pressed");
            Stop();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Debug.Log("Next");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            AddRandomForbiddenTiles(forbiddenTiles);
        }

    }

    public void Stop()
    {
        SceneManager.LoadScene("MainMenu");
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
}
