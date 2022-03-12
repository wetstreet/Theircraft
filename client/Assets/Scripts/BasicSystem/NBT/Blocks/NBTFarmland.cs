using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTFarmland : NBTBlock
{
    public override string name => "Farmland";
    public override string id => "minecraft:farmland";

    public override float hardness => 0.6f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;
    public override SoundMaterial soundMaterial => SoundMaterial.Gravel;

    public override string GetDropItemByData(byte data)
    {
        return "minecraft:dirt";
    }

    public override byte GetDropItemData(byte data)
    {
        return 0;
    }

    public override string GetTopTexName(NBTChunk chunk, int data)
    {
        if (data == 7)
            return "farmland_wet";
        else
            return "farmland_dry";
    }

    public override string GetBottomTexName(NBTChunk chunk, int data) { return "dirt"; }
    public override string GetFrontTexName(NBTChunk chunk, int data) { return "dirt"; }
    public override string GetBackTexName(NBTChunk chunk, int data) { return "dirt"; }
    public override string GetLeftTexName(NBTChunk chunk, int data) { return "dirt"; }
    public override string GetRightTexName(NBTChunk chunk, int data) { return "dirt"; }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }
}
