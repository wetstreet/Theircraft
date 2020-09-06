using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLog : NBTMeshGenerator
{
    List<int> triangles_top = new List<int>();
    List<int> triangles_side = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, CSBlockType type, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(vertices, uv, triangles_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(vertices, uv, triangles_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(vertices, uv, triangles_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(vertices, uv, triangles_side, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(vertices, uv, triangles_top, pos);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(vertices, uv, triangles_top, pos);
        }
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles_top.Count > 0)
        {
            trianglesList.Add(triangles_top);
            materialList.Add(Resources.Load<Material>("Materials/block/log_oak_top"));
        }
        if (triangles_side.Count > 0)
        {
            trianglesList.Add(triangles_side);
            materialList.Add(Resources.Load<Material>("Materials/block/log_oak_side"));
        }
    }

    public override void ClearData()
    {
        triangles_top.Clear();
        triangles_side.Clear();
    }
}
