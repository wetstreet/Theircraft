using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedMushroomBlock : NBTBlock
{
    public override string name { get { return "Red Mushroom Block"; } }
    public override string id { get { return "minecraft:red_mushroom_block"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "mushroom_block_skin_red", "mushroom_block_inside", "mushroom_block_skin_stem" };
    }

    public override float hardness { get { return 0.2f; } }

    public override int GetTopIndexByData(NBTChunk chunk, int data)
    {
        if (data == 10)
            return TextureArrayManager.GetIndexByName("mushroom_block_inside");
        else
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_red");
    }

    public override int GetBottomIndexByData(NBTChunk chunk, int data)
    {
        return TextureArrayManager.GetIndexByName("mushroom_block_inside");
    }

    public override int GetFrontIndexByData(NBTChunk chunk, int data)
    {
        if (data == 1 || data == 2 || data == 3)
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_red");
        else if (data == 10)
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_stem");
        else
            return TextureArrayManager.GetIndexByName("mushroom_block_inside");
    }
    public override int GetBackIndexByData(NBTChunk chunk, int data)
    {
        if (data == 7 || data == 8 || data == 9)
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_red");
        else if (data == 10)
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_stem");
        else
            return TextureArrayManager.GetIndexByName("mushroom_block_inside");
    }
    public override int GetLeftIndexByData(NBTChunk chunk, int data)
    {
        if (data == 1 || data == 4 || data == 7)
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_red");
        else if (data == 10)
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_stem");
        else
            return TextureArrayManager.GetIndexByName("mushroom_block_inside");
    }
    public override int GetRightIndexByData(NBTChunk chunk, int data)
    {
        if (data == 3 || data == 6 || data == 9)
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_red");
        else if (data == 10)
            return TextureArrayManager.GetIndexByName("mushroom_block_skin_stem");
        else
            return TextureArrayManager.GetIndexByName("mushroom_block_inside");
    }

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data) { return "mushroom_block_inside"; }
}
