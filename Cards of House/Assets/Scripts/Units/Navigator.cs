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
        //Debug.Log("Finding locations within attack distance of " + tLoc + " (" + pReach + "," + sReach + "):");
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
        //Debug.Log("Found " + locationList.Count + " possible attack locations");
        return locationList;
    }

    public static Vector3Int FindNearestLocation(Vector3Int s, List<Vector3Int> locs)
    {
        float minDist = 10000f;
        Vector3Int minLoc = new Vector3Int(0, 0, 0);
        foreach (Vector3Int l in locs)
        {
            float dist = Distance(s, l);
            if (dist < minDist)
            {
                minDist = dist;
                minLoc = l;
            }
        }

        return minLoc;
    }

    public static Vector3Int FindNearestByWalkingDistance(Tilemap tm, Vector3Int s, List<Vector3Int> locs, bool diagonalAllowed)
    {
        int minSteps = 10000;
        Vector3Int minLoc = new Vector3Int(0, 0, 0);
        foreach (Vector3Int l in locs)
        {
            int steps = FindPath(tm, s, l, diagonalAllowed).Count;
            if (steps < minSteps)
            {
                minSteps = steps;
                minLoc = l;
            }
        }
        return minLoc;
    }

    public static float Distance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Sqrt(Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.y - a.y, 2));
    }

    //Using A*
    public static List<Vector3Int> FindPath(Tilemap tilemap, Vector3Int start, Vector3Int goal, bool diagonalAllowed)
    {
        if (start == goal)
        {
            return new List<Vector3Int>();
        }

        PQueue<Vector3Int> queue = new PQueue<Vector3Int>();
        queue.Enqueue(start, Distance(start, goal));

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        //actual current score
        Dictionary<Vector3Int, float> acs = new Dictionary<Vector3Int, float>();
        acs[start] = 0;
        //Estimated final score
        Dictionary<Vector3Int, float> efs = new Dictionary<Vector3Int, float>();
        efs[start] = Distance(start, goal);

        while (queue.GetCount() > 0)
        {
            Vector3Int current = queue.Dequeue();
            if (current == goal)
            {
                //Reconstruct path and return
                Vector3Int key = current;
                List<Vector3Int> l = new List<Vector3Int>();
                l.Add(key);
                while (cameFrom[key] != start)
                {
                    key = cameFrom[key];
                    l.Add(key);
                }
                return l;
            }

            List<Vector3Int> neighbors = FindNeighbors(current, tilemap, diagonalAllowed);
            foreach (Vector3Int n in neighbors)
            {
                float tentativeScore = acs[current] + Distance(current, n);
                if (!acs.ContainsKey(n) || tentativeScore < acs[n])
                {
                    cameFrom[n] = current;
                    acs[n] = tentativeScore;
                    efs[n] = tentativeScore + Distance(n, goal);
                    if (!queue.Contains(n))
                    {
                        queue.Enqueue(n, efs[n]);
                    }
                }
            }
        }

        return new List<Vector3Int>();
    }

    private static Vector3Int FirstStep()
    {
        return new Vector3Int(0, 0, 0);
    }

    private static List<Vector3Int> FindNeighbors(Vector3Int c, Tilemap tm, bool diag)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (!diag && x != 0 && y != 0)
                    continue;
                Vector3Int temp = c + new Vector3Int(x, y, 0);
                if (tm.HasTile(temp) && tm.GetTile(temp).name != "Forbidden")
                {
                    neighbors.Add(temp);
                }
            }
        }
        //Debug.Log(neighbors.Count + " neighbors found for coordinate " + c);
        return neighbors;
    }
}
