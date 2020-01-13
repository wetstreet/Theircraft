using protocol.cs_theircraft;
using System;
using System.Collections.Generic;

[Serializable]
public struct Vector3Int
{
    public int x;
    public int y;
    public int z;

    public static Vector3Int ParseFromCSVector3Int(CSVector3Int v)
    {
        return new Vector3Int { x = v.x, y = v.y, z = v.z };
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
