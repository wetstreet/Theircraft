using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Extensions
{
    public static Vector3Int ToVector3Int(this Vector3 v)
    {
        return Vector3Int.RoundToInt(v);
    }

    public static MeshData ToMeshData(this Mesh mesh)
    {
        return new MeshData(mesh);
    }
}
