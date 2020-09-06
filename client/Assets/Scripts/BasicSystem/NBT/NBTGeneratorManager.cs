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
        { 31, new NBTTallGrass() },
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
}
