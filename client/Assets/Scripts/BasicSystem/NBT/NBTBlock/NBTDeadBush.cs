using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDeadBush : NBTMeshGenerator
{
    List<int> triangles = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, CSBlockType type, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        AddDiagonalFace(vertices, uv, triangles, pos);
        AddAntiDiagonalFace(vertices, uv, triangles, pos);
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        trianglesList.Add(triangles);
        materialList.Add(Resources.Load<Material>("Materials/block/deadbush"));
    }

    public override void ClearData()
    {
        triangles.Clear();
    }
}