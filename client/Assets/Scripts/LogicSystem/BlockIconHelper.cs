using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class BlockIconHelper
{
    public static Dictionary<CSBlockType, string> type2icon = new Dictionary<CSBlockType, string>
    {
        {CSBlockType.GrassBlock, "grass" },
        {CSBlockType.Dirt, "dirt" },
        {CSBlockType.BrickStairs, "Brick_Stairs" },
        {CSBlockType.Brick, "brick" },
        {CSBlockType.Furnace, "furnace" },
        {CSBlockType.HayBlock, "hayblock" },
        {CSBlockType.Stone, "stone" },
        {CSBlockType.Torch, "torch" },
        {CSBlockType.BrickWall, "Brick_Wall" },
        {CSBlockType.OakLog, "Oak_Log" },
        {CSBlockType.OakPlanks, "Oak_Planks" },
    };

    public static Sprite GetIcon(CSBlockType type)
    {
        return Resources.Load<Sprite>("GUI/CubeBlock/" + type2icon[type]);
    }
}
