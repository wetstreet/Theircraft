using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public static class ChunkMeshGenerator
{
    public struct TexCoords
    {
        public bool isTransparent;
        public bool isCollidable;
        public Vector2Int front;
        public Vector2Int right;
        public Vector2Int left;
        public Vector2Int back;
        public Vector2Int top;
        public Vector2Int bottom;
    }

    //方块图集的行数和列数
    static readonly int atlas_row = 58;
    static readonly int atlas_column = 24;
    
    //左上为0，0
    public static Vector2Int grass_bottom = new Vector2Int(2, 0);
    public static Vector2Int grass_side = new Vector2Int(3, 0);
    public static Vector2Int grass_top = new Vector2Int(0, 0);

    public static Vector2Int uv_stone = new Vector2Int(1, 0);

    public static Vector2Int brick = new Vector2Int(7, 0);

    public static Vector2Int tnt_side = new Vector2Int(8, 0);
    public static Vector2Int tnt_top = new Vector2Int(9, 0);
    public static Vector2Int tnt_bottom = new Vector2Int(10, 0);

    public static Vector2Int furnace_front = new Vector2Int(12, 2);
    public static Vector2Int furnace_side = new Vector2Int(13, 2);
    public static Vector2Int furnace_top = new Vector2Int(14, 3);

    public static Vector2Int hay_side = new Vector2Int(16, 13);
    public static Vector2Int hay_top = new Vector2Int(17, 13);

    public static Vector2Int leaves_side = new Vector2Int(4, 12);

    public static Vector2Int uv_grass = new Vector2Int(8, 2);

    public static Vector2Int uv_bedrock = new Vector2Int(1, 1);
    public static Vector2Int uv_poppy = new Vector2Int(12, 0);
    public static Vector2Int uv_dandelion = new Vector2Int(13, 0);
    public static Vector2Int uv_oakwood_side = new Vector2Int(4, 1);
    public static Vector2Int uv_oakwood_top = new Vector2Int(5, 1);
    public static Vector2Int uv_oakleaves = new Vector2Int(4, 3);

    public static TexCoords Coords_None = new TexCoords();
    public static TexCoords Coords_Dirt = new TexCoords { isTransparent = false, isCollidable = true, front = grass_bottom, right = grass_bottom, left = grass_bottom, back = grass_bottom, top = grass_bottom, bottom = grass_bottom };
    public static TexCoords Coords_GrassBlock = new TexCoords { isTransparent = false, isCollidable = true, front = grass_side, right = grass_side, left = grass_side, back = grass_side, top = grass_top, bottom = grass_bottom };
    public static TexCoords Coords_Tnt = new TexCoords { isTransparent = false, isCollidable = true, front = tnt_side, right = tnt_side, left = tnt_side, back = tnt_side, top = tnt_top, bottom = tnt_bottom };
    public static TexCoords Coords_Brick = new TexCoords { isTransparent = false, isCollidable = true, front = brick, right = brick, left = brick, back = brick, top = brick, bottom = brick };
    public static TexCoords Coords_Furnace = new TexCoords { isTransparent = false, isCollidable = true, front = furnace_front, right = furnace_side, left = furnace_side, back = furnace_side, top = furnace_top, bottom = furnace_top };
    public static TexCoords Coords_HayBlock = new TexCoords { isTransparent = false, isCollidable = true, front = hay_side, right = hay_side, left = hay_side, back = hay_side, top = hay_top, bottom = hay_top };
    public static TexCoords Coords_Leaves = new TexCoords { isTransparent = false, isCollidable = true, front = leaves_side, right = leaves_side, left = leaves_side, back = leaves_side, top = leaves_side, bottom = leaves_side };
    public static TexCoords Coords_Grass = new TexCoords { isTransparent = true, isCollidable = false, front = uv_grass };
    public static TexCoords Coords_Stone = new TexCoords { isTransparent = false, isCollidable = true, front = uv_stone, right = uv_stone, left = uv_stone, back = uv_stone, top = uv_stone, bottom = uv_stone };
    public static TexCoords Coords_BedRock = new TexCoords { isTransparent = false, isCollidable = true, front = uv_bedrock, right = uv_bedrock, left = uv_bedrock, back = uv_bedrock, top = uv_bedrock, bottom = uv_bedrock };
    public static TexCoords Coords_Poppy = new TexCoords { isTransparent = true, isCollidable = false, front = uv_poppy };
    public static TexCoords Coords_Dandelion = new TexCoords { isTransparent = true, isCollidable = false, front = uv_dandelion };
    public static TexCoords Coords_OakWood = new TexCoords { isTransparent = false, isCollidable = true, front = uv_oakwood_side, right = uv_oakwood_side, left = uv_oakwood_side, back = uv_oakwood_side, top = uv_oakwood_top, bottom = uv_oakwood_top };
    public static TexCoords Coords_OakLeaves = new TexCoords { isTransparent = true, isCollidable = true, front = uv_oakleaves, right = uv_oakleaves, left = uv_oakleaves, back = uv_oakleaves, top = uv_oakleaves, bottom = uv_oakleaves };

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

    // because we pack textures so tightly, we need to manipulate uv coords slightly when sampling the texture.
    static readonly float compensation = 0.01f;
    static readonly float compensation_x = compensation / atlas_column;
    static readonly float compensation_y = compensation / atlas_row;

    static void AddUV(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector2 texPos)
    {
        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        // bottom left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, texPos.y / atlas_row + compensation_y));
        // top left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // bottom right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, texPos.y / atlas_row + compensation_y));
        // top right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 1) / atlas_row - compensation_y));

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

        // bottom left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, texPos.y / atlas_row + compensation_y));
        // top left
        uv.Add(new Vector2(texPos.x / atlas_column + compensation_x, (texPos.y + 1) / atlas_row - compensation_y));
        // bottom right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, texPos.y / atlas_row + compensation_y));
        // top right
        uv.Add(new Vector2((texPos.x + 1) / atlas_column - compensation_x, (texPos.y + 1) / atlas_row - compensation_y));

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
        if (!texCoords.isCollidable)
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
    
    public static void RefreshMeshData(this Chunk chunk)
    {
        chunk.vertices1.Clear();
        chunk.uv1.Clear();
        chunk.triangles1.Clear();

        chunk.vertices2.Clear();
        chunk.uv2.Clear();
        chunk.triangles2.Clear();

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
                        pos.Set(i, k, j);
                        TexCoords texCoords = type2texcoords[(byte)type];

                        if (!texCoords.isCollidable)
                        {
                            AddDiagonalFace(chunk.vertices2, chunk.uv2, chunk.triangles2, pos, texCoords.front);
                            AddAntiDiagonalFace(chunk.vertices2, chunk.uv2, chunk.triangles2, pos, texCoords.front);
                        }
                        else
                        {
                            if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(chunk.globalX + i, k, chunk.globalZ + j - 1))
                            {
                                AddFrontFace(chunk.vertices1, chunk.uv1, chunk.triangles1, pos, texCoords.front);
                            }
                            if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(chunk.globalX + i + 1, k, chunk.globalZ + j))
                            {
                                AddRightFace(chunk.vertices1, chunk.uv1, chunk.triangles1, pos, texCoords.right);
                            }
                            if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(chunk.globalX + i - 1, k, chunk.globalZ + j))
                            {
                                AddLeftFace(chunk.vertices1, chunk.uv1, chunk.triangles1, pos, texCoords.left);
                            }
                            if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(chunk.globalX + i, k, chunk.globalZ + j + 1))
                            {
                                AddBackFace(chunk.vertices1, chunk.uv1, chunk.triangles1, pos, texCoords.back);
                            }
                            if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(chunk.globalX + i, k + 1, chunk.globalZ + j))
                            {
                                AddTopFace(chunk.vertices1, chunk.uv1, chunk.triangles1, pos, texCoords.top);
                            }
                            if (texCoords.isTransparent || !ChunkManager.HasOpaqueBlock(chunk.globalX + i, k - 1, chunk.globalZ + j))
                            {
                                AddBottomFace(chunk.vertices1, chunk.uv1, chunk.triangles1, pos, texCoords.bottom);
                            }
                        }
                    }
                }
            }
        }
    }
}
