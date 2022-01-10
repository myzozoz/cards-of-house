using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Navigator
{
    public static List<Vector3Int> FindLocationsWithinAttackDistance(Tilemap tilemap, ITarget target, int pReach, int sReach)
    {
        List<Vector3Int> locationList = new List<Vector3Int>();
        Vector3Int tLoc = target.GetLocation();
        Vector3Int tempLoc;
        Debug.Log("Finding locations within attack distance of " + tLoc + " (" + pReach + "," + sReach + "):");
        // Primary wind directions (horizontal/vertical)
        for (int i = 1; i <= pReach; i++)
        {
            tempLoc = tLoc + new Vector3Int(i, 0, 0);
            if (tilemap.HasTile(tempLoc) && tilemap.GetTile(tempLoc).name != "Forbidden")
            {
                locationList.Add(tempLoc);
            }
            tempLoc = tLoc + new Vector3Int(-i, 0, 0);
            if (tilemap.HasTile(tempLoc) && tilemap.GetTile(tempLoc).name != "Forbidden")
            {
                locationList.Add(tempLoc);
            }
            tempLoc = tLoc + new Vector3Int(0, i, 0);
            if (tilemap.HasTile(tempLoc) && tilemap.GetTile(tempLoc).name != "Forbidden")
            {
                locationList.Add(tempLoc);
            }
            tempLoc = tLoc + new Vector3Int(0, -i, 0);
            if (tilemap.HasTile(tempLoc) && tilemap.GetTile(tempLoc).name != "Forbidden")
            {
                locationList.Add(tempLoc);
            }
        }
        // Secondary wind directions (diagonal)
        for (int i = 1; i <= sReach; i++)
        {
            tempLoc = tLoc + new Vector3Int(i, i, 0);
            if (tilemap.HasTile(tempLoc) && tilemap.GetTile(tempLoc).name != "Forbidden")
            {
                locationList.Add(tempLoc);
            }
            tempLoc = tLoc + new Vector3Int(-i, i, 0);
            if (tilemap.HasTile(tempLoc) && tilemap.GetTile(tempLoc).name != "Forbidden")
            {
                locationList.Add(tempLoc);
            }
            tempLoc = tLoc + new Vector3Int(i, -i, 0);
            if (tilemap.HasTile(tempLoc) && tilemap.GetTile(tempLoc).name != "Forbidden")
            {
                locationList.Add(tempLoc);
            }
            tempLoc = tLoc + new Vector3Int(-i, -i, 0);
            if (tilemap.HasTile(tempLoc) && tilemap.GetTile(tempLoc).name != "Forbidden")
            {
                locationList.Add(tempLoc);
            }
        }
        Debug.Log("Found " + locationList.Count + " possible attack locations");
        return locationList;
    }

}
