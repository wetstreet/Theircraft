using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDoubleWoodenSlab : NBTBlock
{
    public override string name => "Double Wooden Slab";
    public override string id => "minecraft:double_wooden_slab";

    public override float hardness => 2f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    string GetNameByData(int data)
    {
        switch (data)
        {
            case 0:
                return "planks_oak";
            case 1:
                return "planks_spruce";
            case 2:
                return "planks_birch";
            case 3:
                return "planks_jungle";
            case 4:
                return "planks_acacia";
            case 5:
                return "planks_big_oak";
        }
        return "planks_oak";
    }

    public override string GetTopTexName(NBTChunk chunk, int data) { return GetNameByData(data); }
    public override string GetBottomTexName(NBTChunk chunk, int data) { return GetNameByData(data); }
    public override string GetFrontTexName(NBTChunk chunk, int data) { return GetNameByData(data); }
    public override string GetBackTexName(NBTChunk chunk, int data) { return GetNameByData(data); }
    public override string GetLeftTexName(NBTChunk chunk, int data) { return GetNameByData(data); }
    public override string GetRightTexName(NBTChunk chunk, int data) { return GetNameByData(data); }

    public override string GetDropItemByData(byte data)
    {
        return "minecraft:wooden_slab";
    }

    public override string GetBreakEffectTexture(byte data)
    {
        return GetNameByData(data);
    }
}
