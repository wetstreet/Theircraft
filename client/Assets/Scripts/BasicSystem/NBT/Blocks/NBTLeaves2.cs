using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLeaves2 : NBTBlock
{
    public override string name { get { return "Leaves (Acacia/Dark Oak)"; } }
    public override string id { get { return "minecraft:leaves2"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "leaves_acacia", "leaves_big_oak" };
    }
    
    public override float hardness { get { return 0.2f; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool willReduceLight => true;

    public override string GetNameByData(short data)
    {
        switch (data % 4)
        {
            case 0:
                return "Ancacia Leaves";
            case 1:
                return "Dark Oak Leaves";
        }
        return "Leaves";
    }

    Color GetTintColorByData(byte data)
    {
        switch (data % 4)
        {
            case 0:
                return TintManager.acaciaTintColor;
            case 1:
                return TintManager.darkOakTintColor;
        }
        return TintManager.tintColor;
    }

    public override Color GetTopTintColorByData(byte data) { return GetTintColorByData(data); }
    public override Color GetBottomTintColorByData(byte data) { return GetTintColorByData(data); }
    public override Color GetFrontTintColorByData(byte data) { return GetTintColorByData(data); }
    public override Color GetBackTintColorByData(byte data) { return GetTintColorByData(data); }
    public override Color GetLeftTintColorByData(byte data) { return GetTintColorByData(data); }
    public override Color GetRightTintColorByData(byte data) { return GetTintColorByData(data); }

    int GetIndexByData(int data)
    {
        switch (data % 4)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("leaves_acacia");
            case 1:
                return TextureArrayManager.GetIndexByName("leaves_big_oak");
        }
        return TextureArrayManager.GetIndexByName("leaves_oak");
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetBottomIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetFrontIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetBackIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetLeftIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }
    public override int GetRightIndexByData(NBTChunk chunk, int data) { return GetIndexByData(data); }

    public override string GetBreakEffectTexture(byte data)
    {
        string texture = "";
        switch (data % 4)
        {
            case 0:
                texture = "leaves_acacia";
                break;
            case 1:
                texture = "leaves_big_oak";
                break;
        }
        return texture;
    }
}
