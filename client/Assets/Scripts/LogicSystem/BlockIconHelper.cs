using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class BlockIconHelper
{
    public static Dictionary<CSBlockType, string> type2string = new Dictionary<CSBlockType, string>
    {
        {CSBlockType.GrassBlock, "grassblock" },
        {CSBlockType.Dirt, "dirt" },
        {CSBlockType.BrickStairs, "Brick_Stairs" },
        {CSBlockType.Brick, "brick" },
        {CSBlockType.Furnace, "furnace" },
        {CSBlockType.HayBlock, "hayblock" },
        {CSBlockType.Stone, "stone" },
        {CSBlockType.BrickWall, "Brick_Wall" },
        {CSBlockType.OakLog, "Oak_Log" },
        {CSBlockType.OakPlanks, "Oak_Planks" },
        {CSBlockType.OakLeaves, "Oak_Leaves" },
        {CSBlockType.RedSand, "Red_Sand" },
        {CSBlockType.BedRock, "Bedrock" },
        {CSBlockType.Tnt, "tnt" },
        {CSBlockType.CoalOre, "Coal_Ore" },
        {CSBlockType.IronOre, "Iron_Ore" },
        {CSBlockType.GoldOre, "Gold_Ore" },
        {CSBlockType.DiamondOre, "Diamond_Ore" },
        {CSBlockType.EmeraldOre, "Emerald_Ore_JE2_BE2" },
        {CSBlockType.RedstoneOre, "Redstone_Ore" },
        {CSBlockType.CoalBlock, "Block_of_Coal" },
        {CSBlockType.IronBlock, "Block_of_Iron" },
        {CSBlockType.GoldBlock, "Block_of_Gold" },
        {CSBlockType.DiamondBlock, "Block_of_Diamond_JE6_BE3" },
        {CSBlockType.EmeraldBlock, "Block_of_Emerald_JE4_BE3" },
        {CSBlockType.RedstoneBlock, "Block_of_Redstone_JE2_BE2" },
        {CSBlockType.Sand, "Sand_JE5_BE2" },
        {CSBlockType.Gravel, "Gravel_Revision_5" },
        {CSBlockType.OakWoodStairs, "Oak_Stairs" },
        {CSBlockType.CobblestoneStairs, "Cobblestone_Stairs" },
        {CSBlockType.StoneBrickStairs, "Stone_Brick_Stairs" },
        {CSBlockType.NetherBrickStairs, "Nether_Brick_Stairs" },
        {CSBlockType.SandstoneStairs, "Sandstone_Stairs" },
        {CSBlockType.SpruceWoodStairs, "Spruce_Stairs" },
        {CSBlockType.BirchWoodStairs, "Birch_Stairs" },
        {CSBlockType.JungleWoodStairs, "Jungle_Stairs" },
        {CSBlockType.QuartzStairs, "Quartz_Stairs" },

        {CSBlockType.Torch, "torch" },
        {CSBlockType.Cobweb, "web" },
        {CSBlockType.OakSapling, "oak_sapling" },
        {CSBlockType.Poppy, "poppy" },
        {CSBlockType.Dandelion, "dandelion" },
        {CSBlockType.Grass, "grass" },
    };

    static Dictionary<string, Sprite> name2sprite = new Dictionary<string, Sprite>();

    static bool isInited = false;

    public static Sprite GetIcon(CSBlockType type)
    {
        if (!isInited)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("GUI/blocks");
            foreach (Sprite sprite in sprites)
            {
                name2sprite.Add(sprite.name, sprite);
            }
            isInited = true;
        }

        if (!type2string.ContainsKey(type))
        {
            Debug.LogError("icon not exist! type = " + type);
        }
        string name = type2string[type];

        if (!name2sprite.ContainsKey(name))
        {
            name2sprite.Add(name, Resources.Load<Sprite>("GUI/CubeBlock/" + type2string[type]));
        }

        return name2sprite[name];
    }
}
