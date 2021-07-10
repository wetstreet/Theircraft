using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public float perlinScale = 1;
    [Range(0, 1)]
    public float threshold = 0.5f;
    public int unit = 100;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnGUI()
    {
        if (GUILayout.Button("test"))
        {
            Vector3Int playerPos = PlayerController.instance.position.ToVector3Int();

            HashSet<Vector3Int> cloudPos = new HashSet<Vector3Int>();

            for (int x = -unit; x <= unit; x++)
            {
                for (int z = -unit; z <= unit; z++)
                {
                    float noise = Mathf.PerlinNoise(x / perlinScale, z / perlinScale);
                    if (noise > threshold)
                    {
                        cloudPos.Add(new Vector3Int(playerPos.x + x, 128, playerPos.z + z));
                    }
                }
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            Vector3Int key = new Vector3Int();
            foreach (Vector3Int pos in cloudPos)
            {
                key.Set(pos.x, pos.y, pos.z - 1);
                if (!cloudPos.Contains(key))
                    AddFrontFace(vertices, triangles, pos);

                key.Set(pos.x, pos.y, pos.z + 1);
                if (!cloudPos.Contains(key))
                    AddBackFace(vertices, triangles, pos);

                AddTopFace(vertices, triangles, pos);
                AddBottomFace(vertices, triangles, pos);

                key.Set(pos.x - 1, pos.y, pos.z);
                if (!cloudPos.Contains(key))
                    AddLeftFace(vertices, triangles, pos);

                key.Set(pos.x + 1, pos.y, pos.z);
                if (!cloudPos.Contains(key))
                    AddRightFace(vertices, triangles, pos);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
