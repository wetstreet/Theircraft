using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGeneratorManager : MonoBehaviour
{
    public static Dictionary<int, NBTBlock> generatorDict = new Dictionary<int, NBTBlock>()
    {
        { 1, new NBTStone() },
        { 2, new NBTGrassBlock() },
        { 3, new NBTDirt() },
        { 4, new NBTCobblestone() },
        { 5, new NBTPlanks() },
        { 6, new NBTSapling() },
        { 7, new NBTBedrock() },
        { 9, new NBTStationaryWater() },
        { 12, new NBTSand() },
        { 13, new NBTGravel() },
        { 14, new NBTGoldOre() },
        { 15, new NBTIronOre() },
        { 16, new NBTCoalOre() },
        { 17, new NBTLog() },
        { 18, new NBTLeaves() },
        { 20, new NBTGlass() },
        { 21, new NBTRedstoneOre() },
        { 24, new NBTSandStone() },
        { 31, new NBTTallGrass() },
        { 32, new NBTDeadBush() },
        { 37, new NBTYellowFlower() },
        { 38, new NBTRedFlower() },
        { 39, new NBTBrownMushroom() },
        { 40, new NBTRedMushroom() },
        { 45, new NBTBrick() },
        { 49, new NBTObsidian() },
        { 56, new NBTDiamondOre() },
        { 73, new NBTRedstoneOre() },
        { 81, new NBTCactus() },
        { 82, new NBTClay() },
        { 83, new NBTSugarCane() },
        { 175, new NBTLargeFlowers() },
    };

    public static Dictionary<string, NBTBlock> id2generator = new Dictionary<string, NBTBlock>();

    public static void Init()
    {
        foreach (NBTBlock generator in generatorDict.Values)
        {
            generator.Init();

            if (generator.id != null)
            {
                id2generator.Add(generator.id, generator);
            }
        }
    }

    public static NBTBlock GetMeshGenerator(byte rawType)
    {
        if (generatorDict.ContainsKey(rawType))
        {
            return generatorDict[rawType];
        }
        return null;
    }

    public static bool IsTransparent(int rawType)
    {
        if (rawType == 0) return true;

        if (generatorDict.ContainsKey(rawType))
        {
            return generatorDict[rawType].isTransparent;
        }
        return false;
    }
}
