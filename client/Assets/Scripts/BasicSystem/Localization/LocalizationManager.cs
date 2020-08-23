using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum Language
{
    English,
    Chinese,
}

public class LocalizationManager
{
    static Dictionary<string, string> langDict = new Dictionary<string, string>();

    static List<LocalizationReplacer> replacerList = new List<LocalizationReplacer>();

    static Dictionary<Language, string> lang2path = new Dictionary<Language, string>()
    {
        { Language.English, "Languages/en_us"},
        { Language.Chinese, "Languages/zh_cn"},
    };
    
    public static void Init()
    {
        langDict.Clear();
        
        Language currentLanguage = SettingsPanel.Language;
        string path = lang2path[currentLanguage];
        TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
        string language = textAsset.text;
        string[] lines = language.Split('\n');
        foreach (string line in lines)
        {
            string[] item = line.Split('=');
            if (item.Length == 2)
            {
                string key = item[0];
                string value = item[1];
                if (langDict.ContainsKey(key))
                {
                    langDict[key] = value;
                }
                else
                {
                    langDict.Add(key, value);
                }
            }
        }

        Debug.Log("Init Language Done! current language=" + currentLanguage);
        RefreshAll();
    }

    public static void Add(LocalizationReplacer replacer)
    {
        replacerList.Add(replacer);
    }

    public static void Remove(LocalizationReplacer replacer)
    {
        replacerList.Remove(replacer);
    }

    static void RefreshAll()
    {
        foreach (LocalizationReplacer replacer in replacerList)
        {
            replacer.Refresh();
        }
    }

    public static string GetText(string key)
    {
        if (langDict.ContainsKey(key))
        {
            return langDict[key];
        }
        return null;
    }

