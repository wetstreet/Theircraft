using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLargeFlowers : NBTBlock
{
    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool isCollidable { get { return false; } }

    List<int> triangles_grass_bottom = new List<int>();
    List<int> triangles_grass_top = new List<int>();
    List<int> triangles_fern_bottom = new List<int>();
    List<int> triangles_fern_top = new List<int>();

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        List<int> triangles = null;

        switch (blockData)
        {
            case 2:
                triangles = triangles_grass_bottom;
                break;
            case 3:
                triangles = triangles_fern_bottom;
                break;
            case 10:
                byte bottomType = 0;
                byte bottomData = 0;
                chunk.GetBlockData(pos.x, pos.y - 1, pos.z, ref bottomType, ref bottomData);
                switch (bottomData)
                {
                    case 2:
                        triangles = triangles_grass_top;
                        break;
                    case 3:
                        triangles = triangles_fern_top;
                        break;
                }
                break;
        }

        Vector3Int worldPos = pos + new Vector3Int(chunk.x, 0, chunk.z) * 16;
        if (triangles == null)
        {
            Debug.Log("pos=" + worldPos + ",data=" + blockData);
        }

        AddDiagonalFace(vertices, uv, triangles, pos);
        AddAntiDiagonalFace(vertices, uv, triangles, pos);
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles_grass_bottom.Count > 0)
        {
            trianglesList.Add(triangles_grass_bottom);
            materialList.Add(Resources.Load<Material>("Materials/block/double_plant_grass_bottom"));
        }
        if (triangles_fern_bottom.Count > 0)
        {
            trianglesList.Add(triangles_fern_bottom);
            materialList.Add(Resources.Load<Material>("Materials/block/double_plant_fern_bottom"));
        }
        if (triangles_grass_top.Count > 0)
        {
            trianglesList.Add(triangles_grass_top);
            materialList.Add(Resources.Load<Material>("Materials/block/double_plant_grass_top"));
        }
        if (triangles_fern_top.Count > 0)
        {
            trianglesList.Add(triangles_fern_top);
            materialList.Add(Resources.Load<Material>("Materials/block/double_plant_fern_top"));
        }
    }

    public override void ClearData()
    {
        triangles_grass_bottom.Clear();
        triangles_grass_top.Clear();
        triangles_fern_bottom.Clear();
        triangles_fern_top.Clear();
    }
}