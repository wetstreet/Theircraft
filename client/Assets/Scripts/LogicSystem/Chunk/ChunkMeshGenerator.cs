using UnityEngine;
using protocol.cs_theircraft;

public struct TexCoords
{
    public bool isTransparent;
    public bool isCollidable;
    public bool isRotatable;
    public bool isPlant;
    public Vector2Int front;
    public Vector2Int right;
    public Vector2Int left;
    public Vector2Int back;
    public Vector2Int top;
    public Vector2Int bottom;
}

public static class ChunkMeshGenerator
{
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

    public static Vector2Int uv_torch = new Vector2Int(0, 5);

    public static TexCoords Coords_None = new TexCoords();
    public static TexCoords Coords_Dirt = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = false, front = grass_bottom, right = grass_bottom, left = grass_bottom, back = grass_bottom, top = grass_bottom, bottom = grass_bottom };
    public static TexCoords Coords_GrassBlock = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = false, front = grass_side, right = grass_side, left = grass_side, back = grass_side, top = grass_top, bottom = grass_bottom };
    public static TexCoords Coords_Tnt = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = false, front = tnt_side, right = tnt_side, left = tnt_side, back = tnt_side, top = tnt_top, bottom = tnt_bottom };
    public static TexCoords Coords_Brick = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = false, front = brick, right = brick, left = brick, back = brick, top = brick, bottom = brick };
    public static TexCoords Coords_BrickStairs = new TexCoords { isPlant = false, isTransparent = true, isCollidable = true, isRotatable = true, front = brick };
    public static TexCoords Coords_Furnace = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = true, front = furnace_front, right = furnace_side, left = furnace_side, back = furnace_side, top = furnace_top, bottom = furnace_top };
    public static TexCoords Coords_HayBlock = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = false, front = hay_side, right = hay_side, left = hay_side, back = hay_side, top = hay_top, bottom = hay_top };
    public static TexCoords Coords_Leaves = new TexCoords { isPlant = false, isTransparent = true, isCollidable = true, isRotatable = false, front = leaves_side, right = leaves_side, left = leaves_side, back = leaves_side, top = leaves_side, bottom = leaves_side };
    public static TexCoords Coords_Grass = new TexCoords { isPlant = true, isTransparent = true, isCollidable = false, isRotatable = false, front = uv_grass };
    public static TexCoords Coords_Stone = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = false, front = uv_stone, right = uv_stone, left = uv_stone, back = uv_stone, top = uv_stone, bottom = uv_stone };
    public static TexCoords Coords_BedRock = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = false, front = uv_bedrock, right = uv_bedrock, left = uv_bedrock, back = uv_bedrock, top = uv_bedrock, bottom = uv_bedrock };
    public static TexCoords Coords_Poppy = new TexCoords { isPlant = true, isTransparent = true, isCollidable = false, isRotatable = false, front = uv_poppy };
    public static TexCoords Coords_Dandelion = new TexCoords { isPlant = true, isTransparent = true, isCollidable = false, isRotatable = false, front = uv_dandelion };
    public static TexCoords Coords_OakWood = new TexCoords { isPlant = false, isTransparent = false, isCollidable = true, isRotatable = false, front = uv_oakwood_side, right = uv_oakwood_side, left = uv_oakwood_side, back = uv_oakwood_side, top = uv_oakwood_top, bottom = uv_oakwood_top };
    public static TexCoords Coords_OakLeaves = new TexCoords { isPlant = false, isTransparent = true, isCollidable = true, isRotatable = false, front = uv_oakleaves, right = uv_oakleaves, left = uv_oakleaves, back = uv_oakleaves, top = uv_oakleaves, bottom = uv_oakleaves };
    public static TexCoords Coords_Torch = new TexCoords { isPlant = false, isTransparent = true, isCollidable = false, isRotatable = false, front = uv_torch };

    public static TexCoords[] type2texcoords = new TexCoords[17]
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
        Coords_BrickStairs,
        Coords_Torch,
    };

    public static Mesh GetCubeMesh(CSBlockType type)
    {
        TexCoords texCoords = type2texcoords[(byte)type];

        if (texCoords.isPlant)
        {
            return PlantMeshGenerator.Instance.GenerateSingleMesh(type);
        }
        else
        {
            if (type == CSBlockType.BrickStairs)
            {
                return StairMeshGenerator.Instance.GenerateSingleMesh(type);
            }
            else if (type == CSBlockType.Torch)
            {
                return TorchMeshGenerator.Instance.GenerateSingleMesh(type);
            }
            else
            {
                return BlockMeshGenerator.Instance.GenerateSingleMesh(type);
            }
        }
    }

    public static void RefreshMeshData(this Chunk chunk)
    {
        chunk.vertices1.Clear();
        chunk.uv1.Clear();
        chunk.triangles1.Clear();

        chunk.vertices2.Clear();
        chunk.uv2.Clear();
        chunk.triangles2.Clear();

        Vector3Int pos = new Vector3Int();
        Vector3Int globalPos = new Vector3Int();
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
                        TexCoords texCoords = type2texcoords[(byte)type];

                        pos.Set(i, k, j);
                        globalPos.Set(chunk.globalX + i, k, chunk.globalZ + j);

                        if (texCoords.isPlant)
                        {
                            PlantMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices2, chunk.uv2, chunk.triangles2);
                        }
                        else
                        {
                            if (type == CSBlockType.BrickStairs)
                            {
                                StairMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices1, chunk.uv1, chunk.triangles1);
                            }
                            else if (type == CSBlockType.Torch)
                            {
                                TorchMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices2, chunk.uv2, chunk.triangles2);
                            }
                            else
                            {
                                BlockMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices1, chunk.uv1, chunk.triangles1);
                            }
                        }
                    }
                }
            }
        }
    }

    public static CSBlockOrientation GetBlockOrientation(Vector3 playerPos, Vector3 blockPos, Vector3 hitPos)
    {
        float diff = hitPos.y - blockPos.y;

        Vector2 dir = (new Vector2(playerPos.x, playerPos.z) - new Vector2(blockPos.x, blockPos.z)).normalized;
        CSBlockOrientation orient = CSBlockOrientation.Default;
        if (dir.x > 0)
        {
            if (dir.y > 0)
            {
                if (dir.y > dir.x)
                {
                    orient = diff < 0 ? CSBlockOrientation.PositiveY_PositiveZ : CSBlockOrientation.NegativeY_PositiveZ;
                }
                else
                {
                    orient = diff < 0 ? CSBlockOrientation.PositiveY_PositiveX : CSBlockOrientation.NegativeY_PositiveX;
                }
            }
            else
            {
                if (-dir.y > dir.x)
                {
                    orient = diff < 0 ? CSBlockOrientation.PositiveY_NegativeZ : CSBlockOrientation.NegativeY_NegativeZ;
                }
                else
                {
                    orient = diff < 0 ? CSBlockOrientation.PositiveY_PositiveX : CSBlockOrientation.NegativeY_PositiveX;
                }
            }
        }
        else
        {
            if (dir.y > 0)
            {
                if (dir.y > -dir.x)
                {
                    orient = diff < 0 ? CSBlockOrientation.PositiveY_PositiveZ : CSBlockOrientation.NegativeY_PositiveZ;
                }
                else
                {
                    orient = diff < 0 ? CSBlockOrientation.PositiveY_NegativeX : CSBlockOrientation.NegativeY_NegativeX;
                }
            }
            else
            {
                if (-dir.y > -dir.x)
                {
                    orient = diff < 0 ? CSBlockOrientation.PositiveY_NegativeZ : CSBlockOrientation.NegativeY_NegativeZ;
                }
                else
                {
                    orient = diff < 0 ? CSBlockOrientation.PositiveY_NegativeX : CSBlockOrientation.NegativeY_NegativeX;
                }
            }
        }
        return orient;
    }
}
