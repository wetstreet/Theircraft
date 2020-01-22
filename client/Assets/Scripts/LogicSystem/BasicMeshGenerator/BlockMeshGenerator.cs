using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class BlockMeshGenerator : IMeshGenerator
{
    static BlockMeshGenerator _instance;
    public static BlockMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BlockMeshGenerator();
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
        AddFrontFace(Matrix4x4.identity, vertices, uv, triangles, Vector3.zero, texCoords.front);
        AddRightFace(Matrix4x4.identity, vertices, uv, triangles, Vector3.zero, texCoords.right);
        AddLeftFace(Matrix4x4.identity, vertices, uv, triangles, Vector3.zero, texCoords.left);
        AddBackFace(Matrix4x4.identity, vertices, uv, triangles, Vector3.zero, texCoords.back);
        AddTopFace(Matrix4x4.identity, vertices, uv, triangles, Vector3.zero, texCoords.top);
        AddBottomFace(Matrix4x4.identity, vertices, uv, triangles, Vector3.zero, texCoords.bottom);

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int pos, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];

        if (texCoords.isRotatable)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.identity;
            CSBlockOrientation orient = ChunkManager.GetBlockOrientation(globalPos);
            if (orient != CSBlockOrientation.Default)
            {
                if (orient == CSBlockOrientation.PositiveY_NegativeZ)
                {
                    rotationMatrix = Matrix4x4.identity;
                }
                else if (orient == CSBlockOrientation.PositiveY_NegativeX)
                {
                    rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0));
                }
                else if (orient == CSBlockOrientation.PositiveY_PositiveZ)
                {
                    rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0));
                }
                else if (orient == CSBlockOrientation.PositiveY_PositiveX)
                {
                    rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 270, 0));
                }
            }
            AddFacesNoCull(rotationMatrix, texCoords, pos, vertices, uv, triangles);
        }
        else
        {
            AddFacesCullInvisible(texCoords, pos, globalPos, vertices, uv, triangles);
        }
    }

    static void AddFacesCullInvisible(TexCoords texCoords, Vector3Int pos, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y, globalPos.z - 1))
        {
            AddFrontFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.front);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x + 1, globalPos.y, globalPos.z))
        {
            AddRightFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.right);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x - 1, globalPos.y, globalPos.z))
        {
            AddLeftFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.left);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y, globalPos.z + 1))
        {
            AddBackFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.back);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y + 1, globalPos.z))
        {
            AddTopFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.top);
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y - 1, globalPos.z))
        {
            AddBottomFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.bottom);
        }
    }

    static void AddFacesNoCull(Matrix4x4 orientation, TexCoords texCoords, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        AddFrontFace(orientation, vertices, uv, triangles, pos, texCoords.front);
        AddRightFace(orientation, vertices, uv, triangles, pos, texCoords.right);
        AddLeftFace(orientation, vertices, uv, triangles, pos, texCoords.left);
        AddBackFace(orientation, vertices, uv, triangles, pos, texCoords.back);
        AddTopFace(orientation, vertices, uv, triangles, pos, texCoords.top);
        AddBottomFace(orientation, vertices, uv, triangles, pos, texCoords.bottom);
    }

    protected static void AddFrontFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddRightFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddLeftFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddBackFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddTopFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddBottomFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }
}
