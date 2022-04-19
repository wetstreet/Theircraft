using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPlanks : NBTBlock
{
    public override string name => "Planks";
    public override string id => "minecraft:planks";

    public override float hardness => 2;

    public override short burningTime => 300;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Oak Wood Planks";
            case 1:
                return "Spruce Wood Planks";
            case 2:
                return "Birch Wood Planks";
            case 3:
                return "Jungle Wood Planks";
            case 4:
                return "Acacia Wood Planks";
            case 5:
                return "Dark Oak Wood Planks";
        }
        throw new System.Exception("no name, data=" + data);
    }

    public override string GetFrontTexName(NBTChunk chunk, int data) { return GetTexNameByData(data); }
    public override string GetBackTexName(NBTChunk chunk, int data) { return GetTexNameByData(data); }
    public override string GetLeftTexName(NBTChunk chunk, int data) { return GetTexNameByData(data); }
    public override string GetRightTexName(NBTChunk chunk, int data) { return GetTexNameByData(data); }
    public override string GetTopTexName(NBTChunk chunk, int data) { return GetTexNameByData(data); }
    public override string GetBottomTexName(NBTChunk chunk, int data) { return GetTexNameByData(data); }

    string GetTexNameByData(int data)
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

    public override string GetBreakEffectTexture(byte data)
    {
        return GetTexNameByData(data);
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetTexNameByData(data)); }
    public override int GetBottomIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetTexNameByData(data)); }
    public override int GetFrontIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetTexNameByData(data)); }
    public override int GetBackIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetTexNameByData(data)); }
    public override int GetLeftIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetTexNameByData(data)); }
    public override int GetRightIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName(GetTexNameByData(data)); }
}
