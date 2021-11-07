using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLeaves : NBTBlock
{
    public override string name { get { return "Leaves"; } }
    public override string id { get { return "minecraft:leaves"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "leaves_oak", "leaves_spruce", "leaves_birch", "leaves_jungle" };
    }
    
    public override float hardness { get { return 0.2f; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool willReduceLight => true;

    public override string GetIconPathByData(short data)
    {
        switch (data % 4)
        {
            case 0:
                return "OakLeaves";
            case 1:
                return "SpruceLeaves";
            case 2:
                return "BirchLeaves";
            case 3:
                return "JungleLeaves";
        }
        return "OakLeaves";
    }

    public override string GetNameByData(short data)
    {
        switch (data % 4)
        {
            case 0:
                return "Oak Leaves";
            case 1:
                return "Spruce Leaves";
            case 2:
                return "Birch Leaves";
            case 3:
                return "Jungle Leaves";
        }
        return "Leaves";
    }

    Color GetTintColorByData(byte data)
    {
        switch (data % 4)
        {
            case 0:
                return TintManager.oakTintColor;
            case 1:
                return TintManager.spruceTintColor;
            case 2:
                return TintManager.birchTintColor;
            case 3:
                return TintManager.jungleTintColor;
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
                return TextureArrayManager.GetIndexByName("leaves_oak");
            case 1:
                return TextureArrayManager.GetIndexByName("leaves_spruce");
            case 2:
                return TextureArrayManager.GetIndexByName("leaves_birch");
            case 3:
                return TextureArrayManager.GetIndexByName("leaves_jungle");
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
                texture = "leaves_oak";
                break;
            case 1:
                texture = "leaves_spruce";
                break;
            case 2:
                texture = "leaves_birch";
                break;
            case 3:
                texture = "leaves_jungle";
                break;
        }
        return texture;
    }
}
