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

    static Vector3Int vector3int_forward = new Vector3Int(0, 0, 1);
    static Vector3Int vector3int_back = new Vector3Int(0, 0, -1);
    static Mesh GetWallMesh(Vector3Int globalPosition)
    {
        Vector3Int right = globalPosition + Vector3Int.right;
        Vector3Int left = globalPosition + Vector3Int.left;
        Vector3Int forward = globalPosition + vector3int_forward;
        Vector3Int back = globalPosition + vector3int_back;
        
        byte directionMask = 0;
        if (IsCanLink(right))
        {
            directionMask += 1 << 0;
        }
        if (IsCanLink(left))
        {
            directionMask += 1 << 1;
        }
        if (IsCanLink(forward))
        {
            directionMask += 1 << 2;
        }
        if (IsCanLink(back))
        {
            directionMask += 1 << 3;
        }

        Dictionary<byte, string> dirPathDict = new Dictionary<byte, string>
        {
            {0, "wall" },
            {1, "wall_-x" },
            {2, "wall_+x" },
            {3, "wall_+x-x" },
            {4, "wall_-z" },
            {8, "wall_+z" },
            {12, "wall_+z-z" },
            {13, "wall_no_+x" },
            {14, "wall_no_+x" },
        };

        Debug.Log("mask=" + directionMask);
        Mesh mesh = LoadMesh("Meshes/blocks/wall/" + dirPathDict[directionMask]);

        return mesh;
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