    static Dictionary<CSBlockType, string> type2key = new Dictionary<CSBlockType, string>()
    {
        {CSBlockType.None, "" },
        {CSBlockType.Dirt, "tile.dirt.name" },
        {CSBlockType.GrassBlock, "tile.grass.name" },
        {CSBlockType.Tnt, "tile.tnt.name" },
        {CSBlockType.Brick, "tile.brick.name" },
        {CSBlockType.Furnace, "tile.furnace.name" },
        {CSBlockType.HayBlock, "tile.hayBlock.name" },
        {CSBlockType.JungleLeaves, "tile.leaves.jungle.name" },
        {CSBlockType.Grass, "tile.tallgrass.name" },
        {CSBlockType.Stone, "tile.stone.stone.name" },
        {CSBlockType.BedRock, "tile.bedrock.name" },
        {CSBlockType.Poppy, "tile.flower2.poppy.name" },
        {CSBlockType.Dandelion, "tile.flower1.dandelion.name" },
        {CSBlockType.OakLog, "tile.log.oak.name" },
        {CSBlockType.OakLeaves, "tile.leaves.oak.name" },
        {CSBlockType.BrickStairs, "tile.stairsBrick.name" },
        {CSBlockType.Torch, "tile.torch.name" },
        {CSBlockType.BrickWall, "tile.brickWall.name" },
        {CSBlockType.OakPlanks, "tile.wood.oak.name" },
        {CSBlockType.DoubleStoneSlab, "" },
        {CSBlockType.Cobweb, "tile.web.name" },
        {CSBlockType.RedSand, "tile.sand.red.name" },
        {CSBlockType.OakSapling, "tile.sapling.oak.name" },
        {CSBlockType.CoalOre, "tile.oreCoal.name" },
        {CSBlockType.IronOre, "tile.oreIron.name" },
        {CSBlockType.GoldOre, "tile.oreGold.name" },
        {CSBlockType.DiamondOre, "tile.oreDiamond.name" },
        {CSBlockType.EmeraldOre, "tile.oreEmerald.name" },
        {CSBlockType.RedstoneOre, "tile.oreRedstone.name" },
        {CSBlockType.CoalBlock, "tile.blockCoal.name" },
        {CSBlockType.IronBlock, "tile.blockIron.name" },
        {CSBlockType.GoldBlock, "tile.blockGold.name" },
        {CSBlockType.DiamondBlock, "tile.blockDiamond.name" },
        {CSBlockType.EmeraldBlock, "tile.blockEmerald.name" },
        {CSBlockType.RedstoneBlock, "tile.blockRedstone.name" },
        {CSBlockType.Sand, "tile.sand.name" },
        {CSBlockType.Gravel, "tile.gravel.name" },
        {CSBlockType.OakWoodStairs, "tile.stairsWood.name" },
        {CSBlockType.CobblestoneStairs, "tile.stairsStone.name" },
        {CSBlockType.StoneBrickStairs, "tile.stairsStoneBrickSmooth.name" },
        {CSBlockType.NetherBrickStairs, "tile.stairsNetherBrick.name" },
        {CSBlockType.SandstoneStairs, "tile.stairsSandStone.name" },
        {CSBlockType.SpruceWoodStairs, "tile.stairsWoodSpruce.name" },
        {CSBlockType.BirchWoodStairs, "tile.stairsWoodBirch.name" },
        {CSBlockType.JungleWoodStairs, "tile.stairsWoodJungle.name" },
        {CSBlockType.QuartzStairs, "tile.stairsQuartz.name" },
        {CSBlockType.SpruceWoodPlanks, "tile.wood.spruce.name" },
        {CSBlockType.BirchWoodPlanks, "tile.wood.birch.name" },
        {CSBlockType.JungleWoodPlanks, "tile.wood.jungle.name" },
        {CSBlockType.AcaciaWoodPlanks, "tile.wood.acacia.name" },
        {CSBlockType.DarkOakWoodPlanks, "tile.wood.big_oak.name" },
        {CSBlockType.Cobblestone, "tile.stonebrick.name" },
        {CSBlockType.StoneBricks, "tile.stonebricksmooth.name" },
        {CSBlockType.CobblestoneWall, "tile.cobbleWall.normal.name" },
        {CSBlockType.Bookshelf, "tile.bookshelf.name" },
        {CSBlockType.MossyCobblestoneWall, "tile.cobbleWall.mossy.name" },
        {CSBlockType.MossyCobblestone, "tile.stoneMoss.name" },
        {CSBlockType.MossyCobblestoneStairs, "tile.stairsStone.mossy.name" },
        {CSBlockType.MossyStoneBricks, "tile.stonebricksmooth.mossy.name" },
        {CSBlockType.MossyStoneBrickStairs, "tile.stairsStoneBrickSmooth.mossy.name" },
        {CSBlockType.MossyStoneBrickWall, "tile.stoneBrickWall.mossy.name" },
        {CSBlockType.OakSlab, "tile.woodSlab.oak.name" },
        {CSBlockType.SpruceSlab, "tile.woodSlab.spruce.name" },
        {CSBlockType.BirchSlab, "tile.woodSlab.birch.name" },
        {CSBlockType.JungleSlab, "tile.woodSlab.jungle.name" },
        {CSBlockType.AcaciaSlab, "tile.woodSlab.acacia.name" },
        {CSBlockType.DarkOakSlab, "tile.woodSlab.big_oak.name" },
        {CSBlockType.StoneSlab, "tile.stoneSlab.name" },
        {CSBlockType.SmoothStoneSlab, "tile.stoneSlab.smoothStoneBrick.name" },
        {CSBlockType.CobblestoneSlab, "tile.stoneSlab.cobble.name" },
        {CSBlockType.MossyCobblestoneSlab, "tile.stoneSlab.cobble_mossy.name" },
        {CSBlockType.StoneBrickSlab, "tile.stoneSlab.brick.name" },
        {CSBlockType.MossyStoneBrickSlab, "tile.stoneSlab.mossySmoothStoneBrick.name" },
        {CSBlockType.BrickSlab, "tile.stoneSlab.brick.name" },
        {CSBlockType.NetherBrickSlab, "tile.stoneSlab.netherBrick.name" },
        {CSBlockType.QuartzSlab, "tile.stoneSlab.quartz.name" },
        {CSBlockType.Glass, "tile.glass.name" },
        {CSBlockType.GlassPane, "tile.thinGlass.name" },
        {CSBlockType.SpruceLeaves, "tile.leaves.spruce.name" },
        {CSBlockType.BirchLeaves, "tile.leaves.birch.name" },
        {CSBlockType.AcaciaLeaves, "tile.leaves.acacia.name" },
        {CSBlockType.DarkOakLeaves, "tile.leaves.big_oak.name" },
        {CSBlockType.BirchLog, "tile.log.birch.name" },
        {CSBlockType.SpruceLog, "tile.log.spruce.name" },
        {CSBlockType.JungleLog, "tile.log.jungle.name" },
        {CSBlockType.AcaciaLog, "tile.log.acacia.name" },
        {CSBlockType.DarkOakLog, "tile.log.big_oak.name" },
        {CSBlockType.SpruceSapling, "tile.sapling.spruce.name" },
        {CSBlockType.BirchSapling, "tile.sapling.birch.name" },
        {CSBlockType.JungleSapling, "tile.sapling.jungle.name" },
        {CSBlockType.AcaciaSapling, "tile.sapling.acacia.name" },
        {CSBlockType.DarkOakSapling, "tile.sapling.big_oak.name" },
        {CSBlockType.Ice, "tile.ice.name" },
        {CSBlockType.PackedIce, "tile.icePacked.name" },
        {CSBlockType.Chest, "tile.chest.name" },
        {CSBlockType.VerticalBrickSlab, "tile.stoneSlab.verticalBrick.name" },
    };

    public static string GetBlockName(CSBlockType type)
    {
        if (type2key.ContainsKey(type))
        {
            return GetText(type2key[type]);
        }
        return type.ToString();
    }
}
