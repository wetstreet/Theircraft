using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMeshGenerator : IMeshGenerator
{
    static LogMeshGenerator _instance;
    public static LogMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LogMeshGenerator();
            }
            return _instance;
        }
    }

    public static CSBlockOrientation GetOrientation(Vector3 normal)
    {
        Debug.Log(normal);
        if (normal == Vector3.up || normal == Vector3.down)
        {
            return CSBlockOrientation.Y;
        }
        else if (normal == Vector3.left || normal == Vector3.right)
        {
            return CSBlockOrientation.Z;
        }
        else if (normal == Vector3.forward || normal == Vector3.back)
        {
            return CSBlockOrientation.X;
        }
        return CSBlockOrientation.Y;
    }

    protected static Mesh LoadMesh(CSBlockOrientation orient = CSBlockOrientation.Y)
    {
        Mesh mesh;
        switch (orient)
        {
            case CSBlockOrientation.X:
                mesh = Resources.Load<Mesh>("Meshes/blocks/log/log_x");
                break;
            case CSBlockOrientation.Z:
                mesh = Resources.Load<Mesh>("Meshes/blocks/log/log_z");
                break;
            case CSBlockOrientation.Y:
            default:
                mesh = Resources.Load<Mesh>("Meshes/blocks/log/log_y");
                break;
        }
        return mesh;
    }

    override public Mesh GenerateSingleMesh(CSBlockType type)
    {
        Mesh stairMesh = LoadMesh();

        Mesh mesh = new Mesh();
        mesh.name = "CubeMesh";

        List<Vector2> uv = new List<Vector2>();

        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        foreach (Vector2 singleUV in stairMesh.uv)
        {
            uv.Add(new Vector2((texPos.x + singleUV.x) / atlas_column, (texPos.y + singleUV.y) / atlas_row));
        }

        mesh.vertices = stairMesh.vertices;
        mesh.uv = uv.ToArray();
        mesh.triangles = stairMesh.triangles;
        mesh.normals = stairMesh.normals;

        return mesh;
    }

    public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<Vector3> normals, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        CSBlockOrientation orient = ChunkManager.GetBlockOrientation(globalPos);

        Mesh mesh = LoadMesh(orient);
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

        foreach (Vector3 normal in mesh.normals)
        {
            normals.Add(normal);
        }

        foreach (int index in mesh.triangles)
        {
            triangles.Add(index + length);
        }
    }
}
