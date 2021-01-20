using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMeshGenerator : MonoBehaviour
{
    public Texture2D texture;

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();
    List<int> triangles = new List<int>();

    float unit;
    float offset;

    int GetIndexByCoord(int x, int y)
    {
        return y * texture.height + x;
    }

    private void Start()
    {
        unit = 1f / texture.width;
        offset = unit / 5;

        Mesh mesh = new Mesh();

        Color32[] colors = texture.GetPixels32();
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                int index = i * texture.height + j;

                if (colors[index].a > 0)
                {
                    AddFrontFace(j, i);
                    AddBackFace(j, i);

                    if (j == 0 || colors[GetIndexByCoord(j - 1, i)].a == 0)
                        AddLeftFace(j, i);

                    if (j == texture.width || colors[GetIndexByCoord(j + 1, i)].a == 0)
                        AddRightFace(j, i);

                    if (i == texture.height || colors[GetIndexByCoord(j, i + 1)].a == 0)
                        AddTopFace(j, i);

                    if (i == 0 || colors[GetIndexByCoord(j, i - 1)].a == 0)
                        AddBottomFace(j, i);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Custom/BlockShader"));
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
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
