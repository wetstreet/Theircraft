using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{


    Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    Vector2 uv0_0 = new Vector2(0, 0);
    Vector2 uv1_0 = new Vector2(1 / 16f, 0);
    Vector2 uv0_1 = new Vector2(0, 1 / 16f);
    Vector2 uv1_1 = new Vector2(1 / 16f, 1 / 16f);
    Vector2 uv0_2 = new Vector2(0, 2 / 16f);
    Vector2 uv1_2 = new Vector2(1 / 16f, 2 / 16f);
    Vector2 uv0_3 = new Vector2(0, 3 / 16f);
    Vector2 uv1_3 = new Vector2(1 / 16f, 3 / 16f);

    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();
    List<int> triangles = new List<int>();

    void ClearData()
    {
        vertices.Clear();
        uv.Clear();
        triangles.Clear();
    }

    void AddCube(Vector3Int pos)
    {
        AddFrontFace(pos);
        AddRightFace(pos);
        AddLeftFace(pos);
        AddBackFace(pos);
        AddTopFace(pos);
        AddBottomFace(pos);
    }

    // vertex order
    // 1 3
    // 0 2 
    void AddFrontFace(Vector3Int pos)
    {
        vertices.AddRange(new List<Vector3> { nearBottomLeft + pos, nearTopLeft + pos, nearBottomRight + pos, nearTopRight + pos });
        uv.AddRange(new List<Vector2> { uv0_1, uv0_2, uv1_1, uv1_2 });
        triangles.AddRange(new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2 });
    }

    void AddRightFace(Vector3Int pos)
    {
        vertices.AddRange(new List<Vector3> { nearBottomRight + pos, nearTopRight + pos, farBottomRight + pos, farTopRight + pos });
        uv.AddRange(new List<Vector2> { uv0_1, uv0_2, uv1_1, uv1_2 });
        triangles.AddRange(new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2 });
    }

    void AddLeftFace(Vector3Int pos)
    {
        vertices.AddRange(new List<Vector3> { farBottomLeft + pos, farTopLeft + pos, nearBottomLeft + pos, nearTopLeft + pos });
        uv.AddRange(new List<Vector2> { uv0_1, uv0_2, uv1_1, uv1_2 });
        triangles.AddRange(new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2 });
    }

    void AddBackFace(Vector3Int pos)
    {
        vertices.AddRange(new List<Vector3> { farBottomRight + pos, farTopRight + pos, farBottomLeft + pos, farTopLeft + pos });
        uv.AddRange(new List<Vector2> { uv0_1, uv0_2, uv1_1, uv1_2 });
        triangles.AddRange(new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2 });
    }

    void AddTopFace(Vector3Int pos)
    {
        vertices.AddRange(new List<Vector3> { nearTopLeft + pos, farTopLeft + pos, nearTopRight + pos, farTopRight + pos });
        uv.AddRange(new List<Vector2> { uv0_2, uv0_3, uv1_2, uv1_3 });
        triangles.AddRange(new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2 });
    }

    void AddBottomFace(Vector3Int pos)
    {
        vertices.AddRange(new List<Vector3> { farBottomLeft + pos, nearBottomLeft + pos, farBottomRight + pos, nearBottomRight + pos });
        uv.AddRange(new List<Vector2> { uv0_0, uv0_1, uv1_0, uv1_1 });
        triangles.AddRange(new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2 });
    }

    int sight = 12;
    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < sight; i++)
        {
            for (int j = 0; j < sight; j++)
            {
                GenerateChunk(new Vector2Int(i, j));
            }
        }
        mergetestPlayerController.Init();
    }

    static int scale = 35;
    static int maxHeight = 15;
    void GenerateChunk(Vector2Int pos)
    {
        HashSet<Vector3Int> blocks = new HashSet<Vector3Int>();
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                float x = (float)0.5 + i + pos.x * 16;
                float z = (float)0.5 + j + pos.y * 16;
                float noise = Mathf.PerlinNoise(x / scale, z / scale);
                int height = Mathf.RoundToInt(maxHeight * noise);
                for (int k = 0; k < height; k++)
                {
                    blocks.Add(new Vector3Int(i, k, j));
                }
            }
        }
        GameObject chunkObj = GenerateChunk(blocks);
        chunkObj.transform.localPosition = new Vector3(pos.x * 16, 0, pos.y * 16);
    }


    Vector3Int Vector3Int_forward = new Vector3Int(0, 0, 1);
    Vector3Int Vector3Int_back = new Vector3Int(0, 0, -1);

    GameObject GenerateChunk(HashSet<Vector3Int> chunkBlocks)
    {
        Texture tex = Resources.Load<Texture>("merge-test/texture");
        GameObject chunk = new GameObject("chunk");
        MeshFilter mf = chunk.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        ClearData();
        foreach (Vector3Int pos in chunkBlocks)
        {
            if (!chunkBlocks.Contains(pos + Vector3Int_back))
                AddFrontFace(pos);

            if (!chunkBlocks.Contains(pos + Vector3Int.right))
                AddRightFace(pos);

            if (!chunkBlocks.Contains(pos + Vector3Int.left))
                AddLeftFace(pos);

            if (!chunkBlocks.Contains(pos + Vector3Int_forward))
                AddBackFace(pos);

            if (!chunkBlocks.Contains(pos + Vector3Int.up))
                AddTopFace(pos);

            if (!chunkBlocks.Contains(pos + Vector3Int.down))
                AddBottomFace(pos);
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        mf.mesh = mesh;
        MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("merge-test/mat");

        chunk.AddComponent<MeshCollider>();
        return chunk;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
