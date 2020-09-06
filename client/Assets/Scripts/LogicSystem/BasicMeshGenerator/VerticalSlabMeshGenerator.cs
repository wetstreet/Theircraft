using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalSlabMeshGenerator : IMeshGenerator
{
    static VerticalSlabMeshGenerator _instance;
    public static VerticalSlabMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new VerticalSlabMeshGenerator();
            }
            return _instance;
        }
    }
    
    static Mesh GetMesh()
    {
        Mesh mesh = null;
        mesh = Resources.Load<Mesh>("Meshes/blocks/vertical_slab/vslab_-x");
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
        Vector2 dir = (new Vector2(playerPos.x, playerPos.z) - new Vector2(blockPos.x, blockPos.z)).normalized;
        if (Mathf.Abs(dir.x) - Mathf.Abs(dir.y) > 0)
        {
            if (blockPos.z - hitPos.z > 0)
            {
                return CSBlockOrientation.PositiveY_PositiveZ;
            }
            else
            {
                return CSBlockOrientation.PositiveY_NegativeZ;
            }
        }
        else
        {
            if (blockPos.x - hitPos.x > 0)
            {
                return CSBlockOrientation.PositiveY_PositiveX;
            }
            else
            {
                return CSBlockOrientation.PositiveY_NegativeX;
            }
        }
    }


    static Mesh GetMesh(CSBlockOrientation orientation = CSBlockOrientation.PositiveY_NegativeX)
    {
        Mesh mesh = null;
        switch (orientation)
        {
            case CSBlockOrientation.PositiveY_PositiveX:
            case CSBlockOrientation.NegativeY_PositiveX:
                mesh = Resources.Load<Mesh>("Meshes/blocks/vertical_slab/vslab_+x");
                break;
            case CSBlockOrientation.PositiveY_NegativeX:
            case CSBlockOrientation.NegativeY_NegativeX:
                mesh = Resources.Load<Mesh>("Meshes/blocks/vertical_slab/vslab_-x");
                break;
            case CSBlockOrientation.PositiveY_PositiveZ:
            case CSBlockOrientation.NegativeY_PositiveZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/vertical_slab/vslab_+y");
                break;
            case CSBlockOrientation.PositiveY_NegativeZ:
            case CSBlockOrientation.NegativeY_NegativeZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/vertical_slab/vslab_-y");
                break;
        }

        return mesh;
    }


    public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Color> colors, List<Vector2> uv, List<int> triangles)
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

        foreach (int index in mesh.triangles)
        {
            triangles.Add(index + length);
        }

        foreach (Vector3 normal in mesh.normals)
        {
            colors.Add(Color.white);
        }
    }
}
