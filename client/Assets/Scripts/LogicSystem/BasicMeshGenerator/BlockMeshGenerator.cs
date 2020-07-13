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
        for (int i = 0; i < 4; i++)
        {
            normals.Add(Vector3.back);
        }
        AddRightFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.right);
        for (int i = 0; i < 4; i++)
        {
            normals.Add(Vector3.right);
        }
        AddLeftFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.left);
        for (int i = 0; i < 4; i++)
        {
            normals.Add(Vector3.left);
        }
        AddBackFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.back);
        for (int i = 0; i < 4; i++)
        {
            normals.Add(Vector3.forward);
        }
        AddTopFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.top);
        for (int i = 0; i < 4; i++)
        {
            normals.Add(Vector3.up);
        }
        AddBottomFace(Matrix4x4.identity, vertices, uv, triangles, Vector3Int.zero, texCoords.bottom);
        for (int i = 0; i < 4; i++)
        {
            normals.Add(Vector3.down);
        }

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);

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

    public void GenerateMeshInChunk(CSBlockType type, Vector3Int _pos, Vector3Int _globalPos, List<Vector3> _vertices, List<Color> _colors, List<Vector2> _uv, List<Vector3> _normals, List<int> _triangles)
    {
        texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
        pos = _pos;
        globalPos = _globalPos;
        vertices = _vertices;
        uv = _uv;
        triangles = _triangles;
        normals = _normals;
        colors = _colors;
        rotationMatrix = Matrix4x4.identity;

        if (texCoords.isRotatable)
        {
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
            AddFacesNoCull();
        }
        else
        {
            AddFacesCullInvisible();
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

    void AddFacesCullInvisible()
    {
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y, globalPos.z - 1))
        {
            AddFrontFace();
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x + 1, globalPos.y, globalPos.z))
        {
            AddRightFace();
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x - 1, globalPos.y, globalPos.z))
        {
            AddLeftFace();
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y, globalPos.z + 1))
        {
            AddBackFace();
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y + 1, globalPos.z))
        {
            AddTopFace();
        }
        if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(globalPos.x, globalPos.y - 1, globalPos.z))
        {
            AddBottomFace();
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

    void AddFacesNoCull()
    {
        AddFrontFace();
        AddRightFace();
        AddLeftFace();
        AddBackFace();
        AddTopFace();
        AddBottomFace();
    }

    protected void AddFrontFace()
    {
        Color color = ChunkManager.GetBlockLightColor(globalPos + Utilities.vector3Int.back);
        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
            normals.Add(Vector3.back);
        }
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        AddUV(vertices, uv, triangles, texCoords.front);
    }

    protected static void AddFrontFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected void AddRightFace()
    {
        Color color = ChunkManager.GetBlockLightColor(globalPos + Utilities.vector3Int.right);
        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
            normals.Add(Vector3.right);
        }
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        AddUV(vertices, uv, triangles, texCoords.right);
    }

    protected static void AddRightFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected void AddLeftFace()
    {
        Color color = ChunkManager.GetBlockLightColor(globalPos + Utilities.vector3Int.left);
        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
            normals.Add(Vector3.left);
        }
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texCoords.left);
    }

    protected static void AddLeftFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected void AddBackFace()
    {
        Color color = ChunkManager.GetBlockLightColor(globalPos + Utilities.vector3Int.forward);
        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
            normals.Add(Vector3.forward);
        }
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texCoords.back);
    }

    protected static void AddBackFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected void AddTopFace()
    {
        Color color = ChunkManager.GetBlockLightColor(globalPos + Utilities.vector3Int.up);
        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
            normals.Add(Vector3.up);
        }
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        AddUV(vertices, uv, triangles, texCoords.top);
    }

    protected static void AddTopFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    protected void AddBottomFace()
    {
        Color color = ChunkManager.GetBlockLightColor(globalPos + Utilities.vector3Int.down);
        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
            normals.Add(Vector3.down);
        }
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texCoords.bottom);
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
