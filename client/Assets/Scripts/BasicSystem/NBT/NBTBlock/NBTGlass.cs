using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGlass : NBTMeshGenerator
{
    List<int> triangles = new List<int>();

    bool ShouldAddFace(NBTChunk chunk, int xInChunk, int worldY, int zInChunk)
    {
        byte type = chunk.GetBlockByte(xInChunk, worldY, zInChunk);
        return NBTGeneratorManager.IsTransparent(type) && type != 20;
    }

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        if (ShouldAddFace(chunk, pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(vertices, uv, triangles, pos);
        }
        if (ShouldAddFace(chunk, pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(vertices, uv, triangles, pos);
        }
        if (ShouldAddFace(chunk, pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(vertices, uv, triangles, pos);
        }
        if (ShouldAddFace(chunk, pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(vertices, uv, triangles, pos);
        }
        if (ShouldAddFace(chunk, pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(vertices, uv, triangles, pos);
        }
        if (ShouldAddFace(chunk, pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(vertices, uv, triangles, pos);
        }
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles.Count > 0)
        {
            trianglesList.Add(triangles);
            materialList.Add(Resources.Load<Material>("Materials/block/glass"));
        }
    }

    public override void ClearData()
    {
        triangles.Clear();
    }
}
