using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPlanks : NBTBlock
{
    public override string name { get { return "Planks"; } }
    public override string id { get { return "minecraft:planks"; } }

    public override float hardness => 2;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

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

    int GetIndexByData(int data)
    {
        switch (data)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("planks_oak");
            case 1:
                return TextureArrayManager.GetIndexByName("planks_spruce");
            case 2:
                return TextureArrayManager.GetIndexByName("planks_birch");
            case 3:
                return TextureArrayManager.GetIndexByName("planks_jungle");
            case 4:
                return TextureArrayManager.GetIndexByName("planks_acacia");
            case 5:
                return TextureArrayManager.GetIndexByName("planks_big_oak");
        }
        return TextureArrayManager.GetIndexByName("planks_oak");
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetBottomIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetFrontIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetBackIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetLeftIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetRightIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data)
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
        return null;
    }
}
