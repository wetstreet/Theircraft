using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public Texture2D cloudTex;
    public Material mat;

    void Start()
    {
        for (int i = -8; i < 8; i++)
        {
            for (int j = -8; j < 8; j++)
            {
                GameObject go = new GameObject(i + "," + j);
                go.transform.parent = transform;

                List <Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();
                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        Color noise = cloudTex.GetPixel(i * 16 + x, j * 16 + z);
                        if (noise.a > 0.5f)
                        {
                            Vector3 pos = new Vector3(i * 16 + x, 0, j * 16 + z);

                            AddFrontFace(vertices, triangles, pos);
                            AddBackFace(vertices, triangles, pos);
                            AddTopFace(vertices, triangles, pos);
                            AddBottomFace(vertices, triangles, pos);
                            AddLeftFace(vertices, triangles, pos);
                            AddRightFace(vertices, triangles, pos);
                        }
                    }
                }
                Mesh mesh = new Mesh();
                mesh.vertices = vertices.ToArray();
                mesh.triangles = triangles.ToArray();
                mesh.RecalculateNormals();

                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                go.AddComponent<MeshRenderer>().sharedMaterial = mat;
            }
        }
        Vector3 playerPos = DataCenter.spawnPosition;
        transform.localPosition = new Vector3(playerPos.x, 128, playerPos.z);
        transform.localScale = new Vector3(12, 4, 12);
    }

    public Vector3 speed = Vector3.zero;
    void Update()
    {
        transform.position += speed * Time.deltaTime;
    }

    static Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    static Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    static Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    static Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    static Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    static Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    static Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    static Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    void AddFace(List<Vector3> vertices, List<int> triangles, Vector3 pos, Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4)
    {
        int startIndex = vertices.Count;

        vertices.Add(pos + pos1);
        vertices.Add(pos + pos2);
        vertices.Add(pos + pos3);
        vertices.Add(pos + pos4);

        triangles.Add(startIndex);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 3);
    }

    void AddFrontFace(List<Vector3> vertices, List<int> triangles, Vector3 pos)
    {
        AddFace(vertices, triangles, pos, nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight);
    }

    void AddBackFace(List<Vector3> vertices, List<int> triangles, Vector3 pos)
    {
        AddFace(vertices, triangles, pos, farBottomRight, farTopRight, farTopLeft, farBottomLeft);
    }

    void AddTopFace(List<Vector3> vertices, List<int> triangles, Vector3 pos)
    {
        AddFace(vertices, triangles, pos, farTopRight, nearTopRight, nearTopLeft, farTopLeft);
    }

    void AddBottomFace(List<Vector3> vertices, List<int> triangles, Vector3 pos)
    {
        AddFace(vertices, triangles, pos, nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft);
    }

    void AddLeftFace(List<Vector3> vertices, List<int> triangles, Vector3 pos)
    {
        AddFace(vertices, triangles, pos, farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft);
    }

    void AddRightFace(List<Vector3> vertices, List<int> triangles, Vector3 pos)
    {
        AddFace(vertices, triangles, pos, nearBottomRight, nearTopRight, farTopRight, farBottomRight);
    }
}
