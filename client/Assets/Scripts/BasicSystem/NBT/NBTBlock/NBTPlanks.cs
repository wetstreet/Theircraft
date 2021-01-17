using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPlanks : NBTBlock
{
    public override string name { get { return "Planks"; } }
    public override string id { get { return "minecraft:planks"; } }

    public override string GetIconPathByData(short data)
    {
        if (data == 0) return "OakWoodPlanks";
        else if (data == 1) return "SpruceWoodPlanks";
        else if (data == 2) return "BirchWoodPlanks";
        else if (data == 3) return "JungleWoodPlanks";
        else return null;
    }


    public override void Init()
    {
        UsedTextures = new string[] { "planks_oak", "planks_spruce", "planks_birch", "planks_jungle" };
    }

    int GetIndexByData(int data)
    {
        switch (data % 4)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("planks_oak");
            case 1:
                return TextureArrayManager.GetIndexByName("planks_spruce");
            case 2:
                return TextureArrayManager.GetIndexByName("planks_birch");
            case 3:
                return TextureArrayManager.GetIndexByName("planks_jungle");
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
        switch (data % 4)
        {
            case 0:
                return "planks_oak";
            case 1:
                return "planks_spruce";
            case 2:
                return "planks_birch";
            case 3:
                return "planks_jungle";
        }
        return null;
    }
}
