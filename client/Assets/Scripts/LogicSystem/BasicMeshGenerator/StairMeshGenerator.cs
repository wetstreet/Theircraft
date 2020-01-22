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
        Mesh stairMesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair");

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

        return mesh;
    }

    static Mesh GetMeshByOrientation(CSBlockOrientation orientation)
    {
        Mesh mesh = null;
        switch (orientation)
        {
            case CSBlockOrientation.PositiveY_NegativeX:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x");
                break;
            case CSBlockOrientation.PositiveY_NegativeZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-z");
                break;
            case CSBlockOrientation.PositiveY_PositiveX:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x");
                break;
            case CSBlockOrientation.PositiveY_PositiveZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+z");
                break;
            case CSBlockOrientation.NegativeY_NegativeX:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-y-x");
                break;
            case CSBlockOrientation.NegativeY_NegativeZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-y-z");
                break;
            case CSBlockOrientation.NegativeY_PositiveX:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-y+x");
                break;
            case CSBlockOrientation.NegativeY_PositiveZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-y+z");
                break;
            default:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-z");
                break;
        }
        return mesh;
    }

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        CSBlockOrientation orient = ChunkManager.GetBlockOrientation(globalPos);

        Mesh stairMesh = GetMeshByOrientation(orient);
        int length = vertices.Count;
        foreach (Vector3 singleVertex in stairMesh.vertices)
        {
            Vector3 pos = singleVertex + posInChunk;
            vertices.Add(pos);
        }

        foreach (Vector2 singleUV in stairMesh.uv)
        {
            uv.Add(new Vector2((texPos.x + singleUV.x) / atlas_column, (texPos.y + singleUV.y) / atlas_row));
        }

        foreach (int index in stairMesh.triangles)
        {
            triangles.Add(index + length);
        }
    }
}
