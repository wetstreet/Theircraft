using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public class ChunkMeshGenerator : MonoBehaviour
{
    public struct TexCoords
    {
        public bool isPlant;
        public Vector2 front;
        public Vector2 right;
        public Vector2 left;
        public Vector2 back;
        public Vector2 top;
        public Vector2 bottom;
    }

    //方块图集的行数和列数
    static readonly int atlas_row = 58;
    static readonly int atlas_column = 24;

    //左上为0，0
    static Vector2 grass_bottom = new Vector2Int(2, 0);
    static Vector2 grass_side = new Vector2Int(3, 0);
    static Vector2 grass_top = new Vector2Int(0, 0);

    static Vector2 uv_stone = new Vector2Int(1, 0);

    static Vector2 brick = new Vector2Int(7, 0);

    static Vector2 tnt_side = new Vector2Int(8, 0);
    static Vector2 tnt_top = new Vector2Int(9, 0);
    static Vector2 tnt_bottom = new Vector2Int(10, 0);

    static Vector2 furnace_front = new Vector2Int(12, 2);
    static Vector2 furnace_side = new Vector2Int(13, 2);
    static Vector2 furnace_top = new Vector2Int(14, 3);

    static Vector2 hay_side = new Vector2Int(16, 13);
    static Vector2 hay_top = new Vector2Int(17, 13);

    static Vector2 leaves_side = new Vector2Int(4, 12);

    static Vector2 uv_grass = new Vector2Int(8, 2);

    static Vector2 uv_bedrock = new Vector2Int(1, 1);
    static Vector2 uv_poppy = new Vector2Int(12, 0);
    static Vector2 uv_dandelion = new Vector2Int(13, 0);
    static Vector2 uv_oakwood_side = new Vector2Int(4, 1);
    static Vector2 uv_oakwood_top = new Vector2Int(5, 1);
    static Vector2 uv_oakleaves = new Vector2Int(4, 3);

    static TexCoords Coords_None = new TexCoords();
    static TexCoords Coords_Dirt = new TexCoords { front = grass_bottom, right = grass_bottom, left = grass_bottom, back = grass_bottom, top = grass_bottom, bottom = grass_bottom };
    static TexCoords Coords_GrassBlock = new TexCoords { front = grass_side, right = grass_side, left = grass_side, back = grass_side, top = grass_top, bottom = grass_bottom };
    static TexCoords Coords_Tnt = new TexCoords { front = tnt_side, right = tnt_side, left = tnt_side, back = tnt_side, top = tnt_top, bottom = tnt_bottom };
    static TexCoords Coords_Brick = new TexCoords { front = brick, right = brick, left = brick, back = brick, top = brick, bottom = brick };
    static TexCoords Coords_Furnace = new TexCoords { front = furnace_front, right = furnace_side, left = furnace_side, back = furnace_side, top = furnace_top, bottom = furnace_top };
    static TexCoords Coords_HayBlock = new TexCoords { front = hay_side, right = hay_side, left = hay_side, back = hay_side, top = hay_top, bottom = hay_top };
    static TexCoords Coords_Leaves = new TexCoords { front = leaves_side, right = leaves_side, left = leaves_side, back = leaves_side, top = leaves_side, bottom = leaves_side };
    static TexCoords Coords_Grass = new TexCoords { isPlant = true, front = uv_grass };
    static TexCoords Coords_Stone = new TexCoords { front = uv_stone, right = uv_stone, left = uv_stone, back = uv_stone, top = uv_stone, bottom = uv_stone };
    static TexCoords Coords_BedRock = new TexCoords { front = uv_bedrock, right = uv_bedrock, left = uv_bedrock, back = uv_bedrock, top = uv_bedrock, bottom = uv_bedrock };
    static TexCoords Coords_Poppy = new TexCoords { isPlant = true, front = uv_poppy };
    static TexCoords Coords_Dandelion = new TexCoords { isPlant = true, front = uv_dandelion };
    static TexCoords Coords_OakWood = new TexCoords { front = uv_oakwood_side, right = uv_oakwood_side, left = uv_oakwood_side, back = uv_oakwood_side, top = uv_oakwood_top, bottom = uv_oakwood_top };
    static TexCoords Coords_OakLeaves = new TexCoords { front = uv_oakleaves, right = uv_oakleaves, left = uv_oakleaves, back = uv_oakleaves, top = uv_oakleaves, bottom = uv_oakleaves };

    public static TexCoords[] type2texcoords = new TexCoords[15]
    {
        Coords_None,
        Coords_Dirt,
        Coords_GrassBlock,
        Coords_Tnt,
        Coords_Brick,
        Coords_Furnace,
        Coords_HayBlock,
        Coords_Leaves,
        Coords_Grass,
        Coords_Stone,
        Coords_BedRock,
        Coords_Poppy,
        Coords_Dandelion,
        Coords_OakWood,
        Coords_OakLeaves,
    };

    static Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    static Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    static Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    static Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    static Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    static Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    static Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    static Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    static void AddUV(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector2 texPos)
    {
        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        uv.Add(new Vector2(texPos.x / atlas_column, texPos.y / atlas_row));
        uv.Add(new Vector2(texPos.x / atlas_column, (texPos.y + 1) / atlas_row));
        uv.Add(new Vector2((texPos.x + 1) / atlas_column, texPos.y / atlas_row));
        uv.Add(new Vector2((texPos.x + 1) / atlas_column, (texPos.y + 1) / atlas_row));

        int verticesCount = vertices.Count;
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 1);
        triangles.Add(verticesCount - 2);
    }

    static void AddUV_BackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector2 texPos)
    {
        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        uv.Add(new Vector2(texPos.x / atlas_column, texPos.y / atlas_row));
        uv.Add(new Vector2(texPos.x / atlas_column, (texPos.y + 1) / atlas_row));
        uv.Add(new Vector2((texPos.x + 1) / atlas_column, texPos.y / atlas_row));
        uv.Add(new Vector2((texPos.x + 1) / atlas_column, (texPos.y + 1) / atlas_row));

        int verticesCount = vertices.Count;
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);
    }

    // vertex order
    // 1 3
    // 0 2 
    // 1st tri:0 1 2
    // 2nd tri:1 3 2
    static void AddDiagonalFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearBottomRight + pos);
        vertices.Add(nearTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearBottomRight + pos);
        vertices.Add(nearTopRight + pos);
        AddUV_BackFace(vertices, uv, triangles, texPos);
    }

    static void AddAntiDiagonalFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        AddUV_BackFace(vertices, uv, triangles, texPos);
    }

    static void AddFrontFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(nearBottomRight + pos);
        vertices.Add(nearTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddRightFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearBottomRight + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddLeftFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddBackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddTopFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(farTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddBottomFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3 pos, Vector2 texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(farBottomRight + pos);
        vertices.Add(nearBottomRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    public static Mesh GetCubeMesh(CSBlockType type)
    {
        Mesh mesh = new Mesh();
        mesh.name = "CubeMesh";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        TexCoords texCoords = type2texcoords[(byte)type];
        Vector3 pos = Vector3.zero;
        if (texCoords.isPlant)
        {
            AddDiagonalFace(vertices, uv, triangles, pos, texCoords.front);
            AddAntiDiagonalFace(vertices, uv, triangles, pos, texCoords.front);
        }
        else
        {
            AddFrontFace(vertices, uv, triangles, pos, texCoords.front);
            AddRightFace(vertices, uv, triangles, pos, texCoords.right);
            AddLeftFace(vertices, uv, triangles, pos, texCoords.left);
            AddBackFace(vertices, uv, triangles, pos, texCoords.back);
            AddTopFace(vertices, uv, triangles, pos, texCoords.top);
            AddBottomFace(vertices, uv, triangles, pos, texCoords.bottom);
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }
    
    public static void GenerateMeshData(Chunk chunk)
    {
        //Mesh mesh = new Mesh();
        //mesh.name = "ChunkMesh";

        List<Vector3> vertices = chunk.vertices;
        List<Vector2> uv = chunk.uv;
        List<int> triangles = chunk.triangles;

        vertices.Clear();
        uv.Clear();
        triangles.Clear();
        
        List<Vector3> plantVertices = chunk.plantVertices;
        List<Vector2> plantUV = chunk.plantUV;
        List<int> plantTriangles = chunk.plantTriangles;

        plantVertices.Clear();
        plantUV.Clear();
        plantTriangles.Clear();

        Vector3 pos = new Vector3();
        //压缩后的数据结构
        for (int k = 0; k < 256; k++)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    byte b = chunk.blocksInByte[256 * k + 16 * i + j];
                    CSBlockType type = (CSBlockType)b;
                    if (type != CSBlockType.None)
                    {
                        pos.Set(chunk.x * 16 + i, k, chunk.z * 16 + j);
                        TexCoords texCoords = type2texcoords[(byte)type];

                        if (texCoords.isPlant)
                        {
                            AddDiagonalFace(plantVertices, plantUV, plantTriangles, pos, texCoords.front);
                            AddAntiDiagonalFace(plantVertices, plantUV, plantTriangles, pos, texCoords.front);
                        }
                        else
                        {
                            if (!chunk.HasBlock(i, k, j - 1))
                            {
                                AddFrontFace(vertices, uv, triangles, pos, texCoords.front);
                            }
                            if (!chunk.HasBlock(i + 1, k, j))
                            {
                                AddRightFace(vertices, uv, triangles, pos, texCoords.right);
                            }
                            if (!chunk.HasBlock(i - 1, k, j))
                            {
                                AddLeftFace(vertices, uv, triangles, pos, texCoords.left);
                            }
                            if (!chunk.HasBlock(i, k, j + 1))
                            {
                                AddBackFace(vertices, uv, triangles, pos, texCoords.back);
                            }
                            if (!chunk.HasBlock(i, k + 1, j))
                            {
                                AddTopFace(vertices, uv, triangles, pos, texCoords.top);
                            }
                            if (!chunk.HasBlock(i, k - 1, j))
                            {
                                AddBottomFace(vertices, uv, triangles, pos, texCoords.bottom);
                            }
                        }
                    }
                }
            }
        }

        //Debug.Log("vertices Capacity=" + vertices.Capacity+ ",uv Capacity=" + uv.Capacity + ",triangles Capacity=" + triangles.Capacity);
        //mesh.vertices = vertices.ToArray();
        //mesh.uv = uv.ToArray();
        //mesh.triangles = triangles.ToArray();

        //return mesh;
    }
}
