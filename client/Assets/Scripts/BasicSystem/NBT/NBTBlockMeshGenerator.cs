using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTBlockMeshGenerator : IMeshGenerator
{
    static NBTBlockMeshGenerator _instance;
    public static NBTBlockMeshGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new NBTBlockMeshGenerator();
            }
            return _instance;
        }
    }

    Matrix4x4 rotationMatrix;
    List<Vector3> vertices;
    List<Color> colors;
    List<Vector2> uv;
    List<Vector3> normals;
    List<int> triangles;
    TexCoords texCoords;
    Vector3Int pos;
    Vector3Int globalPos;

    override public Mesh GenerateSingleMesh(CSBlockType type)
    {
        Mesh mesh = new Mesh();
        mesh.name = "CubeMesh";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        AddFrontFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.front);
        AddRightFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.right);
        AddLeftFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.left);
        AddBackFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.back);
        AddTopFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.top);
        AddBottomFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.bottom);

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        return mesh;
    }

    public void GenerateMeshInChunk(NBTChunk chunk, CSBlockType type, Vector3Int pos, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            AddFrontFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.front);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            AddRightFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.right);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            AddLeftFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.left);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            AddBackFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.back);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            AddTopFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.top);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            AddBottomFace(Matrix4x4.identity, vertices, uv, triangles, pos, texCoords.bottom);
        }
    }
    
    protected static void AddFrontFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddRightFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected static void AddLeftFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }
    
    protected static void AddBackFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }
    
    protected static void AddTopFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }
    
    protected static void AddBottomFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }
}
