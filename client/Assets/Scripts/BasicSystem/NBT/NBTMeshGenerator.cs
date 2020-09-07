using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public abstract class NBTMeshGenerator
{
    protected static Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    protected static Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    protected static Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    protected static Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    protected static Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    protected static Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    protected static Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    protected static Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    public abstract void GenerateMeshInChunk(NBTChunk chunk, byte blockData, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv);

    public abstract void AfterGenerateMesh(List<List<int>> trianglesList, List<Material> materialList);

    public abstract void ClearData();

    protected static void AddUV(List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        // bottom left
        uv.Add(Vector2.zero);
        // top left
        uv.Add(Vector2.up);
        // top right
        uv.Add(Vector2.one);
        // bottom right
        uv.Add(Vector2.right);

        int verticesCount = vertices.Count;
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);
    }

    protected static void AddUV_BackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        // bottom right
        uv.Add(Vector2.right);
        // top right
        uv.Add(Vector2.one);
        // top left
        uv.Add(Vector2.up);
        // bottom left
        uv.Add(Vector2.zero);

        int verticesCount = vertices.Count;
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 1);
        triangles.Add(verticesCount - 2);
    }

    protected static void AddFrontFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((nearBottomLeft) + pos);
        vertices.Add((nearTopLeft) + pos);
        vertices.Add((nearTopRight) + pos);
        vertices.Add((nearBottomRight) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddRightFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((nearBottomRight) + pos);
        vertices.Add((nearTopRight) + pos);
        vertices.Add((farTopRight) + pos);
        vertices.Add((farBottomRight) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddLeftFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((farBottomLeft) + pos);
        vertices.Add((farTopLeft) + pos);
        vertices.Add((nearTopLeft) + pos);
        vertices.Add((nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddBackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((farBottomRight) + pos);
        vertices.Add((farTopRight) + pos);
        vertices.Add((farTopLeft) + pos);
        vertices.Add((farBottomLeft) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddTopFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((farTopRight) + pos);
        vertices.Add((nearTopRight) + pos);
        vertices.Add((nearTopLeft) + pos);
        vertices.Add((farTopLeft) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddBottomFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos)
    {
        vertices.Add((nearBottomRight) + pos);
        vertices.Add((farBottomRight) + pos);
        vertices.Add((farBottomLeft) + pos);
        vertices.Add((nearBottomLeft) + pos);
        AddUV(vertices, uv, triangles);
    }

    protected static void AddDiagonalFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(nearBottomRight + pos);
        AddUV(vertices, uv, triangles);
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(nearBottomRight + pos);
        AddUV_BackFace(vertices, uv, triangles);
    }

    protected static void AddAntiDiagonalFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos)
    {
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farBottomRight + pos);
        AddUV(vertices, uv, triangles);
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farBottomRight + pos);
        AddUV_BackFace(vertices, uv, triangles);
    }

    protected void CopyFromMesh(Mesh mesh, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        int length = vertices.Count;
        foreach (Vector3 vertex in mesh.vertices)
        {
            vertices.Add(vertex + pos);
        }
        uv.AddRange(mesh.uv);

        foreach (int index in mesh.triangles)
        {
            triangles.Add(index + length);
        }
    }
}
