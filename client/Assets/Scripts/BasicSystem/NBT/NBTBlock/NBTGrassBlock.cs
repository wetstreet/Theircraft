using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGrassBlock : NBTMeshGenerator
{
    List<int> triangles_grass_top = new List<int>();
    List<int> triangles_grass_side = new List<int>();
    List<int> triangles_grass_bot = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, CSBlockType type, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(vertices, uv, triangles_grass_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(vertices, uv, triangles_grass_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(vertices, uv, triangles_grass_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(vertices, uv, triangles_grass_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(vertices, uv, triangles_grass_top, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(vertices, uv, triangles_grass_bot, pos);
        }
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        trianglesList.Add(triangles_grass_top);
        materialList.Add(Resources.Load<Material>("Materials/block/grass_top"));
        trianglesList.Add(triangles_grass_side);
        materialList.Add(Resources.Load<Material>("Materials/block/grass_side"));
        trianglesList.Add(triangles_grass_bot);
        materialList.Add(Resources.Load<Material>("Materials/block/grass_bottom"));
    }

    public override void ClearData()
    {
        triangles_grass_top.Clear();
        triangles_grass_side.Clear();
        triangles_grass_bot.Clear();
    }
}
