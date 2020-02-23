using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class PlantMeshGenerator : IMeshGenerator
{
    static PlantMeshGenerator _instance;
    public static PlantMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlantMeshGenerator();
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

        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];

        AddDiagonalFace(vertices, uv, triangles, Vector3.zero, texCoords.front);
        AddAntiDiagonalFace(vertices, uv, triangles, Vector3.zero, texCoords.front);

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int pos, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];

        AddDiagonalFace(vertices, uv, triangles, pos, texCoords.front);
        AddAntiDiagonalFace(vertices, uv, triangles, pos, texCoords.front);
    }

    static void AddUV_BackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector2 texPos)
    {
        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        // bottom right
        uv.Add(new Vector2((texPos.x + 1 - compensation) / atlas_column, (texPos.y + compensation) / atlas_row));
        // top right
        uv.Add(new Vector2((texPos.x + 1 - compensation) / atlas_column, (texPos.y + 1 - compensation) / atlas_row));
        // top left
        uv.Add(new Vector2((texPos.x + compensation) / atlas_column, (texPos.y + 1 - compensation) / atlas_row));
        // bottom left
        uv.Add(new Vector2((texPos.x + compensation) / atlas_column, (texPos.y + compensation) / atlas_row));

        int verticesCount = vertices.Count;
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 1);
        triangles.Add(verticesCount - 2);
    }

    static void AddDiagonalFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(nearBottomRight + pos);
        AddUV(vertices, uv, triangles, texPos);
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(nearBottomRight + pos);
        AddUV_BackFace(vertices, uv, triangles, texPos);
    }

    static void AddAntiDiagonalFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farBottomRight + pos);
        AddUV(vertices, uv, triangles, texPos);
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farBottomRight + pos);
        AddUV_BackFace(vertices, uv, triangles, texPos);
    }
}
