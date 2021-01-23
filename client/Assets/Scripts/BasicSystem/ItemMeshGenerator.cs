using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMeshGenerator
{
    Texture2D texture;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();
    List<int> triangles = new List<int>();

    float unit;
    float uv_offset;
    float pos_offset;

    int GetIndexByCoord(int j, int i)
    {
        return i * texture.width + j;
    }

    static ItemMeshGenerator _instance;
    public static ItemMeshGenerator instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemMeshGenerator();
            }
            return _instance;
        }
    }

    public Mesh Generate(Texture2D tex)
    {
        texture = tex;

        vertices.Clear();
        uv.Clear();
        triangles.Clear();

        unit = 1f / texture.width;
        uv_offset = unit / 5;
        pos_offset = -0.5f;

        Mesh mesh = new Mesh();

        Color32[] colors = texture.GetPixels32();

        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                int index = i * texture.width + j;

                int x = j;
                int y = i;

                if (colors[index].a > 0)
                {
                    AddFrontFace(x, y);
                    AddBackFace(x, y);

                    if (x == 0 || colors[GetIndexByCoord(x - 1, y)].a == 0)
                        AddLeftFace(x, y);

                    if (x == texture.width - 1 || colors[GetIndexByCoord(x + 1, y)].a == 0)
                        AddRightFace(x, y);

                    if (y == texture.height - 1 || colors[GetIndexByCoord(x, y + 1)].a == 0)
                        AddTopFace(x, y);

                    if (y == 0 || colors[GetIndexByCoord(x, y - 1)].a == 0)
                        AddBottomFace(x, y);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }

    void AddUV(int start, int x, int y)
    {
        uv.Add(new Vector2(unit * x + uv_offset, unit * y + uv_offset));
        uv.Add(new Vector2(unit * (x + 1) - uv_offset, unit * y + uv_offset));
        uv.Add(new Vector2(unit * (x + 1) - uv_offset, unit * (y + 1) - uv_offset));
        uv.Add(new Vector2(unit * x + uv_offset, unit * (y + 1) - uv_offset));
        triangles.Add(start);
        triangles.Add(start + 3);
        triangles.Add(start + 2);
        triangles.Add(start);
        triangles.Add(start + 2);
        triangles.Add(start + 1);
    }

    void AddVertices(int x, int y, int z)
    {
        vertices.Add(new Vector3(unit * x + pos_offset, unit * y + pos_offset, unit * z / 2));
    }

    void AddFrontFace(int x, int y)
    {
        int count = vertices.Count;
        AddVertices(x, y, -1);
        AddVertices(x + 1, y, -1);
        AddVertices(x + 1, y + 1, -1);
        AddVertices(x, y + 1, -1);
        AddUV(count, x, y);
    }

    void AddBackFace(int x, int y)
    {
        int count = vertices.Count;
        AddVertices(x + 1, y, 1);
        AddVertices(x, y, 1);
        AddVertices(x, y + 1, 1);
        AddVertices(x + 1, y + 1, 1);
        AddUV(count, x, y);
    }

    void AddLeftFace(int x, int y)
    {
        int count = vertices.Count;
        AddVertices(x, y, 1);
        AddVertices(x, y, -1);
        AddVertices(x, y + 1, -1);
        AddVertices(x, y + 1, 1);
        AddUV(count, x, y);
    }

    void AddRightFace(int x, int y)
    {
        int count = vertices.Count;
        AddVertices(x + 1, y, -1);
        AddVertices(x + 1, y, 1);
        AddVertices(x + 1, y + 1, 1);
        AddVertices(x + 1, y + 1, -1);
        AddUV(count, x, y);
    }
    
    void AddTopFace(int x, int y)
    {
        int count = vertices.Count;
        AddVertices(x, y + 1, -1);
        AddVertices(x + 1, y + 1, -1);
        AddVertices(x + 1, y + 1, 1);
        AddVertices(x, y + 1, 1);
        AddUV(count, x, y);
    }

    void AddBottomFace(int x, int y)
    {
        int count = vertices.Count;
        AddVertices(x, y, 1);
        AddVertices(x + 1, y, 1);
        AddVertices(x + 1, y, -1);
        AddVertices(x, y, -1);
        AddUV(count, x, y);
    }
}
