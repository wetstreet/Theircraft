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
        Mesh mesh = new Mesh();
        mesh.name = "CubeMesh";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        ChunkMeshGenerator.TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];
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

    override public void GenerateMeshInChunk(CSBlockType type, Vector3Int posInChunk, Vector3Int globalPos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        ChunkMeshGenerator.TexCoords texCoords = ChunkMeshGenerator.type2texcoords[(byte)type];

        CSBlockOrientation orient = ChunkManager.GetBlockOrientation(globalPos);

        Matrix4x4 rotationMatrix = Matrix4x4.identity;
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

        AddFrontFace(rotationMatrix, vertices, uv, triangles, posInChunk, texCoords.front);
        AddTopFace(rotationMatrix, vertices, uv, triangles, posInChunk, texCoords.top);
        AddRightFace(rotationMatrix, vertices, uv, triangles, posInChunk, texCoords.right);
        AddLeftFace(rotationMatrix, vertices, uv, triangles, posInChunk, texCoords.left);
        AddBackFace(rotationMatrix, vertices, uv, triangles, posInChunk, texCoords.back);
        AddBottomFace(rotationMatrix, vertices, uv, triangles, posInChunk, texCoords.bottom);
    }
    
    static Vector3 nearMidLeft = new Vector3(-0.5f, 0, -0.5f);
    static Vector3 nearMidRight = new Vector3(0.5f, 0, -0.5f);

    static Vector3 midMidLeft = new Vector3(-0.5f, 0, 0);
    static Vector3 midMidRight = new Vector3(0.5f, 0, 0);

    static Vector3 midTopLeft = new Vector3(-0.5f, 0.5f, 0);
    static Vector3 midTopRight = new Vector3(0.5f, 0.5f, 0);
    
    static void AddBottomFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddBackFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddLeftFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midMidLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearMidLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);

        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        // bottom left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, texPos.y / atlas_row + compensation_y));
        // top left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // top mid
        uv.Add(new Vector2((texPos.x + 0.5f) / atlas_column, (texPos.y + 1) / atlas_row - compensation_y));
        // mid mid
        uv.Add(new Vector2((texPos.x + 0.5f) / atlas_column, (texPos.y + 0.5f) / atlas_row));
        // mid right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 0.5f) / atlas_row));
        // bottom right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, texPos.y / atlas_row + compensation_y));

        int verticesCount = vertices.Count;

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 5);
        triangles.Add(verticesCount - 4);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);
    }

    static void AddRightFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(farBottomRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midMidRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearMidRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);

        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        // bottom left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, texPos.y / atlas_row + compensation_y));
        // top left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // top mid
        uv.Add(new Vector2((texPos.x + 0.5f) / atlas_column, (texPos.y + 1) / atlas_row - compensation_y));
        // mid mid
        uv.Add(new Vector2((texPos.x + 0.5f) / atlas_column, (texPos.y + 0.5f) / atlas_row));
        // mid right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 0.5f) / atlas_row));
        // bottom right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, texPos.y / atlas_row + compensation_y));

        int verticesCount = vertices.Count;

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 5);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 4);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 3);

        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 1);
        triangles.Add(verticesCount - 2);
    }

    static void AddUV_SplitInHalf(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector2 texPos)
    {
        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        // bottom left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, texPos.y / atlas_row + compensation_y));
        // mid left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 0.5f) / atlas_row));
        // mid right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 0.5f) / atlas_row));
        // bottom right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, texPos.y / atlas_row + compensation_y));

        // mid left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 0.5f) / atlas_row));
        // top left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // top right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // mid right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 0.5f) / atlas_row));

        int verticesCount = vertices.Count;

        triangles.Add(verticesCount - 8);
        triangles.Add(verticesCount - 7);
        triangles.Add(verticesCount - 6);

        triangles.Add(verticesCount - 8);
        triangles.Add(verticesCount - 6);
        triangles.Add(verticesCount - 5);

        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);

        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);
    }

    static void AddFrontFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearMidLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearMidRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearBottomRight) + pos);

        vertices.Add(rotationMatrix.MultiplyPoint(midMidLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midMidRight) + pos);

        AddUV_SplitInHalf(vertices, uv, triangles, texPos);
    }

    static void AddTopFace(Matrix4x4 rotationMatrix, List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(rotationMatrix.MultiplyPoint(nearMidLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midMidLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midMidRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(nearMidRight) + pos);

        vertices.Add(rotationMatrix.MultiplyPoint(midTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopLeft) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(farTopRight) + pos);
        vertices.Add(rotationMatrix.MultiplyPoint(midTopRight) + pos);

        AddUV_SplitInHalf(vertices, uv, triangles, texPos);
    }
}
