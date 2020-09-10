using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStationaryWater : NBTMeshGenerator
{
    List<int> triangles_still = new List<int>();
    List<int> triangles_flow = new List<int>();

    static byte TYPE_WATER = 9;

    bool ShouldAddFace(byte type)
    {
        return NBTGeneratorManager.IsTransparent(type) && type != TYPE_WATER;
    }

    static byte height;

    static byte leftType;
    static byte leftData;
    static byte rightType;
    static byte rightData;
    static byte frontType;
    static byte frontData;
    static byte backType;
    static byte backData;
    static byte topType;
    static byte topData;
    static byte bottomType;
    static byte bottomData;

    public override void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv)
    {
        height = blockData;

        chunk.GetBlockData(pos.x, pos.y, pos.z - 1, ref frontType, ref frontData);
        chunk.GetBlockData(pos.x + 1, pos.y, pos.z, ref rightType, ref rightData);
        chunk.GetBlockData(pos.x - 1, pos.y, pos.z, ref leftType, ref leftData);
        chunk.GetBlockData(pos.x, pos.y, pos.z + 1, ref backType, ref backData);
        chunk.GetBlockData(pos.x, pos.y + 1, pos.z, ref topType, ref topData);
        chunk.GetBlockData(pos.x, pos.y - 1, pos.z, ref bottomType, ref bottomData);

        if (ShouldAddFace(frontType))
        {
            AddFrontFace(vertices, uv, triangles_still, pos);
        }
        if (ShouldAddFace(rightType))
        {
            AddRightFace(vertices, uv, triangles_still, pos);
        }
        if (ShouldAddFace(leftType))
        {
            AddLeftFace(vertices, uv, triangles_still, pos);
        }
        if (ShouldAddFace(backType))
        {
            AddBackFace(vertices, uv, triangles_still, pos);
        }
        if (ShouldAddFace(topType))
        {
            AddTopFace(vertices, uv, triangles_still, pos);
        }
        if (ShouldAddFace(bottomType))
        {
            AddBottomFace(vertices, uv, triangles_still, pos);
        }
    }

    public override void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList)
    {
        if (triangles_still.Count > 0)
        {
            trianglesList.Add(triangles_still);
            materialList.Add(Resources.Load<Material>("Materials/block/water_still"));
        }
    }

    public override void ClearData()
    {
        triangles_still.Clear();
    }

    static float step = 0.8f / 7;

    static Vector3 nearTopLeft
    {
        get
        {
            if (height > 0 && leftType == TYPE_WATER && leftData < height)
            {
                return new Vector3(-0.5f, 0.375f - (height - 1) * step, -0.5f);
            }
            if (height > 0 && frontType == TYPE_WATER && frontData < height)
            {
                return new Vector3(-0.5f, 0.375f - (height - 1) * step, -0.5f);
            }
            return new Vector3(-0.5f, 0.375f - height * step, -0.5f);
        }
    }

    static Vector3 nearTopRight
    {
        get
        {
            if (height > 0 && rightType == TYPE_WATER && rightData < height)
            {
                return new Vector3(0.5f, 0.375f - (height - 1) * step, -0.5f);
            }
            if (height > 0 && frontType == TYPE_WATER && frontData < height)
            {
                return new Vector3(0.5f, 0.375f - (height - 1) * step, -0.5f);
            }
            return new Vector3(0.5f, 0.375f - height * step, -0.5f);
        }
    }

    static Vector3 farTopLeft
    {
        get
        {
            if (height > 0 && leftType == TYPE_WATER && leftData < height)
            {
                return new Vector3(-0.5f, 0.375f - (height - 1) * step, 0.5f);
            }
            if (height > 0 && backType == TYPE_WATER && backData < height)
            {
                return new Vector3(-0.5f, 0.375f - (height - 1) * step, 0.5f);
            }
            return new Vector3(-0.5f, 0.375f - height * step, 0.5f);
        }
    }

    static Vector3 farTopRight
    {
        get
        {
            if (height > 0 && rightType == TYPE_WATER && rightData < height)
            {
                return new Vector3(0.5f, 0.375f - (height - 1) * step, 0.5f);
            }
            if (height > 0 && backType == TYPE_WATER && backData < height)
            {
                return new Vector3(0.5f, 0.375f - (height - 1) * step, 0.5f);
            }
            return new Vector3(0.5f, 0.375f - height * step, 0.5f);
        }
    }

    protected static void AddFrontFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((nearBottomLeft) + pos);
        vertices.Add((nearTopLeft) + pos);
        vertices.Add((nearTopRight) + pos);
        vertices.Add((nearBottomRight) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddRightFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((nearBottomRight) + pos);
        vertices.Add((nearTopRight) + pos);
        vertices.Add((farTopRight) + pos);
        vertices.Add((farBottomRight) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddLeftFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((farBottomLeft) + pos);
        vertices.Add((farTopLeft) + pos);
        vertices.Add((nearTopLeft) + pos);
        vertices.Add((nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddBackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((farBottomRight) + pos);
        vertices.Add((farTopRight) + pos);
        vertices.Add((farTopLeft) + pos);
        vertices.Add((farBottomLeft) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddTopFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((farTopRight) + pos);
        vertices.Add((nearTopRight) + pos);
        vertices.Add((nearTopLeft) + pos);
        vertices.Add((farTopLeft) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddBottomFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((nearBottomRight) + pos);
        vertices.Add((farBottomRight) + pos);
        vertices.Add((farBottomLeft) + pos);
        vertices.Add((nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles);
    }
}
