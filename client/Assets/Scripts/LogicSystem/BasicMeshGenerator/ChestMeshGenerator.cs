using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMeshGenerator : IMeshGenerator
{
    static ChestMeshGenerator _instance;
    public static ChestMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ChestMeshGenerator();
            }
            return _instance;
        }
    }

    static Mesh GetMesh(bool isTop = false)
    {
        Mesh mesh = null;
        if (isTop)
        {
            mesh = Resources.Load<Mesh>("Meshes/blocks/slab/top");
        }
        else
        {
            mesh = Resources.Load<Mesh>("Meshes/blocks/slab/bottom");
        }
        return mesh;
    }

    override public Mesh GenerateSingleMesh(CSBlockType type)
    {
        Mesh mesh = GetMesh();

        Mesh singleMesh = new Mesh();
        singleMesh.name = "CubeMesh";

        List<Vector2> uv = new List<Vector2>();

        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        foreach (Vector2 singleUV in mesh.uv)
        {
            uv.Add(new Vector2((texPos.x + singleUV.x) / atlas_column, (texPos.y + singleUV.y) / atlas_row));
        }

        singleMesh.SetVertices(mesh.vertices);
        singleMesh.SetUVs(0, uv);
        singleMesh.SetTriangles(mesh.triangles, 0);
        singleMesh.SetNormals(mesh.normals);

        return singleMesh;
    }

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        Mesh mesh = GetMesh();
        int length = vertices.Count;
        foreach (Vector3 singleVertex in mesh.vertices)
        {
            Vector3 pos = singleVertex + posInChunk;
            vertices.Add(pos);
        }

        foreach (Vector2 singleUV in mesh.uv)
        {
            uv.Add(new Vector2((texPos.x + singleUV.x) / atlas_column, (texPos.y + singleUV.y) / atlas_row));
        }

        foreach (int index in mesh.triangles)
        {
            triangles.Add(index + length);
        }
    }
}
