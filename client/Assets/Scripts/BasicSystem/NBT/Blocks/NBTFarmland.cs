using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTFarmland : NBTBlock
{
    public override string name { get { return "Farmland"; } }
    public override string id { get { return "minecraft:farmland"; } }

    public override int GetTopIndexByData(NBTChunk chunk, int data)
    {
        if (data == 7)
            return TextureArrayManager.GetIndexByName("farmland_wet");
        else
            return TextureArrayManager.GetIndexByName("farmland_dry");
    }

    public override int GetBottomIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName("dirt"); }
    public override int GetFrontIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName("dirt"); }
    public override int GetBackIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName("dirt"); }
    public override int GetLeftIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName("dirt"); }
    public override int GetRightIndexByData(NBTChunk chunk, int data) { return TextureArrayManager.GetIndexByName("dirt"); }

    public override float hardness { get { return 0.6f; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Gravel; } }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }
}
