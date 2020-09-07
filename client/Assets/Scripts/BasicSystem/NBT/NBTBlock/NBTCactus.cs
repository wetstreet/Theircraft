using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCactus : NBTMeshGenerator
{
    List<int> triangles_top = new List<int>();
    List<int> triangles_side = new List<int>();
    List<int> triangles_bot = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        Mesh top = Resources.Load<Mesh>("Meshes/blocks/cactus/top");
        CopyFromMesh(top, pos, vertices, uv, triangles_top);
        Mesh side = Resources.Load<Mesh>("Meshes/blocks/cactus/side");
        CopyFromMesh(side, pos, vertices, uv, triangles_side);
        Mesh bottom = Resources.Load<Mesh>("Meshes/blocks/cactus/bottom");
        CopyFromMesh(bottom, pos, vertices, uv, triangles_bot);
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles_top.Count > 0)
        {
            trianglesList.Add(triangles_top);
            materialList.Add(Resources.Load<Material>("Materials/block/cactus_top"));
        }
        if (triangles_side.Count > 0)
        {
            trianglesList.Add(triangles_side);
            materialList.Add(Resources.Load<Material>("Materials/block/cactus_side"));
        }
        if (triangles_bot.Count > 0)
        {
            trianglesList.Add(triangles_bot);
            materialList.Add(Resources.Load<Material>("Materials/block/cactus_bottom"));
        }
    }

    public override void ClearData()
    {
        triangles_top.Clear();
        triangles_side.Clear();
        triangles_bot.Clear();
    }
}
