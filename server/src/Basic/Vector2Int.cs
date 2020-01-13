using protocol.cs_theircraft;
using System;
using System.Collections.Generic;

[Serializable]
public struct Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int _x,int _y)
    {
        x = _x;
        y = _y;
    }

    public Vector2Int(CSVector2Int v)
    {
        x = v.x;
        y = v.y;
    }

    public static List<Vector2Int> CSVector2IntList_To_Vector2IntList(List<CSVector2Int> csvList)
    {
        List<Vector2Int> vList = new List<Vector2Int>();
        foreach (CSVector2Int csv in csvList)
        {
            vList.Add(new Vector2Int(csv));
        }
        return vList;
    }

    public static List<CSVector2Int> Vector2IntList_To_CSVector2IntList(List<Vector2Int> vList)
    {
        List<CSVector2Int> csvList = new List<CSVector2Int>();
        foreach (Vector2Int v in vList)
        {
            csvList.Add(v.ToCSVector2Int());
        }
        return csvList;
    }

    public CSVector2Int ToCSVector2Int()
    {
        return new CSVector2Int
        {
            x = x,
            y = y
        };
    }
}