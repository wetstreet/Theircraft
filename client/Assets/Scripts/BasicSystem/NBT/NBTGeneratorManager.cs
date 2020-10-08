using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGeneratorManager : MonoBehaviour
{
    static Dictionary<int, NBTBlock> generatorDict = new Dictionary<int, NBTBlock>()
    {
        { 1, new NBTStone() },
        { 2, new NBTGrassBlock() },
        { 3, new NBTDirt() },
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
        { 45, new NBTBrick() },
        { 49, new NBTObsidian() },
        { 56, new NBTDiamondOre() },
        { 73, new NBTRedstoneOre() },
        { 81, new NBTCactus() },
        { 83, new NBTSugarCane() },
    };

    public static void ClearGeneratorData()
    {
        foreach (NBTBlock generator in generatorDict.Values)
        {
            generator.ClearData();
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
