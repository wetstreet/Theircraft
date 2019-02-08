using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Theircraft;
using System.IO;

public static class BlockGenerator
{
    public static Dictionary<protocol.cs_theircraft.CSBlockType, string> type2icon = new Dictionary<protocol.cs_theircraft.CSBlockType, string>
    {
        {protocol.cs_theircraft.CSBlockType.Grass, "grass" },
        {protocol.cs_theircraft.CSBlockType.Dirt, "grass" },
        {protocol.cs_theircraft.CSBlockType.Tnt, "tnt" },
        {protocol.cs_theircraft.CSBlockType.Brick, "brick" },
        {protocol.cs_theircraft.CSBlockType.Furnace, "furnace" },
        {protocol.cs_theircraft.CSBlockType.HayBlock, "hayblock" },
    };

    static Dictionary<protocol.cs_theircraft.CSBlockType, GameObject> blockType2prefab = new Dictionary<protocol.cs_theircraft.CSBlockType, GameObject>();

    public static GameObject GetBlockPrefab(protocol.cs_theircraft.CSBlockType type)
    {
        if (!blockType2prefab.ContainsKey(type))
        {
            string path = string.Format("Prefabs/Blocks/{0}", type2icon[type]);
            blockType2prefab[type] = Resources.Load(path) as GameObject;
        }
        return blockType2prefab[type];
    }
    static public GameObject CreateCube(protocol.cs_theircraft.CSBlockType type)
    {
        return Object.Instantiate(GetBlockPrefab(type));
    }
}
