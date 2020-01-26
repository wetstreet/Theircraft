using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class WallMeshGenerator : IMeshGenerator
{
    static WallMeshGenerator _instance;
    public static WallMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new WallMeshGenerator();
            }
            return _instance;
        }
    }

    override public Mesh GenerateSingleMesh(CSBlockType type)
    {
        Mesh wallMesh = Resources.Load<Mesh>("Meshes/blocks/wall/wall");

        Mesh mesh = new Mesh();
        mesh.name = "CubeMesh";

        List<Vector2> uv = new List<Vector2>();

        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        foreach (Vector2 singleUV in wallMesh.uv)
        {
            uv.Add(new Vector2((texPos.x + singleUV.x) / atlas_column, (texPos.y + singleUV.y) / atlas_row));
        }

        mesh.vertices = wallMesh.vertices;
        mesh.uv = uv.ToArray();
        mesh.triangles = wallMesh.triangles;

        return mesh;
    }

    static bool IsCanLink(Vector3Int pos)
    {
        CSBlockType type = ChunkManager.GetBlockType(pos);
        if (type == CSBlockType.BrickWall)
        {
            return true;
        }
        else
        {
            if (ChunkManager.HasOpaqueBlock(pos))
            {
                return true;
            }
        }
        return false;
    }

    static string[] wallMeshPath = new string[16]
    {
        "wall",
        "wall_-x",
        "wall_+x",
        "wall_+x-x",
        "wall_-z",
        "wall_-x-z",
        "wall_+x-z",
        "wall_no_-z",
        "wall_+z",
        "wall_-x+z",
        "wall_+x+z",
        "wall_no_+z",
        "wall_+z-z",
        "wall_no_+x",
        "wall_no_-x",
        "wall_full"
    };

    static Vector3Int vector3int_forward = new Vector3Int(0, 0, 1);
    static Vector3Int vector3int_back = new Vector3Int(0, 0, -1);
    static Mesh GetWallMesh(Vector3Int globalPosition)
    {
        byte directionMask = 0;
        if (IsCanLink(globalPosition + Vector3Int.right))
        {
            directionMask += 1 << 0;
        }
        if (IsCanLink(globalPosition + Vector3Int.left))
        {
            directionMask += 1 << 1;
        }
        if (IsCanLink(globalPosition + vector3int_forward))
        {
            directionMask += 1 << 2;
        }
        if (IsCanLink(globalPosition + vector3int_back))
        {
            directionMask += 1 << 3;
        }

        return LoadMesh("Meshes/blocks/wall/" + wallMeshPath[directionMask]);
    }

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        Vector2Int texPos = texCoords.front;
        texPos.y = (atlas_row - 1) - texPos.y;

        Mesh wallMesh = GetWallMesh(globalPos);
        int length = vertices.Count;
        foreach (Vector3 singleVertex in wallMesh.vertices)
        {
            Vector3 pos = singleVertex + posInChunk;
            vertices.Add(pos);
        }

        foreach (Vector2 singleUV in wallMesh.uv)
        {
            uv.Add(new Vector2((texPos.x + singleUV.x) / atlas_column, (texPos.y + singleUV.y) / atlas_row));
        }

        foreach (int index in wallMesh.triangles)
        {
            triangles.Add(index + length);
        }
    }
}
