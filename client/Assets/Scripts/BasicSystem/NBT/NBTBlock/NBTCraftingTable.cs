using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCraftingTable : NBTBlock
{
    public override string name { get { return "Crafting Table"; } }
    public override string id { get { return "minecraft:crafting_table"; } }

    public override string GetIconPathByData(short data)
    {
        return "CraftingTable";
    }

    public override float hardness { get { return 2.5f; } }

    public override void Init()
    {
        UsedTextures = new string[] { "crafting_table_front", "crafting_table_top", "crafting_table_side" };
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data)
    {
        return TextureArrayManager.GetIndexByName("crafting_table_top");
    }

    public override int GetBottomIndexByData(NBTChunk chunk, int data)
    {
        return TextureArrayManager.GetIndexByName("crafting_table_top");
    }

    public override int GetFrontIndexByData(NBTChunk chunk, int data)
    {
        return TextureArrayManager.GetIndexByName("crafting_table_front");
    }

    public override int GetBackIndexByData(NBTChunk chunk, int data)
    {
        return TextureArrayManager.GetIndexByName("crafting_table_side");
    }

    public override int GetLeftIndexByData(NBTChunk chunk, int data)
    {
        return TextureArrayManager.GetIndexByName("crafting_table_side");
    }

    public override int GetRightIndexByData(NBTChunk chunk, int data)
    {
        return TextureArrayManager.GetIndexByName("crafting_table_front");
    }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data)
    {
        return "crafting_table_front";
    }
}
