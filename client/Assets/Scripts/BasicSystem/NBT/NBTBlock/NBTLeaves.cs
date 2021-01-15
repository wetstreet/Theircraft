using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLeaves : NBTBlock
{
    public override string name { get { return "Leaves"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "leaves_oak", "leaves_spruce", "leaves_birch", "leaves_jungle" };
    }

    Color GetTintColorByData(byte data)
    {
        switch (data % 4)
        {
            case 1:
                return TintManager.spruceTintColor;
            case 2:
                return TintManager.birchTintColor;
        }
        return TintManager.tintColor;
    }

    public override Color GetTopTintColorByData(NBTChunk chunk, byte data) { return GetTintColorByData(data); }
    public override Color GetBottomTintColorByData(NBTChunk chunk, byte data) { return GetTintColorByData(data); }
    public override Color GetFrontTintColorByData(NBTChunk chunk, byte data) { return GetTintColorByData(data); }
    public override Color GetBackTintColorByData(NBTChunk chunk, byte data) { return GetTintColorByData(data); }
    public override Color GetLeftTintColorByData(NBTChunk chunk, byte data) { return GetTintColorByData(data); }
    public override Color GetRightTintColorByData(NBTChunk chunk, byte data) { return GetTintColorByData(data); }

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

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

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
