using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class BlockMeshGenerator : IMeshGenerator
{
    static BlockMeshGenerator _instance;
    public static BlockMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BlockMeshGenerator();
            }
            return _instance;
        }
    }

    override public Mesh GenerateSingleMesh(CSBlockType type)
    {
        Mesh mesh = new Mesh();
        mesh.name = "CubeMesh";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        ChunkMeshGenerator.TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        AddFrontFace(vertices, uv, triangles, Vector3.zero, texCoords.front);
        AddRightFace(vertices, uv, triangles, Vector3.zero, texCoords.right);
        AddLeftFace(vertices, uv, triangles, Vector3.zero, texCoords.left);
        AddBackFace(vertices, uv, triangles, Vector3.zero, texCoords.back);
        AddTopFace(vertices, uv, triangles, Vector3.zero, texCoords.top);
        AddBottomFace(vertices, uv, triangles, Vector3.zero, texCoords.bottom);

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int pos, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        ChunkMeshGenerator.TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y, globalPos.z - 1))
        {
            AddFrontFace(vertices, uv, triangles, pos, texCoords.front);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x + 1, globalPos.y, globalPos.z))
        {
            AddRightFace(vertices, uv, triangles, pos, texCoords.right);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x - 1, globalPos.y, globalPos.z))
        {
            AddLeftFace(vertices, uv, triangles, pos, texCoords.left);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y, globalPos.z + 1))
        {
            AddBackFace(vertices, uv, triangles, pos, texCoords.back);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y + 1, globalPos.z))
        {
            AddTopFace(vertices, uv, triangles, pos, texCoords.top);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y - 1, globalPos.z))
        {
            AddBottomFace(vertices, uv, triangles, pos, texCoords.bottom);
        }
    }

    protected static void AddFrontFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(nearBottomRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddRightFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearBottomRight + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farBottomRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddLeftFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(nearBottomLeft + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddBackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(farBottomLeft + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddTopFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(nearTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddBottomFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearBottomRight + pos);
        vertices.Add(farBottomRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }
}
