using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class StairMeshGenerator : IMeshGenerator
{
    static StairMeshGenerator _instance;
    public static StairMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new StairMeshGenerator();
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

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        ChunkMeshGenerator.TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];

        AddFrontFace(vertices, uv, triangles, posInChunk, texCoords.front);
        AddTopFace(vertices, uv, triangles, posInChunk, texCoords.top);

        if (!ChunkManager.HasOpaqueBlock(globalPos.x + 1, globalPos.y, globalPos.z))
        {
            AddRightFace(vertices, uv, triangles, posInChunk, texCoords.right);
        }
        if (!ChunkManager.HasOpaqueBlock(globalPos.x - 1, globalPos.y, globalPos.z))
        {
            AddLeftFace(vertices, uv, triangles, posInChunk, texCoords.left);
        }
        if (!ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y, globalPos.z + 1))
        {
            AddBackFace(vertices, uv, triangles, posInChunk, texCoords.back);
        }
        if (!ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y - 1, globalPos.z))
        {
            AddBottomFace(vertices, uv, triangles, posInChunk, texCoords.bottom);
        }
    }
    
    static Vector3 nearMidLeft = new Vector3(-0.5f, 0, -0.5f);
    static Vector3 nearMidRight = new Vector3(0.5f, 0, -0.5f);

    static Vector3 midMidLeft = new Vector3(-0.5f, 0, 0);
    static Vector3 midMidRight = new Vector3(0.5f, 0, 0);

    static Vector3 midTopLeft = new Vector3(-0.5f, 0.5f, 0);
    static Vector3 midTopRight = new Vector3(0.5f, 0.5f, 0);
    
    static void AddBottomFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearBottomRight + pos);
        vertices.Add(farBottomRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddBackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(farBottomLeft + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddLeftFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(midTopLeft + pos);
        vertices.Add(midMidLeft + pos);
        vertices.Add(nearMidLeft + pos);
        vertices.Add(nearBottomLeft + pos);

        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        // bottom left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, texPos.y / atlas_row + compensation_y));
        // top left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // top mid
        uv.Add(new Vector2((texPos.x + 0.5f) / atlas_column, (texPos.y + 1) / atlas_row - compensation_y));
        // mid mid
        uv.Add(new Vector2((texPos.x + 0.5f) / atlas_column, (texPos.y + 0.5f) / atlas_row));
        // mid right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 0.5f) / atlas_row));
        // bottom right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, texPos.y / atlas_row + compensation_y));

        int verticesCount = vertices.Count;

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 5);
        triangles.Add(verticesCount - 4);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);
    }

    static void AddRightFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(midTopRight + pos);
        vertices.Add(midMidRight + pos);
        vertices.Add(nearMidRight + pos);
        vertices.Add(nearBottomRight + pos);

        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        // bottom left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, texPos.y / atlas_row + compensation_y));
        // top left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // top mid
        uv.Add(new Vector2((texPos.x + 0.5f) / atlas_column, (texPos.y + 1) / atlas_row - compensation_y));
        // mid mid
        uv.Add(new Vector2((texPos.x + 0.5f) / atlas_column, (texPos.y + 0.5f) / atlas_row));
        // mid right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 0.5f) / atlas_row));
        // bottom right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, texPos.y / atlas_row + compensation_y));

        int verticesCount = vertices.Count;

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 5);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 4);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 3);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 1);
        triangles.Add(verticesCount - 2);
    }

    static void AddUV_SplitInHalf(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector2 texPos)
    {
        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        // bottom left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, texPos.y / atlas_row + compensation_y));
        // mid left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 0.5f) / atlas_row));
        // mid right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 0.5f) / atlas_row));
        // bottom right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, texPos.y / atlas_row + compensation_y));

        // mid left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 0.5f) / atlas_row));
        // top left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // top right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // mid right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 0.5f) / atlas_row));

        int verticesCount = vertices.Count;

        triangles.Add(verticesCount - 8);
        triangles.Add(verticesCount - 7);
        triangles.Add(verticesCount - 6);

        triangles.Add(verticesCount - 8);
        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 5);

        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);

        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);
    }

    static void AddFrontFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearMidLeft + pos);
        vertices.Add(nearMidRight + pos);
        vertices.Add(nearBottomRight + pos);

        vertices.Add(midMidLeft + pos);
        vertices.Add(midTopLeft + pos);
        vertices.Add(midTopRight + pos);
        vertices.Add(midMidRight + pos);

        AddUV_SplitInHalf(vertices, uv, triangles, texPos);
    }

    static void AddTopFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearMidLeft + pos);
        vertices.Add(midMidLeft + pos);
        vertices.Add(midMidRight + pos);
        vertices.Add(nearMidRight + pos);

        vertices.Add(midTopLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(midTopRight + pos);

        AddUV_SplitInHalf(vertices, uv, triangles, texPos);
    }
}
