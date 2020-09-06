using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGeneratorManager : MonoBehaviour
{
    static Dictionary<int, NBTMeshGenerator> generatorDict = new Dictionary<int, NBTMeshGenerator>()
    {
        { 1, new NBTStone() },
        { 2, new NBTGrassBlock() },
        { 3, new NBTDirt() },
        { 7, new NBTBedrock() },
        { 12, new NBTSand() },
        { 13, new NBTGravel() },
        { 14, new NBTGoldOre() },
        { 15, new NBTIronOre() },
        { 16, new NBTCoalOre() },
        { 21, new NBTRedstoneOre() },
        { 24, new NBTSandStone() },
        { 31, new NBTTallGrass() },
        { 32, new NBTDeadBush() },
        { 49, new NBTObsidian() },
        { 56, new NBTDiamondOre() },
        { 73, new NBTRedstoneOre() },
        { 81, new NBTCactus() },
    };

    public static void ClearGeneratorData()
    {
        foreach (NBTMeshGenerator generator in generatorDict.Values)
        {
            generator.ClearData();
        }
    }

    public static NBTMeshGenerator GetMeshGenerator(int rawType)
    {
        if (generatorDict.ContainsKey(rawType))
        {
            return generatorDict[rawType];
        }
        return null;
    }

    static List<int> transparentList = new List<int>()
    {
        0,6,8,9,10,11,18,20,30,31,32,37,38,39,40,81
    };

    public static bool IsTransparent(int rawType)
    {
        return transparentList.Contains(rawType);
    }
}
