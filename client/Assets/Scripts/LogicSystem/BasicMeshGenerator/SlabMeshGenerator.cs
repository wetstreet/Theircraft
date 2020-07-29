using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlabType
{
    Top,
    Bottom,
    Full
}

public class SlabMeshGenerator : IMeshGenerator
{
    static SlabMeshGenerator _instance;
    public static SlabMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SlabMeshGenerator();
            }
            return _instance;
        }
    }
    
    static Mesh GetMesh(CSBlockOrientation orientation = CSBlockOrientation.NegativeY_PositiveX)
    {
        Mesh mesh = null;
        switch (orientation)
        {
            case CSBlockOrientation.PositiveY_NegativeX:
            case CSBlockOrientation.PositiveY_NegativeZ:
            case CSBlockOrientation.PositiveY_PositiveX:
            case CSBlockOrientation.PositiveY_PositiveZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/slab/top");
                break;
            case CSBlockOrientation.NegativeY_NegativeX:
            case CSBlockOrientation.NegativeY_NegativeZ:
            case CSBlockOrientation.NegativeY_PositiveX:
            case CSBlockOrientation.NegativeY_PositiveZ:
            default:
                mesh = Resources.Load<Mesh>("Meshes/blocks/slab/bottom");
                break;
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

    public static CSBlockOrientation GetOrientation(Vector3 playerPos, Vector3 blockPos, Vector3 hitPos)
    {
        if (hitPos.y - blockPos.y > 0)
        {
            return CSBlockOrientation.PositiveY_PositiveX;
        }
        else
        {
            return CSBlockOrientation.NegativeY_PositiveX;
        }
    }

    public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<Vector3> normals, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        CSBlockOrientation orient = ChunkManager.GetBlockOrientation(globalPos);
        Mesh mesh = GetMesh(orient);
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
