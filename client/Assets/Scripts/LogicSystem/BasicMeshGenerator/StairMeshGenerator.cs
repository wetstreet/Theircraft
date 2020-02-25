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
        Mesh stairMesh = LoadMesh("stair_+y-z");

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

    static Vector3Int forward = new Vector3Int(0, 0, 1);
    static Vector3Int back = new Vector3Int(0, 0, -1);
    static Mesh GetStairMesh(CSBlockOrientation orientation, Vector3Int globalPosition)
    {
        Mesh mesh = null;
        switch (orientation)
        {
            case CSBlockOrientation.PositiveY_NegativeX:
                mesh = GetMeshPositiveYNegativeX(globalPosition);
                break;
            case CSBlockOrientation.PositiveY_NegativeZ:
                mesh = GetMeshPositiveYNegativeZ(globalPosition);
                break;
            case CSBlockOrientation.PositiveY_PositiveX:
                mesh = GetMeshPositiveYPositiveX(globalPosition);
                break;
            case CSBlockOrientation.PositiveY_PositiveZ:
                mesh = GetMeshPositiveYPositiveZ(globalPosition);
                break;
            case CSBlockOrientation.NegativeY_NegativeX:
                mesh = GetMeshNegativeYNegativeX(globalPosition);
                break;
            case CSBlockOrientation.NegativeY_NegativeZ:
                mesh = GetMeshNegativeYNegativeZ(globalPosition);
                break;
            case CSBlockOrientation.NegativeY_PositiveX:
                mesh = GetMeshNegativeYPositiveX(globalPosition);
                break;
            case CSBlockOrientation.NegativeY_PositiveZ:
                mesh = GetMeshNegativeYPositiveZ(globalPosition);
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

        Mesh stairMesh = GetStairMesh(orient, globalPos);
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

    static Mesh GetMeshPositiveYPositiveX(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("stair_+y+x");
        if (ChunkManager.IsStairs(globalPosition + Vector3Int.left))
        {
            CSBlockOrientation leftOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.left);
            if (leftOrientation == CSBlockOrientation.PositiveY_PositiveZ)
            {
                mesh = LoadMesh("stair_joint_top_right");
            }
            else if (leftOrientation == CSBlockOrientation.PositiveY_NegativeZ)
            {
                mesh = LoadMesh("stair_joint_bottom_right");
            }
        }
        else if (ChunkManager.IsStairs(globalPosition + Vector3Int.right))
        {
            CSBlockOrientation rightOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.right);
            if (rightOrientation == CSBlockOrientation.PositiveY_PositiveZ)
            {
                mesh = LoadMesh("stair_joint_no_bottom_left");
            }
            else if (rightOrientation == CSBlockOrientation.PositiveY_NegativeZ)
            {
                mesh = LoadMesh("stair_joint_no_top_left");
            }
        }
        return mesh;
    }

    static Mesh GetMeshPositiveYNegativeX(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("stair_+y-x");
        if (ChunkManager.IsStairs(globalPosition + Vector3Int.right))
        {
            CSBlockOrientation rightOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.right);
            if (rightOrientation == CSBlockOrientation.PositiveY_PositiveZ)
            {
                mesh = LoadMesh("stair_joint_top_left");
            }
            else if (rightOrientation == CSBlockOrientation.PositiveY_NegativeZ)
            {
                mesh = LoadMesh("stair_joint_bottom_left");
            }
        }
        else if (ChunkManager.IsStairs(globalPosition + Vector3Int.left))
        {
            CSBlockOrientation leftOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.left);
            if (leftOrientation == CSBlockOrientation.PositiveY_PositiveZ)
            {
                mesh = LoadMesh("stair_joint_no_bottom_right");
            }
            else if (leftOrientation == CSBlockOrientation.PositiveY_NegativeZ)
            {
                mesh = LoadMesh("stair_joint_no_top_right");
            }
        }
        return mesh;
    }

    static Mesh GetMeshPositiveYPositiveZ(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("stair_+y+z");
        if (ChunkManager.IsStairs(globalPosition + forward))
        {
            CSBlockOrientation forwardOrientation = ChunkManager.GetBlockOrientation(globalPosition + forward);
            if (forwardOrientation == CSBlockOrientation.PositiveY_PositiveX)
            {
                mesh = LoadMesh("stair_joint_no_bottom_left");
            }
            else if (forwardOrientation == CSBlockOrientation.PositiveY_NegativeX)
            {
                mesh = LoadMesh("stair_joint_no_bottom_right");
            }
        }
        else if (ChunkManager.IsStairs(globalPosition + back))
        {
            CSBlockOrientation backOrientation = ChunkManager.GetBlockOrientation(globalPosition + back);
            if (backOrientation == CSBlockOrientation.PositiveY_PositiveX)
            {
                mesh = LoadMesh("stair_joint_top_right");
            }
            else if (backOrientation == CSBlockOrientation.PositiveY_NegativeX)
            {
                mesh = LoadMesh("stair_joint_top_left");
            }
        }
        return mesh;
    }

    static Mesh GetMeshPositiveYNegativeZ(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("stair_+y-z");
        if (ChunkManager.IsStairs(globalPosition + forward))
        {
            CSBlockOrientation forwardOrientation = ChunkManager.GetBlockOrientation(globalPosition + forward);
            if (forwardOrientation == CSBlockOrientation.PositiveY_PositiveX)
            {
                mesh = LoadMesh("stair_joint_bottom_right");
            }
            else if (forwardOrientation == CSBlockOrientation.PositiveY_NegativeX)
            {
                mesh = LoadMesh("stair_joint_bottom_left");
            }
        }
        else if (ChunkManager.IsStairs(globalPosition + back))
        {
            CSBlockOrientation backOrientation = ChunkManager.GetBlockOrientation(globalPosition + back);
            if (backOrientation == CSBlockOrientation.PositiveY_PositiveX)
            {
                mesh = LoadMesh("stair_joint_no_top_left");
            }
            else if (backOrientation == CSBlockOrientation.PositiveY_NegativeX)
            {
                mesh = LoadMesh("stair_joint_no_top_right");
            }
        }
        return mesh;
    }

    static Mesh GetMeshNegativeYPositiveX(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("stair_-y+x");
        if (ChunkManager.IsStairs(globalPosition + Vector3Int.left))
        {
            CSBlockOrientation leftOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.left);
            if (leftOrientation == CSBlockOrientation.NegativeY_PositiveZ)
            {
                mesh = LoadMesh("stair_joint_-y_top_right");
            }
            else if (leftOrientation == CSBlockOrientation.NegativeY_NegativeZ)
            {
                mesh = LoadMesh("stair_joint_-y_bottom_right");
            }
        }
        else if (ChunkManager.IsStairs(globalPosition + Vector3Int.right))
        {
            CSBlockOrientation rightOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.right);
            if (rightOrientation == CSBlockOrientation.NegativeY_PositiveZ)
            {
                mesh = LoadMesh("stair_joint_-y_no_bottom_left");
            }
            else if (rightOrientation == CSBlockOrientation.NegativeY_NegativeZ)
            {
                mesh = LoadMesh("stair_joint_-y_no_top_left");
            }
        }
        return mesh;
    }

    static Mesh GetMeshNegativeYNegativeX(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("stair_-y-x");
        if (ChunkManager.IsStairs(globalPosition + Vector3Int.right))
        {
            CSBlockOrientation rightOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.right);
            if (rightOrientation == CSBlockOrientation.NegativeY_PositiveZ)
            {
                mesh = LoadMesh("stair_joint_-y_top_left");
            }
            else if (rightOrientation == CSBlockOrientation.NegativeY_NegativeZ)
            {
                mesh = LoadMesh("stair_joint_-y_bottom_left");
            }
        }
        else if (ChunkManager.IsStairs(globalPosition + Vector3Int.left))
        {
            CSBlockOrientation leftOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.left);
            if (leftOrientation == CSBlockOrientation.NegativeY_PositiveZ)
            {
                mesh = LoadMesh("stair_joint_-y_no_bottom_right");
            }
            else if (leftOrientation == CSBlockOrientation.NegativeY_NegativeZ)
            {
                mesh = LoadMesh("stair_joint_-y_no_top_right");
            }
        }
        return mesh;
    }

    static Mesh GetMeshNegativeYPositiveZ(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("stair_-y+z");
        if (ChunkManager.IsStairs(globalPosition + forward))
        {
            CSBlockOrientation forwardOrientation = ChunkManager.GetBlockOrientation(globalPosition + forward);
            if (forwardOrientation == CSBlockOrientation.NegativeY_PositiveX)
            {
                mesh = LoadMesh("stair_joint_-y_no_bottom_left");
            }
            else if (forwardOrientation == CSBlockOrientation.NegativeY_NegativeX)
            {
                mesh = LoadMesh("stair_joint_-y_no_bottom_right");
            }
        }
        else if (ChunkManager.IsStairs(globalPosition + back))
        {
            CSBlockOrientation backOrientation = ChunkManager.GetBlockOrientation(globalPosition + back);
            if (backOrientation == CSBlockOrientation.NegativeY_PositiveX)
            {
                mesh = LoadMesh("stair_joint_-y_top_right");
            }
            else if (backOrientation == CSBlockOrientation.NegativeY_NegativeX)
            {
                mesh = LoadMesh("stair_joint_-y_top_left");
            }
        }
        return mesh;
    }

    static Mesh GetMeshNegativeYNegativeZ(Vector3Int globalPosition)
    {
        Mesh mesh = LoadMesh("stair_-y-z");
        if (ChunkManager.IsStairs(globalPosition + forward))
        {
            CSBlockOrientation forwardOrientation = ChunkManager.GetBlockOrientation(globalPosition + forward);
            if (forwardOrientation == CSBlockOrientation.NegativeY_PositiveX)
            {
                mesh = LoadMesh("stair_joint_-y_bottom_right");
            }
            else if (forwardOrientation == CSBlockOrientation.NegativeY_NegativeX)
            {
                mesh = LoadMesh("stair_joint_-y_bottom_left");
            }
        }
        else if (ChunkManager.IsStairs(globalPosition + back))
        {
            CSBlockOrientation backOrientation = ChunkManager.GetBlockOrientation(globalPosition + back);
            if (backOrientation == CSBlockOrientation.NegativeY_PositiveX)
            {
                mesh = LoadMesh("stair_joint_-y_no_top_left");
            }
            else if (backOrientation == CSBlockOrientation.NegativeY_NegativeX)
            {
                mesh = LoadMesh("stair_joint_-y_no_top_right");
            }
        }
        return mesh;
    }
    
    static Dictionary<string, Mesh> meshDict;
    protected new static Mesh LoadMesh(string path)
    {
        if (meshDict == null)
        {
            meshDict = new Dictionary<string, Mesh>();
            Mesh[] meshes = Resources.LoadAll<Mesh>("Meshes/blocks/stair/stairs");
            foreach (Mesh mesh in meshes)
            {
                meshDict.Add(mesh.name, mesh);
            }
        }
        return meshDict[path];
    }
}
