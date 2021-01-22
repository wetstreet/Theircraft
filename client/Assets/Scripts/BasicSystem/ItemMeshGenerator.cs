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
    float offset;

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
        Debug.Log("generator,tex=" + texture + ",height=" + texture.height);

        unit = 1f / texture.width;
        offset = unit / 5;

        Mesh mesh = new Mesh();

        Color32[] colors = texture.GetPixels32();

        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                int index = i * texture.width + j;
                Debug.Log("i=" + i + ",j=" + j + ",index=" + index + ",color=" + colors[index]);

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
        uv.Add(new Vector2(unit * x + offset, unit * y + offset));
        uv.Add(new Vector2(unit * (x + 1) - offset, unit * y + offset));
        uv.Add(new Vector2(unit * (x + 1) - offset, unit * (y + 1) - offset));
        uv.Add(new Vector2(unit * x + offset, unit * (y + 1) - offset));
        triangles.Add(start);
        triangles.Add(start + 3);
        triangles.Add(start + 2);
        triangles.Add(start);
        triangles.Add(start + 2);
        triangles.Add(start + 1);
    }

    void AddFrontFace(int x, int y)
    {
        int count = vertices.Count;
        vertices.Add(new Vector3(unit * x, unit * y, -unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * y, -unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * (y + 1), -unit / 2));
        vertices.Add(new Vector3(unit * x, unit * (y + 1), -unit / 2));
        AddUV(count, x, y);
    }

    void AddBackFace(int x, int y)
    {
        int count = vertices.Count;
        vertices.Add(new Vector3(unit * (x + 1), unit * y, unit / 2));
        vertices.Add(new Vector3(unit * x, unit * y, unit / 2));
        vertices.Add(new Vector3(unit * x, unit * (y + 1), unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * (y + 1), unit / 2));
        AddUV(count, x, y);
    }

    void AddLeftFace(int x, int y)
    {
        int count = vertices.Count;
        vertices.Add(new Vector3(unit * x, unit * y, unit / 2));
        vertices.Add(new Vector3(unit * x, unit * y, -unit / 2));
        vertices.Add(new Vector3(unit * x, unit * (y + 1), -unit / 2));
        vertices.Add(new Vector3(unit * x, unit * (y + 1), unit / 2));
        AddUV(count, x, y);
    }

    void AddRightFace(int x, int y)
    {
        int count = vertices.Count;
        vertices.Add(new Vector3(unit * (x + 1), unit * y, -unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * y, unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * (y + 1), unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * (y + 1), -unit / 2));
        AddUV(count, x, y);
    }
    
    void AddTopFace(int x, int y)
    {
        int count = vertices.Count;
        vertices.Add(new Vector3(unit * x, unit * (y + 1), -unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * (y + 1), -unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * (y + 1), unit / 2));
        vertices.Add(new Vector3(unit * x, unit * (y + 1), unit / 2));
        AddUV(count, x, y);
    }

    void AddBottomFace(int x, int y)
    {
        int count = vertices.Count;
        vertices.Add(new Vector3(unit * x, unit * y, unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * y, unit / 2));
        vertices.Add(new Vector3(unit * (x + 1), unit * y, -unit / 2));
        vertices.Add(new Vector3(unit * x, unit * y, -unit / 2));
        AddUV(count, x, y);
    }
}
