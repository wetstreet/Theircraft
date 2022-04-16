using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    None             = 0,
    Stone            = 1,
    GrassBlock       = 2,
    Dirt             = 3,
    Cobblestone      = 4,
    Planks           = 5,
    Sapling          = 6,
}

public class NBTGeneratorManager : MonoBehaviour
{
    public static Dictionary<byte, NBTBlock> generatorDict;
    public static List<NBTItem> itemList;
    public static Dictionary<string, NBTBlock> id2generator;
    public static Dictionary<string, byte> id2type;
    public static Dictionary<string, NBTItem> id2item;

    public static void Init()
    {
        generatorDict = new Dictionary<byte, NBTBlock>()
        {
            { 1, new NBTStone() },
            { 2, new NBTGrassBlock() },
            { 3, new NBTDirt() },
            { 4, new NBTCobblestone() },
            { 5, new NBTPlanks() },
            { 6, new NBTSapling() },
            { 7, new NBTBedrock() },
            { 9, new NBTStationaryWater() },
            { 10, new NBTStationaryLava() },
            { 12, new NBTSand() },
            { 13, new NBTGravel() },
            { 14, new NBTGoldOre() },
            { 15, new NBTIronOre() },
            { 16, new NBTCoalOre() },
            { 17, new NBTLog() },
            { 18, new NBTLeaves() },
            { 20, new NBTGlass() },
            { 21, new NBTLapisOre() },
            { 24, new NBTSandStone() },
            { 26, new NBTBed() },
            { 30, new NBTWeb() },
            { 31, new NBTTallGrass() },
            { 32, new NBTDeadBush() },
            { 35, new NBTWool() },
            { 37, new NBTYellowFlower() },
            { 38, new NBTRedFlower() },
            { 39, new NBTBrownMushroom() },
            { 40, new NBTRedMushroom() },
            { 43, new NBTDoubleStoneSlab() },
            { 44, new NBTStoneSlab() },
            { 45, new NBTBrick() },
            { 47, new NBTBookshelf() },
            { 48, new NBTMossyCobblestone() },
            { 49, new NBTObsidian() },
            { 50, new NBTTorch() },
            { 53, new NBTOakStairs() },
            { 54, new NBTChest() },
            { 56, new NBTDiamondOre() },
            { 58, new NBTCraftingTable() },
            { 59, new NBTWheat() },
            { 60, new NBTFarmland() },
            { 61, new NBTFurnace() },
            { 62, new NBTLitFurnace() },
            { 64, new NBTOakDoor() },
            { 65, new NBTLadder() },
            { 66, new NBTRail() },
            { 67, new NBTCobblestoneStairs() },
            { 72, new NBTWoodenPressurePlate() },
            { 73, new NBTRedstoneOre() },
            { 78, new NBTSnowLayer() },
            { 79, new NBTIce() },
            { 81, new NBTCactus() },
            { 82, new NBTClay() },
            { 83, new NBTSugarCane() },
            { 85, new NBTFence() },
            { 86, new NBTPumpkin() },
            { 97, new NBTMonsterEgg() },
            { 99, new NBTBrownMushroomBlock() },
            { 100, new NBTRedMushroomBlock() },
            { 102, new NBTGlassPane() },
            { 108, new NBTBrickStairs() },
            { 125, new NBTDoubleWoodenSlab() },
            { 126, new NBTWoodenSlab() },
            { 129, new NBTEmeraldOre() },
            { 134, new NBTSpruceStairs() },
            { 135, new NBTBirchStairs() },
            { 136, new NBTJungleStairs() },
            { 141, new NBTCarrots() },
            { 142, new NBTPotatoes() },
            { 161, new NBTLeaves2() },
            { 162, new NBTLog2() },
            { 175, new NBTLargeFlowers() },
            { 188, new NBTSpruceFence() },
            { 193, new NBTSpruceDoor() },
            { 207, new NBTBeetroots() },
            { 208, new NBTGrassPath() },
        };

        itemList = new List<NBTItem>()
        {
            new NBTShears(),
            new NBTWheatSeeds(),
            new NBTBeetrootSeeds(),
            new NBTWoodenHoe(),
            new NBTWoodenShovel(),

            new NBTWoodenPickaxe(),
            new NBTStonePickaxe(),
            new NBTIronPickaxe(),
            new NBTGoldPickaxe(),
            new NBTDiamondPickaxe(),

            new NBTWoodenAxe(),
            new NBTStoneAxe(),
            new NBTIronAxe(),
            new NBTGoldAxe(),
            new NBTDiamondAxe(),

            new NBTWoodenSword(),
            new NBTStoneSword(),
            new NBTIronSword(),
            new NBTGoldSword(),
            new NBTDiamondSword(),

            new NBTStick(),
            new NBTBucket(),
            new NBTWaterBucket(),
            new NBTGunpowder(),
            new NBTPoisonousPotato(),
            new NBTRottonFlesh(),

            new NBTCoal(),
            new NBTIronIngot(),
            new NBTGoldIngot(),
            new NBTDiamond(),
        };

        id2generator = new Dictionary<string, NBTBlock>();
        id2type = new Dictionary<string, byte>();
        foreach (KeyValuePair<byte, NBTBlock> keyValue in generatorDict)
        {
            NBTBlock generator = keyValue.Value;
            generator.Init();

            if (generator.id != null)
            {
                id2generator.Add(generator.id, generator);
                id2type.Add(generator.id, keyValue.Key);
            }
        }

        id2item = new Dictionary<string, NBTItem>();
        foreach (NBTItem item in itemList)
        {
            if (item.id != null)
            {
                id2item.Add(item.id, item);
            }
        }
    }

    public static void AfterTexutreInit()
    {
        foreach (KeyValuePair<byte, NBTBlock> keyValue in generatorDict)
        {
            NBTBlock generator = keyValue.Value;
            generator.AfterTextureInit();
        }
    }

    public static void Uninit()
    {
        generatorDict = null;
        itemList = null;
        id2generator = null;
        id2type = null;
        id2item = null;
    }

    public static NBTBlock GetMeshGenerator(byte rawType)
    {
        if (generatorDict.ContainsKey(rawType))
        {
            return generatorDict[rawType];
        }
        return null;
    }

    public static NBTBlock GetMeshGenerator(string id)
    {
        if (id2generator.ContainsKey(id))
        {
            return id2generator[id];
        }
        return null;
    }

    public static NBTObject GetObjectGenerator(string id)
    {
        if (id2generator.ContainsKey(id))
        {
            return id2generator[id];
        }
        if (id2item.ContainsKey(id))
        {
            return id2item[id];
        }
        Debug.LogError("no generator, id=" + id);
        return null;
    }

    public static bool IsTransparent(byte rawType)
    {
        if (rawType == 0) return true;

        if (generatorDict.ContainsKey(rawType))
        {
            return generatorDict[rawType].isTransparent;
        }
        return false;
    }

    public static bool LightCanTravel(byte rawType)
    {
        if (rawType == 0) return true;

        if (generatorDict.ContainsKey(rawType))
        {
            return generatorDict[rawType].isTransparent && !generatorDict[rawType].willReduceLight;
        }
        return false;
    }

    public static bool IsFence(byte rawType)
    {
        if (generatorDict.ContainsKey(rawType))
        {
            return generatorDict[rawType] is NBTFence;
        }
        return false;
    }
}
