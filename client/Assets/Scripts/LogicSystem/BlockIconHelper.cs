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
