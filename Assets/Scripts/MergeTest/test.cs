using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public class test : MonoBehaviour
{
    public struct TexCoords
    {
        public Vector2Int front;
        public Vector2Int right;
        public Vector2Int left;
        public Vector2Int back;
        public Vector2Int top;
        public Vector2Int bottom;
    }


    static Vector2Int grass_bottom = new Vector2Int(0, 0);
    static Vector2Int grass_side = new Vector2Int(0, 1);
    static Vector2Int grass_top = new Vector2Int(0, 2);
    static Vector2Int brick = new Vector2Int(3, 0);

    public static Dictionary<CSBlockType, TexCoords> type2texcoords = new Dictionary<CSBlockType, TexCoords>
    {
        {CSBlockType.Grass, new TexCoords{ front = grass_side, right = grass_side, left = grass_side, back = grass_side, top = grass_top, bottom = grass_bottom } },
        {CSBlockType.Dirt, new TexCoords{ front = grass_bottom, right = grass_bottom, left = grass_bottom, back = grass_bottom, top = grass_bottom, bottom = grass_bottom } },
        {CSBlockType.Tnt, new TexCoords{ front = grass_side, right = grass_side, left = grass_side, back = grass_side, top = grass_top, bottom = grass_bottom } },
        {CSBlockType.Brick, new TexCoords{ front = brick, right = brick, left = brick, back = brick, top = brick, bottom = brick } },
        {CSBlockType.Furnace, new TexCoords{ front = grass_side, right = grass_side, left = grass_side, back = grass_side, top = grass_top, bottom = grass_bottom } },
        {CSBlockType.HayBlock, new TexCoords{ front = grass_side, right = grass_side, left = grass_side, back = grass_side, top = grass_top, bottom = grass_bottom } },
    };

    static Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    static Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    static Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    static Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    static Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    static Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    static Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    static Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    static Vector2 uv0_0 = new Vector2(0, 0);
    static Vector2 uv1_0 = new Vector2(1 / 16f, 0);
    static Vector2 uv0_1 = new Vector2(0, 1 / 16f);
    static Vector2 uv1_1 = new Vector2(1 / 16f, 1 / 16f);
    static Vector2 uv0_2 = new Vector2(0, 2 / 16f);
    static Vector2 uv1_2 = new Vector2(1 / 16f, 2 / 16f);
    static Vector2 uv0_3 = new Vector2(0, 3 / 16f);
    static Vector2 uv1_3 = new Vector2(1 / 16f, 3 / 16f);

    static List<Vector3> vertices = new List<Vector3>();
    static List<Vector2> uv = new List<Vector2>();
    static List<int> triangles = new List<int>();

    static void ClearData()
    {
        vertices.Clear();
        uv.Clear();
        triangles.Clear();
    }

    static void AddUV(Vector2Int texPos)
    {
        uv.Add(new Vector2(texPos.x / 16f, texPos.y / 16f));
        uv.Add(new Vector2(texPos.x / 16f, (texPos.y + 1) / 16f));
        uv.Add(new Vector2((texPos.x + 1) / 16f, texPos.y / 16f));
        uv.Add(new Vector2((texPos.x + 1) / 16f, (texPos.y + 1) / 16f));
        triangles.AddRange(new int[] { vertices.Count - 4, vertices.Count - 3, vertices.Count - 2, vertices.Count - 3, vertices.Count - 1, vertices.Count - 2 });
    }

    // vertex order
    // 1 3
    // 0 2 
    static void AddFrontFace(Vector3Int pos, Vector2Int texPos)
    {
        vertices.AddRange(new List<Vector3> { nearBottomLeft + pos, nearTopLeft + pos, nearBottomRight + pos, nearTopRight + pos });
        AddUV(texPos);
    }

    static void AddRightFace(Vector3Int pos, Vector2Int texPos)
    {
        vertices.AddRange(new List<Vector3> { nearBottomRight + pos, nearTopRight + pos, farBottomRight + pos, farTopRight + pos });
        AddUV(texPos);
    }

    static void AddLeftFace(Vector3Int pos, Vector2Int texPos)
    {
        vertices.AddRange(new List<Vector3> { farBottomLeft + pos, farTopLeft + pos, nearBottomLeft + pos, nearTopLeft + pos });
        AddUV(texPos);
    }

    static void AddBackFace(Vector3Int pos, Vector2Int texPos)
    {
        vertices.AddRange(new List<Vector3> { farBottomRight + pos, farTopRight + pos, farBottomLeft + pos, farTopLeft + pos });
        AddUV(texPos);
    }

    static void AddTopFace(Vector3Int pos, Vector2Int texPos)
    {
        vertices.AddRange(new List<Vector3> { nearTopLeft + pos, farTopLeft + pos, nearTopRight + pos, farTopRight + pos });
        AddUV(texPos);
    }

    static void AddBottomFace(Vector3Int pos, Vector2Int texPos)
    {
        vertices.AddRange(new List<Vector3> { farBottomLeft + pos, nearBottomLeft + pos, farBottomRight + pos, nearBottomRight + pos });
        AddUV(texPos);
    }

    int sight = 12;
    // Start is called before the first frame update
    void Start()
    {
        int start;
        int max;
        if (sight % 2 == 0)
        {
            int half = sight / 2;
            start = -half;
            max = half - 1;
        }
        else
        {
            int half = Mathf.FloorToInt(sight / 2);
            start = -half;
            max = half;
        }

        for (int i = start; i < max; i++)
        {
            for (int j = start; j < max; j++)
            {
                GenerateChunk(new Vector2Int(i, j));
            }
        }
        mergetestPlayerController.Init();
    }
    
    static Dictionary<Vector2Int, GameObject> chunk2object = new Dictionary<Vector2Int, GameObject>();


    static Dictionary<Vector3Int, Block> posBlockDict = new Dictionary<Vector3Int, Block>();
    static Dictionary<Vector2Int, Dictionary<Vector3Int, Block>> chunkBlocksDict = new Dictionary<Vector2Int, Dictionary<Vector3Int, Block>>();

    public static bool ContainBlock(Vector3Int blockPos)
    {
        return posBlockDict.ContainsKey(blockPos);
    }

    public static void AddBlock(Vector3Int blockPos)
    {
        Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt(blockPos.x / 16), Mathf.FloorToInt(blockPos.z / 16));

        Block block = new Block { pos = blockPos, chunk = chunkPos, type = CSBlockType.Brick };
        posBlockDict[blockPos] = block;
        chunkBlocksDict[chunkPos].Add(block.pos, block);

        RefreshChunk(chunkPos);
    }

    public static void RemoveBlock(Vector3Int blockPos)
    {
        Block block = posBlockDict[blockPos];
        Vector2Int chunkPos = block.chunk;

        posBlockDict.Remove(blockPos);
        chunkBlocksDict[chunkPos].Remove(blockPos);

        RefreshChunk(chunkPos);
    }

    public static void GenerateChunk(List<CSChunk> chunkList)
    {
        foreach (CSChunk chunk in chunkList)
        {
            Dictionary<Vector3Int, Block> chunk_posBlockDict = new Dictionary<Vector3Int, Block>();

            Vector2Int chunkPos = new Vector2Int(chunk.Position.x, chunk.Position.y);
            foreach (CSBlock csblock in chunk.Blocks)
            {
                Vector3Int blockPos = new Vector3Int(csblock.position.x, csblock.position.y, csblock.position.z);
                Block block = new Block { pos = blockPos, chunk = chunkPos, type = csblock.type };
                posBlockDict[blockPos] = block;
                chunk_posBlockDict[blockPos] = block;
            }
            chunkBlocksDict[chunkPos] = chunk_posBlockDict;

            GameObject chunkObj = GenerateChunkObj(chunkPos);
            chunk2object[chunkPos] = chunkObj;
        }
    }


    static int scale = 35;
    static int maxHeight = 15;
    void GenerateChunk(Vector2Int chunkPos)
    {
        Dictionary<Vector3Int, Block> chunk_posBlockDict = new Dictionary<Vector3Int, Block>();

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                float x = (float)0.5 + i + chunkPos.x * 16;
                float z = (float)0.5 + j + chunkPos.y * 16;
                float noise = Mathf.PerlinNoise(x / scale, z / scale);
                int height = Mathf.RoundToInt(maxHeight * noise);
                for (int k = 0; k <= height; k++)
                {
                    Vector3Int blockPos = new Vector3Int(i + chunkPos.x * 16, k, j + chunkPos.y * 16);
                    CSBlockType type = k == height ? CSBlockType.Grass : CSBlockType.Dirt;
                    Block block = new Block { pos = blockPos, chunk = chunkPos, type = type };
                    posBlockDict[blockPos] = block;
                    chunk_posBlockDict[blockPos] = block;
                }
            }
        }
        chunkBlocksDict[chunkPos] = chunk_posBlockDict;

        GameObject chunkObj = GenerateChunkObj(chunkPos);
        chunk2object[chunkPos] = chunkObj;
    }

    struct Block
    {
        public Vector3Int pos;
        public Vector2Int chunk;
        public CSBlockType type;
    }


    static Vector3Int Vector3Int_forward = new Vector3Int(0, 0, 1);
    static Vector3Int Vector3Int_back = new Vector3Int(0, 0, -1);

    static Mesh RebuildMesh(Dictionary<Vector3Int, Block> chunkBlocks)
    {
        Mesh mesh = new Mesh();

        ClearData();
        foreach (Block block in chunkBlocks.Values)
        {
            Vector3Int pos = block.pos;
            TexCoords texCoords = type2texcoords[block.type];

            if (!chunkBlocks.ContainsKey(pos + Vector3Int_back))
                AddFrontFace(pos, texCoords.front);
            if (!chunkBlocks.ContainsKey(pos + Vector3Int.right))
                AddRightFace(pos, texCoords.right);
            if (!chunkBlocks.ContainsKey(pos + Vector3Int.left))
                AddLeftFace(pos, texCoords.left);
            if (!chunkBlocks.ContainsKey(pos + Vector3Int_forward))
                AddBackFace(pos, texCoords.back);
            if (!chunkBlocks.ContainsKey(pos + Vector3Int.up))
                AddTopFace(pos, texCoords.top);
            if (!chunkBlocks.ContainsKey(pos + Vector3Int.down))
                AddBottomFace(pos, texCoords.bottom);
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        return mesh;
    }

    static Mesh RebuildMesh(HashSet<Vector3Int> chunkBlocks)
    {
        Mesh mesh = new Mesh();

        ClearData();
        foreach (Vector3Int pos in chunkBlocks)
        {
            if (!chunkBlocks.Contains(pos + Vector3Int_back))
                AddFrontFace(pos, grass_side);
            if (!chunkBlocks.Contains(pos + Vector3Int.right))
                AddRightFace(pos, grass_side);
            if (!chunkBlocks.Contains(pos + Vector3Int.left))
                AddLeftFace(pos, grass_side);
            if (!chunkBlocks.Contains(pos + Vector3Int_forward))
                AddBackFace(pos, grass_side);
            if (!chunkBlocks.Contains(pos + Vector3Int.up))
                AddTopFace(pos, grass_top);
            if (!chunkBlocks.Contains(pos + Vector3Int.down))
                AddBottomFace(pos, grass_bottom);
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        return mesh;
    }

    static GameObject GenerateChunkObj(Vector2Int chunkPos)
    {
        GameObject chunk = new GameObject("chunk (" + chunkPos.x + "," + chunkPos.y + ")");
        MeshFilter mf = chunk.AddComponent<MeshFilter>();

        mf.mesh = RebuildMesh(chunkBlocksDict[chunkPos]);
        MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
        mr.material = Resources.Load<Material>("merge-test/mat");

        chunk.AddComponent<MeshCollider>();
        chunk.layer = LayerMask.NameToLayer("Block");
        return chunk;
    }

    static void RefreshChunk(Vector2Int chunkPos)
    {
        GameObject chunkObj = chunk2object[chunkPos];
        MeshFilter mf = chunkObj.GetComponent<MeshFilter>();
        mf.mesh = RebuildMesh(chunkBlocksDict[chunkPos]);
        MeshCollider mc = chunkObj.GetComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;
    }
}
