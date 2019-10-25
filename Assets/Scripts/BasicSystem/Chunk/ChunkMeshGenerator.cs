using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using protocol.cs_theircraft;

public class ChunkMeshGenerator : MonoBehaviour
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

    //方块图集的行数和列数
    static readonly int atlas_row = 58;
    static readonly int atlas_column = 24;

    //左上为0，0
    static Vector2Int grass_bottom = new Vector2Int(2, 0);
    static Vector2Int grass_side = new Vector2Int(3, 0);
    static Vector2Int grass_top = new Vector2Int(0, 0);

    static Vector2Int brick = new Vector2Int(7, 0);

    static Vector2Int tnt_side = new Vector2Int(8, 0);
    static Vector2Int tnt_top = new Vector2Int(9, 0);
    static Vector2Int tnt_bottom = new Vector2Int(10, 0);

    static Vector2Int furnace_front = new Vector2Int(12, 2);
    static Vector2Int furnace_side = new Vector2Int(13, 2);
    static Vector2Int furnace_top = new Vector2Int(14, 3);

    static Vector2Int hay_side = new Vector2Int(16, 13);
    static Vector2Int hay_top = new Vector2Int(17, 13);

    static TexCoords Coords_None = new TexCoords();
    static TexCoords Coords_Dirt = new TexCoords { front = grass_bottom, right = grass_bottom, left = grass_bottom, back = grass_bottom, top = grass_bottom, bottom = grass_bottom };
    static TexCoords Coords_Grass = new TexCoords { front = grass_side, right = grass_side, left = grass_side, back = grass_side, top = grass_top, bottom = grass_bottom };
    static TexCoords Coords_Tnt = new TexCoords { front = tnt_side, right = tnt_side, left = tnt_side, back = tnt_side, top = tnt_top, bottom = tnt_bottom };
    static TexCoords Coords_Brick = new TexCoords { front = brick, right = brick, left = brick, back = brick, top = brick, bottom = brick };
    static TexCoords Coords_Furnace = new TexCoords { front = furnace_front, right = furnace_side, left = furnace_side, back = furnace_side, top = furnace_top, bottom = furnace_top };
    static TexCoords Coords_HayBlock = new TexCoords { front = hay_side, right = hay_side, left = hay_side, back = hay_side, top = hay_top, bottom = hay_top };

    public static TexCoords[] type2texcoords = new TexCoords[7]
    {
        Coords_None,
        Coords_Dirt,
        Coords_Grass,
        Coords_Tnt,
        Coords_Brick,
        Coords_Furnace,
        Coords_HayBlock
    };

    static Vector3 nearBottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
    static Vector3 nearBottomRight = new Vector3(0.5f, -0.5f, -0.5f);
    static Vector3 nearTopLeft = new Vector3(-0.5f, 0.5f, -0.5f);
    static Vector3 nearTopRight = new Vector3(0.5f, 0.5f, -0.5f);
    static Vector3 farBottomLeft = new Vector3(-0.5f, -0.5f, 0.5f);
    static Vector3 farBottomRight = new Vector3(0.5f, -0.5f, 0.5f);
    static Vector3 farTopLeft = new Vector3(-0.5f, 0.5f, 0.5f);
    static Vector3 farTopRight = new Vector3(0.5f, 0.5f, 0.5f);

    static void AddUV(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector2Int texPos)
    {
        //上下翻转
        texPos.y = (atlas_row - 1) - texPos.y;

        uv.Add(new Vector2(texPos.x / (float)atlas_column, texPos.y / (float)atlas_row));
        uv.Add(new Vector2(texPos.x / (float)atlas_column, (texPos.y + 1) / (float)atlas_row));
        uv.Add(new Vector2((texPos.x + 1) / (float)atlas_column, texPos.y / (float)atlas_row));
        uv.Add(new Vector2((texPos.x + 1) / (float)atlas_column, (texPos.y + 1) / (float)atlas_row));
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
    }

    // vertex order
    // 1 3
    // 0 2 
    static void AddFrontFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2Int texPos)
    {
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        vertices.Add(nearBottomRight + pos);
        vertices.Add(nearTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddRightFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2Int texPos)
    {
        vertices.Add(nearBottomRight + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddLeftFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2Int texPos)
    {
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearBottomLeft + pos);
        vertices.Add(nearTopLeft + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddBackFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2Int texPos)
    {
        vertices.Add(farBottomRight + pos);
        vertices.Add(farTopRight + pos);
        vertices.Add(farBottomLeft + pos);
        vertices.Add(farTopLeft + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddTopFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2Int texPos)
    {
        vertices.Add(nearTopLeft + pos);
        vertices.Add(farTopLeft + pos);
        vertices.Add(nearTopRight + pos);
        vertices.Add(farTopRight + pos);
        AddUV(vertices, uv, triangles, texPos);
    }

    static void AddBottomFace(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector3Int pos, Vector2Int texPos)
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
        Vector3Int pos = Vector3Int.zero;
        AddFrontFace(vertices, uv, triangles, pos, texCoords.front);
        AddRightFace(vertices, uv, triangles, pos, texCoords.right);
        AddLeftFace(vertices, uv, triangles, pos, texCoords.left);
        AddBackFace(vertices, uv, triangles, pos, texCoords.back);
        AddTopFace(vertices, uv, triangles, pos, texCoords.top);
        AddBottomFace(vertices, uv, triangles, pos, texCoords.bottom);

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }

    static Vector3Int Vector3Int_forward = new Vector3Int(0, 0, 1);
    static Vector3Int Vector3Int_back = new Vector3Int(0, 0, -1);

    public static Mesh GenerateMesh(Chunk chunk)
    {
        Mesh mesh = new Mesh();
        mesh.name = "ChunkMesh";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        Vector3Int pos = new Vector3Int();
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

        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }
}
