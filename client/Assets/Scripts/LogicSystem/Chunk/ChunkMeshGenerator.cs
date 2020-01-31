using UnityEngine;
using protocol.cs_theircraft;

public class TexCoords
{
    public bool isTransparent = false;  // side blocks don't cull face
    public bool isCollidable = true;    // colliding with player
    public bool isRotatable = false;    // stores orientation
    public bool isPlant = false;        // uses plant mesh generator
    public bool isStair = false;        // uses stair mesh generator

    public Vector2Int front;
    public Vector2Int right;
    public Vector2Int left;
    public Vector2Int back;
    public Vector2Int top;
    public Vector2Int bottom;

    private TexCoords() { }

    public static TexCoords None()
    {
        return new TexCoords();
    }
    
    public static TexCoords Block_1(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = false,
            isPlant = false,
            isRotatable = false,
            isCollidable = true,
            front = uv,
            right = uv,
            left = uv,
            back = uv,
            top = uv,
            bottom = uv
        };
    }

    public static TexCoords Block_1_transparent(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isPlant = false,
            isRotatable = false,
            isCollidable = true,
            front = uv,
            right = uv,
            left = uv,
            back = uv,
            top = uv,
            bottom = uv
        };
    }

    public static TexCoords Block_front_polar_side(Vector2Int front, Vector2Int polar, Vector2Int side)
    {
        return new TexCoords
        {
            isTransparent = false,
            isPlant = false,
            isRotatable = false,
            isCollidable = true,
            front = front,
            right = side,
            left = side,
            back = side,
            top = polar,
            bottom = polar
        };
    }

    public static TexCoords Block_polar_side(Vector2Int polar, Vector2Int side)
    {
        return new TexCoords
        {
            isTransparent = false,
            isPlant = false,
            isRotatable = false,
            isCollidable = true,
            front = side,
            right = side,
            left = side,
            back = side,
            top = polar,
            bottom = polar
        };
    }

    public static TexCoords Block_top_bottom_side(Vector2Int top, Vector2Int bottom, Vector2Int side)
    {
        return new TexCoords
        {
            isTransparent = false,
            isPlant = false,
            isRotatable = false,
            isCollidable = true,
            front = side,
            right = side,
            left = side,
            back = side,
            top = top,
            bottom = bottom
        };
    }

    public static TexCoords Plant(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isPlant = true,
            front = uv
        };
    }

    public static TexCoords Stair(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isRotatable = true,
            isStair = true,
            front = uv
        };
    }

    public static TexCoords Wall(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            front = uv
        };
    }

    public static TexCoords Torch(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isCollidable = false,
            front = uv
        };
    }
}

public static class ChunkMeshGenerator
{
    // face uv list, top left corner is (0, 0)

    // first row
    public static Vector2Int uv_grassblock_top = new Vector2Int(0, 0);
    public static Vector2Int uv_stone = new Vector2Int(1, 0);
    public static Vector2Int uv_dirt = new Vector2Int(2, 0);
    public static Vector2Int uv_grassblock_side = new Vector2Int(3, 0);
    public static Vector2Int uv_oak_planks = new Vector2Int(4, 0);
    public static Vector2Int uv_double_stone_slab_side = new Vector2Int(5, 0);
    public static Vector2Int uv_double_stone_slab_top = new Vector2Int(6, 0);
    public static Vector2Int uv_bricks = new Vector2Int(7, 0);
    public static Vector2Int uv_tnt_side = new Vector2Int(8, 0);
    public static Vector2Int uv_tnt_top = new Vector2Int(9, 0);
    public static Vector2Int uv_tnt_bottom = new Vector2Int(10, 0);
    public static Vector2Int uv_web = new Vector2Int(11, 0);
    public static Vector2Int uv_poppy = new Vector2Int(12, 0);
    public static Vector2Int uv_dandelion = new Vector2Int(13, 0);
    public static Vector2Int uv_red_sand = new Vector2Int(14, 0);
    public static Vector2Int uv_oak_sapling = new Vector2Int(15, 0);

    // second row
    public static Vector2Int uv_cobblestone = new Vector2Int(0, 1);
    public static Vector2Int uv_bedrock = new Vector2Int(1, 1);
    public static Vector2Int uv_sand = new Vector2Int(2, 1);
    public static Vector2Int uv_gravel = new Vector2Int(3, 1);
    public static Vector2Int uv_oaklog_side = new Vector2Int(4, 1);
    public static Vector2Int uv_oaklog_top = new Vector2Int(5, 1);
    public static Vector2Int uv_iron_block = new Vector2Int(6, 1);
    public static Vector2Int uv_gold_block = new Vector2Int(7, 1);
    public static Vector2Int uv_diamond_block = new Vector2Int(8, 1);
    public static Vector2Int uv_emerald_block = new Vector2Int(9, 1);
    public static Vector2Int uv_redstone_block = new Vector2Int(10, 1);
    public static Vector2Int uv_red_mushroom = new Vector2Int(12, 1);
    public static Vector2Int uv_brown_mushroom = new Vector2Int(13, 1);
    public static Vector2Int uv_jungle_sapling = new Vector2Int(14, 1);

