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

    static Vector3Int forward = new Vector3Int(0, 0, 1);
    static Vector3Int back = new Vector3Int(0, 0, -1);
    static Mesh GetMeshByOrientationAndPosition(CSBlockOrientation orientation, Vector3Int globalPosition)
    {
        Mesh mesh = null;
        Debug.Log("orient=" + orientation);
        switch (orientation)
        {
            case CSBlockOrientation.PositiveY_NegativeX:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x");
                if (ChunkManager.GetBlockType(globalPosition + Vector3Int.right) == CSBlockType.BrickStairs)
                {
                    CSBlockOrientation rightOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.right);
                    if (rightOrientation == CSBlockOrientation.PositiveY_PositiveZ)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_top_left");
                    }
                    else if (rightOrientation == CSBlockOrientation.PositiveY_NegativeZ)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_bottom_left");
                    }
                }
                else if (ChunkManager.GetBlockType(globalPosition + Vector3Int.left) == CSBlockType.BrickStairs)
                {
                    CSBlockOrientation leftOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.left);
                    if (leftOrientation == CSBlockOrientation.PositiveY_PositiveZ)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_no_bottom_right");
                    }
                    else if (leftOrientation == CSBlockOrientation.PositiveY_NegativeZ)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_no_top_right");
                    }
                }
                break;
            case CSBlockOrientation.PositiveY_NegativeZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-z");
                if (ChunkManager.GetBlockType(globalPosition + forward) == CSBlockType.BrickStairs)
                {
                    CSBlockOrientation forwardOrientation = ChunkManager.GetBlockOrientation(globalPosition + forward);
                    if (forwardOrientation == CSBlockOrientation.PositiveY_PositiveX)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_bottom_right");
                    }
                    else if (forwardOrientation == CSBlockOrientation.PositiveY_NegativeX)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_bottom_left");
                    }
                }
                else if (ChunkManager.GetBlockType(globalPosition + back) == CSBlockType.BrickStairs)
                {
                    CSBlockOrientation backOrientation = ChunkManager.GetBlockOrientation(globalPosition + back);
                    if (backOrientation == CSBlockOrientation.PositiveY_PositiveX)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_no_top_left");
                    }
                    else if (backOrientation == CSBlockOrientation.PositiveY_NegativeX)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_no_top_right");
                    }
                }
                break;
            case CSBlockOrientation.PositiveY_PositiveX:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x");
                if (ChunkManager.GetBlockType(globalPosition + Vector3Int.left) == CSBlockType.BrickStairs)
                {
                    CSBlockOrientation leftOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.left);
                    if (leftOrientation == CSBlockOrientation.PositiveY_PositiveZ)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_top_right");
                    }
                    else if (leftOrientation == CSBlockOrientation.PositiveY_NegativeZ)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_bottom_right");
                    }
                }
                else if (ChunkManager.GetBlockType(globalPosition + Vector3Int.right) == CSBlockType.BrickStairs)
                {
                    CSBlockOrientation rightOrientation = ChunkManager.GetBlockOrientation(globalPosition + Vector3Int.right);
                    if (rightOrientation == CSBlockOrientation.PositiveY_PositiveZ)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_no_bottom_left");
                    }
                    else if (rightOrientation == CSBlockOrientation.PositiveY_NegativeZ)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_no_top_left");
                    }
                }
                break;
            case CSBlockOrientation.PositiveY_PositiveZ:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+z");
                if (ChunkManager.GetBlockType(globalPosition + forward) == CSBlockType.BrickStairs)
                {
                    CSBlockOrientation forwardOrientation = ChunkManager.GetBlockOrientation(globalPosition + forward);
                    if (forwardOrientation == CSBlockOrientation.PositiveY_PositiveX)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_no_bottom_left");
                    }
                    else if (forwardOrientation == CSBlockOrientation.PositiveY_NegativeX)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_no_bottom_right");
                    }
                }
                else if (ChunkManager.GetBlockType(globalPosition + back) == CSBlockType.BrickStairs)
                {
                    CSBlockOrientation backOrientation = ChunkManager.GetBlockOrientation(globalPosition + back);
                    if (backOrientation == CSBlockOrientation.PositiveY_PositiveX)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_top_right");
                    }
                    else if (backOrientation == CSBlockOrientation.PositiveY_NegativeX)
                    {
                        mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_joint_top_left");
                    }
                }
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

        Mesh stairMesh = GetMeshByOrientationAndPosition(orient, globalPos);
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
