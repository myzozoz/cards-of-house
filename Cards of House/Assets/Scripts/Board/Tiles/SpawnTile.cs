using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SpawnTile : Tile {
    public void SetGameObjectEnabled(bool enabled)
    {
        gameObject.SetActive(enabled);
    }

#if UNITY_EDITOR
// The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/SpawnTile")]
    public static void CreateSpawnTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Spawn Tile", "New Spawn Tile", "Asset", "Save Spawn Tile", "Assets");
        if (path == "")
            return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SpawnTile>(), path);
    }
#endif
}