    // third row
    public static Vector2Int uv_gold_ore = new Vector2Int(0, 2);
    public static Vector2Int uv_iron_ore = new Vector2Int(1, 2);
    public static Vector2Int uv_coal_ore = new Vector2Int(2, 2);
    public static Vector2Int uv_bookshelf = new Vector2Int(3, 2);
    public static Vector2Int uv_mossy_cobblestone = new Vector2Int(4, 2);
    public static Vector2Int uv_obsidian = new Vector2Int(5, 2);
    public static Vector2Int uv_grass = new Vector2Int(8, 2);
    public static Vector2Int uv_redstone_wire_sw = new Vector2Int(9, 2);
    public static Vector2Int uv_daylight_detector_inverted = new Vector2Int(10, 2);
    public static Vector2Int uv_crafting_table_top = new Vector2Int(11, 2);
    public static Vector2Int uv_furnace_front = new Vector2Int(12, 2);
    public static Vector2Int uv_furnace_side = new Vector2Int(13, 2);
    public static Vector2Int uv_dispenser = new Vector2Int(14, 2);
    public static Vector2Int uv_dropper = new Vector2Int(15, 2);
    public static Vector2Int uv_redstone_torch = new Vector2Int(16, 2);

    // fourth row
    public static Vector2Int uv_sponge = new Vector2Int(0, 3);
    public static Vector2Int uv_glass = new Vector2Int(1, 3);
    public static Vector2Int uv_diamond_ore = new Vector2Int(2, 3);
    public static Vector2Int uv_redstone_ore = new Vector2Int(3, 3);
    public static Vector2Int uv_oakleaves = new Vector2Int(4, 3);
    public static Vector2Int uv_oakleaves_fast = new Vector2Int(5, 3);
    public static Vector2Int uv_stonebrick = new Vector2Int(6, 3);
    public static Vector2Int uv_deadbush = new Vector2Int(7, 3);
    public static Vector2Int uv_fern = new Vector2Int(8, 3);
    public static Vector2Int uv_daylight_detector = new Vector2Int(9, 3);
    public static Vector2Int uv_daylight_detector_bottom = new Vector2Int(10, 3);
    public static Vector2Int uv_crafting_table_side_1 = new Vector2Int(11, 2);
    public static Vector2Int uv_crafting_table_side_2 = new Vector2Int(12, 2);
    public static Vector2Int uv_lit_furnace_front = new Vector2Int(13, 3);
    public static Vector2Int uv_furnace_top = new Vector2Int(14, 3);
    public static Vector2Int uv_spruce_sapling = new Vector2Int(15, 3);

    // fifth row
    public static Vector2Int uv_white_wool = new Vector2Int(0, 4);
    public static Vector2Int uv_snow = new Vector2Int(2, 4);
    public static Vector2Int uv_ice = new Vector2Int(3, 4);
    public static Vector2Int uv_snowy_grassblock_side = new Vector2Int(4, 4);
    public static Vector2Int uv_cactus_top = new Vector2Int(5, 4);
    public static Vector2Int uv_cactus_side = new Vector2Int(6, 4);
    public static Vector2Int uv_clay = new Vector2Int(8, 4);
    public static Vector2Int uv_sugar_cane = new Vector2Int(9, 4);
    public static Vector2Int uv_noteblock = new Vector2Int(10, 4);
    public static Vector2Int uv_jukebox = new Vector2Int(11, 4);
    public static Vector2Int uv_waterlily = new Vector2Int(12, 4);
    public static Vector2Int uv_mycelium_side = new Vector2Int(13, 4);
    public static Vector2Int uv_mycelium_top = new Vector2Int(14, 4);
    public static Vector2Int uv_birch_sapling = new Vector2Int(15, 4);
    public static Vector2Int uv_piston_w = new Vector2Int(16, 4);
    public static Vector2Int uv_piston_s = new Vector2Int(17, 4);
    public static Vector2Int uv_piston_e = new Vector2Int(18, 4);
    public static Vector2Int uv_piston_n = new Vector2Int(19, 4);

    public static Vector2Int uv_torch = new Vector2Int(0, 5);

    public static Vector2Int leaves_side = new Vector2Int(4, 12);
    public static Vector2Int hay_side = new Vector2Int(16, 13);
    public static Vector2Int hay_top = new Vector2Int(17, 13);

    public static TexCoords[] type2texcoords = new TexCoords[19]
    {
        // None
        TexCoords.None(),
        // Dirt
        TexCoords.Block_1(uv_dirt),
        // GrassBlock
        TexCoords.Block_top_bottom_side(uv_grassblock_top, uv_dirt, uv_grassblock_side),
        // Tnt
        TexCoords.Block_top_bottom_side(uv_tnt_top, uv_tnt_bottom, uv_tnt_side),
        // Brick
        TexCoords.Block_1(uv_bricks),
        // Furnace
        TexCoords.Block_front_polar_side(uv_furnace_front, uv_furnace_top, uv_furnace_side),
        // HayBlock
        TexCoords.Block_polar_side(hay_top, hay_side),
        // Leaves
        TexCoords.Block_1_transparent(leaves_side),
        // Grass
        TexCoords.Plant(uv_grass),
        // Stone
        TexCoords.Block_1(uv_stone),
        // BedRock
        TexCoords.Block_1(uv_bedrock),
        // Poppy
        TexCoords.Plant(uv_poppy),
        // Dandelion
        TexCoords.Plant(uv_dandelion),
        // OakLog
        TexCoords.Block_polar_side(uv_oaklog_top, uv_oaklog_side),
        // OakLeaves
        TexCoords.Block_1_transparent(uv_oakleaves),
        // BrickStairs
        TexCoords.Stair(uv_bricks),
        // Torch
        TexCoords.Torch(uv_torch),
        // BrickWall
        TexCoords.Wall(uv_bricks),
        // OakPlanks
        TexCoords.Block_1(uv_oak_planks),
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
            else if (type == CSBlockType.BrickWall)
            {
                return WallMeshGenerator.Instance.GenerateSingleMesh(type);
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
                            else if (type == CSBlockType.BrickWall)
                            {
                                WallMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices1, chunk.uv1, chunk.triangles1);
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
