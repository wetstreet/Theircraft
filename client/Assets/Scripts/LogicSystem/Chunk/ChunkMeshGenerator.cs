using UnityEngine;
using protocol.cs_theircraft;
using System.Collections.Generic;

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
    public static Vector2Int uv_birch_leaves = new Vector2Int(19, 5);

    public static Vector2Int uv_mossy_stone = new Vector2Int(4, 6);

    public static Vector2Int uv_spruce_log = new Vector2Int(4, 7);
    public static Vector2Int uv_birch_log = new Vector2Int(5, 7);

    public static Vector2Int uv_spruce_leaves = new Vector2Int(4, 8);

    public static Vector2Int uv_jungle_log = new Vector2Int(9, 9);

    public static Vector2Int uv_emerald_ore = new Vector2Int(11, 10);

    public static Vector2Int uv_sandstone = new Vector2Int(0, 12);
    public static Vector2Int uv_spruce_wood = new Vector2Int(6, 12);
    public static Vector2Int uv_jungle_wood = new Vector2Int(7, 12);
    public static Vector2Int uv_jungle_leaves = new Vector2Int(4, 12);
    public static Vector2Int uv_purpur = new Vector2Int(13, 12);

    public static Vector2Int uv_birch_wood = new Vector2Int(6, 13);
    public static Vector2Int hay_side = new Vector2Int(16, 13);
    public static Vector2Int hay_top = new Vector2Int(17, 13);
    public static Vector2Int uv_coal_block = new Vector2Int(19, 13);

    public static Vector2Int uv_nether_brick = new Vector2Int(0, 14);
    public static Vector2Int uv_quartz = new Vector2Int(10, 14);

    public static Vector2Int uv_acacia_sapling = new Vector2Int(20, 15);
    public static Vector2Int uv_dark_oak_sapling = new Vector2Int(21, 15);

    public static Vector2Int uv_podzol_top = new Vector2Int(15, 16);
    public static Vector2Int uv_podzol_side = new Vector2Int(16, 16);
    public static Vector2Int uv_packed_ice = new Vector2Int(17, 16);
    public static Vector2Int uv_spruce_log_top = new Vector2Int(18, 16);
    public static Vector2Int uv_birch_log_top = new Vector2Int(19, 16);
    public static Vector2Int uv_jungle_log_top = new Vector2Int(20, 16);
    public static Vector2Int uv_acacia_log = new Vector2Int(21, 16);
    public static Vector2Int uv_acacia_log_top = new Vector2Int(22, 16);
    public static Vector2Int uv_dark_oak_log = new Vector2Int(23, 16);

    public static Vector2Int uv_dark_oak_log_top = new Vector2Int(0, 17);
    public static Vector2Int uv_acacia_wood = new Vector2Int(1, 17);
    public static Vector2Int uv_dark_oak_wood = new Vector2Int(2, 17);

    public static TexCoords[] type2texcoords = new TexCoords[]
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
        TexCoords.Block_front_polar_side(uv_furnace_front, uv_furnace_top, uv_furnace_side, true),
        // HayBlock
        TexCoords.Block_polar_side(hay_top, hay_side),
        // JungleLeaves
        TexCoords.Block_1_transparent(uv_jungle_leaves),
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
        // DoubleStoneSlab
        TexCoords.Block_polar_side(uv_double_stone_slab_top, uv_double_stone_slab_side),
        // Cobweb
        TexCoords.Plant(uv_web),
        // RedSand
        TexCoords.Block_1(uv_red_sand),
        // OakSapling
        TexCoords.Plant(uv_oak_sapling),
        // CoalOre
        TexCoords.Block_1(uv_coal_ore),
        // IronOre
        TexCoords.Block_1(uv_iron_ore),
        // GoldOre
        TexCoords.Block_1(uv_gold_ore),
        // DiamondOre
        TexCoords.Block_1(uv_diamond_ore),
        // EmeraldOre
        TexCoords.Block_1(uv_emerald_ore),
        // RedstoneOre
        TexCoords.Block_1(uv_redstone_ore),
        // CoalBlock
        TexCoords.Block_1(uv_coal_block),
        // IronBlock
        TexCoords.Block_1(uv_iron_block),
        // GoldBlock
        TexCoords.Block_1(uv_gold_block),
        // DiamondBlock
        TexCoords.Block_1(uv_diamond_block),
        // EmeraldBlock
        TexCoords.Block_1(uv_emerald_block),
        // RedstoneBlock
        TexCoords.Block_1(uv_redstone_block),
        // Sand
        TexCoords.Block_1(uv_sand),
        // Gravel
        TexCoords.Block_1(uv_gravel),
        // OakWoodStairs
        TexCoords.Stair(uv_oak_planks),
        // CobblestoneStairs
        TexCoords.Stair(uv_cobblestone),
        // StoneBrickStairs
        TexCoords.Stair(uv_stonebrick),
        // NetherBrickStairs
        TexCoords.Stair(uv_nether_brick),
        // SandstoneStairs
        TexCoords.Stair(uv_sandstone),
        // SpruceWoodStairs
        TexCoords.Stair(uv_spruce_wood),
        // BirchWoodStairs
        TexCoords.Stair(uv_birch_wood),
        // JungleWoodStairs
        TexCoords.Stair(uv_jungle_wood),
        // QuartzStairs
        TexCoords.Stair(uv_quartz),
        // SpruceWoodPlanks
        TexCoords.Block_1(uv_spruce_wood),
        // BirchWoodPlanks
        TexCoords.Block_1(uv_birch_wood),
        // JungleWoodPlanks
        TexCoords.Block_1(uv_jungle_wood),
        // AcaciaWoodPlanks
        TexCoords.Block_1(uv_acacia_wood),
        // DarkOakWoodPlanks
        TexCoords.Block_1(uv_dark_oak_wood),
        // Cobblestone
        TexCoords.Block_1(uv_cobblestone),
        // StoneBricks
        TexCoords.Block_1(uv_stonebrick),
        // CobblestoneWall
        TexCoords.Wall(uv_cobblestone),
        // Bookshelf
        TexCoords.Block_polar_side(uv_oak_planks, uv_bookshelf),
        // MossyCobblestoneWall
        TexCoords.Wall(uv_mossy_cobblestone),
        // MossyCobblestone
        TexCoords.Block_1(uv_mossy_cobblestone),
        // MossyCobblestoneStairs
        TexCoords.Stair(uv_mossy_cobblestone),
        // MossyStoneBricks
        TexCoords.Block_1(uv_mossy_stone),
        // MossyStoneBrickStairs
        TexCoords.Stair(uv_mossy_stone),
        // MossyStoneBrickWall
        TexCoords.Wall(uv_mossy_stone),
        // OakSlab
        TexCoords.Slab(uv_oak_planks),
        // SpruceSlab
        TexCoords.Slab(uv_spruce_wood),
        // BirchSlab
        TexCoords.Slab(uv_birch_wood),
        // JungleSlab
        TexCoords.Slab(uv_jungle_wood),
        // AcaciaSlab
        TexCoords.Slab(uv_acacia_wood),
        // DarkOakSlab
        TexCoords.Slab(uv_dark_oak_wood),
        // StoneSlab
        TexCoords.Slab(uv_stone),
        // SmoothStoneSlab
        TexCoords.Slab(uv_double_stone_slab_top),
        // CobbleStoneSlab
        TexCoords.Slab(uv_cobblestone),
        // MossyCobbleStoneSlab
        TexCoords.Slab(uv_mossy_cobblestone),
        // StoneBrickSlab
        TexCoords.Slab(uv_stonebrick),
        // MossyStoneBrickSlab
        TexCoords.Slab(uv_mossy_stone),
        // BrickSlab
        TexCoords.Slab(uv_bricks),
        // NetherBrickSlab
        TexCoords.Slab(uv_nether_brick),
        // QuartzSlab
        TexCoords.Slab(uv_quartz),
        // Glass
        TexCoords.Block_1_transparent(uv_glass),
        // GlassPane
        TexCoords.Block_1_transparent(uv_glass),
        // SpruceLeaves
        TexCoords.Block_1_transparent(uv_spruce_leaves),
        // BirchLeaves
        TexCoords.Block_1_transparent(uv_birch_leaves),
        // AcaciaLeaves
        TexCoords.Block_1_transparent(uv_oakleaves),
        // DarkOakLeaves
        TexCoords.Block_1_transparent(uv_oakleaves),
        // BirchLog
        TexCoords.Block_polar_side(uv_birch_log_top, uv_birch_log),
        // SpruceLog
        TexCoords.Block_polar_side(uv_spruce_log_top, uv_spruce_log),
        // JungleLog
        TexCoords.Block_polar_side(uv_jungle_log_top, uv_jungle_log),
        // AcaciaLog
        TexCoords.Block_polar_side(uv_acacia_log_top, uv_acacia_log),
        // DarkOakLog
        TexCoords.Block_polar_side(uv_dark_oak_log_top, uv_dark_oak_log),
        // SpruceSapling
        TexCoords.Plant(uv_spruce_sapling),
        // BirchSapling
        TexCoords.Plant(uv_birch_sapling),
        // JungleSapling
        TexCoords.Plant(uv_jungle_sapling),
        // AcaciaSapling
        TexCoords.Plant(uv_acacia_sapling),
        // DarkOakSapling
        TexCoords.Plant(uv_dark_oak_sapling),
        // Ice
        TexCoords.Block_1_transparent(uv_ice),
        // PackedIce
        TexCoords.Block_1(uv_packed_ice),
        // Chest
        TexCoords.Chest(),
    };

    static Dictionary<CSBlockType, Mesh> type2mesh = new Dictionary<CSBlockType, Mesh>();

    static Dictionary<CSBlockType, string> type2path = new Dictionary<CSBlockType, string>
    {
        { CSBlockType.Dandelion, "dandelion" },
        { CSBlockType.Poppy, "poppy" },
        { CSBlockType.Grass, "grass" },
        { CSBlockType.Cobweb, "cobweb" },
        { CSBlockType.Torch, "torch" },
        { CSBlockType.OakSapling, "oak_sapling" },
        { CSBlockType.SpruceSapling, "spruce_sapling" },
        { CSBlockType.BirchSapling, "birch_sapling" },
        { CSBlockType.JungleSapling, "jungle_sapling" },
        { CSBlockType.AcaciaSapling, "acacia_sapling" },
        { CSBlockType.DarkOakSapling, "dark_oak_sapling" },
    };

    public static bool IsCubeType(CSBlockType type)
    {
        return !type2texcoords[(byte)type].isPlant && type != CSBlockType.Torch && type != CSBlockType.Chest;
    }

    public static bool IsStair(CSBlockType type)
    {
        return type2texcoords[(byte)type].isStair;
    }

    public static bool IsWall(CSBlockType type)
    {
        return type2texcoords[(byte)type].isWall;
    }

    public static bool IsSlab(CSBlockType type)
    {
        return type2texcoords[(byte)type].isSlab;
    }

    public static Texture GetBlockTexture(CSBlockType type)
    {
        if (IsCubeType(type))
        {
            return Resources.Load<Material>("Materials/block").mainTexture;
        }
        else
        {
            string path = type2path[type];
            return Resources.Load<Texture2D>("Meshes/items/" + path + "/" + path);
        }
    }

    public static Mesh GetBlockMesh(CSBlockType type)
    {
        if (!type2mesh.ContainsKey(type))
        {
            if (IsCubeType(type))
            {
                if (IsStair(type))
                {
                    type2mesh[type] = StairMeshGenerator.Instance.GenerateSingleMesh(type);
                }
                else if (IsWall(type))
                {
                    type2mesh[type] = WallMeshGenerator.Instance.GenerateSingleMesh(type);
                }
                else if (IsSlab(type))
                {
                    type2mesh[type] = SlabMeshGenerator.Instance.GenerateSingleMesh(type);
                }
                else
                {
                    type2mesh[type] = BlockMeshGenerator.Instance.GenerateSingleMesh(type);
                }
            }
            else
            {
                string path = type2path[type];
                type2mesh[type] = Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
            }
        }
        return type2mesh[type];
    }

    public static void RefreshMeshData(this Chunk chunk)
    {
        chunk.vertices1.Clear();
        chunk.colors1.Clear();
        chunk.uv1.Clear();
        chunk.normals1.Clear();
        chunk.triangles1.Clear();

        chunk.vertices2.Clear();
        chunk.uv2.Clear();
        chunk.triangles2.Clear();

        List<Color> colors = new List<Color>();

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
                            if (IsStair(type))
                            {
                                StairMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices1, chunk.uv1, chunk.triangles1);
                            }
                            else if (IsWall(type))
                            {
                                WallMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices1, chunk.uv1, chunk.triangles1);
                            }
                            else if (IsSlab(type))
                            {
                                SlabMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices1, chunk.uv1, chunk.triangles1);
                            }
                            else if (type == CSBlockType.Torch)
                            {
                                if (!chunk.hasBuiltMesh)
                                {
                                    //加载完第一次build
                                    chunk.AddTorch(globalPos);
                                }
                            }
                            else
                            {
                                //BlockMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices1, chunk.uv1, chunk.triangles1);
                                BlockMeshGenerator.Instance.GenerateMeshInChunk(type, pos, globalPos, chunk.vertices1, chunk.colors1, chunk.uv1, chunk.normals1, chunk.triangles1);
                            }
                        }
                    }
                }
            }
        }
        chunk.hasBuiltMesh = true;
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
