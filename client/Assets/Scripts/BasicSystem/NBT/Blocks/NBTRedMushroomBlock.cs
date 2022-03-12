using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedMushroomBlock : NBTBlock
{
    public override string name => "Red Mushroom Block";
    public override string id => "minecraft:red_mushroom_block";

    public override float hardness => 0.2f;

    public override byte GetDropItemData(byte data)
    {
        return 0;
    }

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override string GetTopTexName(NBTChunk chunk, int data)
    {
        if (data == 10)
            return "mushroom_block_inside";
        else
            return "mushroom_block_skin_red";
    }

    public override string GetBottomTexName(NBTChunk chunk, int data)
    {
        return "mushroom_block_inside";
    }

    public override string GetFrontTexName(NBTChunk chunk, int data)
    {
        if (data == 1 || data == 2 || data == 3)
            return "mushroom_block_skin_red";
        else if (data == 10)
            return "mushroom_block_skin_stem";
        else
            return "mushroom_block_inside";
    }
    public override string GetBackTexName(NBTChunk chunk, int data)
    {
        if (data == 7 || data == 8 || data == 9)
            return "mushroom_block_skin_red";
        else if (data == 10)
            return "mushroom_block_skin_stem";
        else
            return "mushroom_block_inside";
    }
    public override string GetLeftTexName(NBTChunk chunk, int data)
    {
        if (data == 1 || data == 4 || data == 7)
            return "mushroom_block_skin_red";
        else if (data == 10)
            return "mushroom_block_skin_stem";
        else
            return "mushroom_block_inside";
    }
    public override string GetRightTexName(NBTChunk chunk, int data)
    {
        if (data == 3 || data == 6 || data == 9)
            return "mushroom_block_skin_red";
        else if (data == 10)
            return "mushroom_block_skin_stem";
        else
            return "mushroom_block_inside";
    }

    public override string GetBreakEffectTexture(byte data) { return "mushroom_block_inside"; }
}
