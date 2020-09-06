using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTTallGrass : NBTMeshGenerator
{
    List<int> triangles_tallgrass = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, CSBlockType type, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        AddDiagonalFace(vertices, uv, triangles_tallgrass, pos);
        AddAntiDiagonalFace(vertices, uv, triangles_tallgrass, pos);
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        trianglesList.Add(triangles_tallgrass);
        materialList.Add(Resources.Load<Material>("Materials/block/tallgrass"));
    }

    public override void ClearData()
    {
        triangles_tallgrass.Clear();
    }
}
