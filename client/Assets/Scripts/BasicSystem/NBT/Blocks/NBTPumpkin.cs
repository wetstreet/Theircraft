using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPumpkin : NBTBlock
{
    public override string name { get { return "Pumpkin"; } }
    public override string id { get { return "minecraft:pumpkin"; } }

    public override string topName { get { return "pumpkin_top"; } }
    public override string bottomName { get { return "pumpkin_top"; } }
    public override string frontName { get { return "pumpkin_face_off"; } }
    public override string backName { get { return "pumpkin_side"; } }
    public override string leftName { get { return "pumpkin_side"; } }
    public override string rightName { get { return "pumpkin_side"; } }

    public override int GetFrontIndexByData(NBTChunk chunk, int data)
    {
        if (data == 2)
            return TextureArrayManager.GetIndexByName("pumpkin_face_off");
        else
            return TextureArrayManager.GetIndexByName("pumpkin_side");
    }
    public override int GetBackIndexByData(NBTChunk chunk, int data)
    {
        if (data == 0)
            return TextureArrayManager.GetIndexByName("pumpkin_face_off");
        else
            return TextureArrayManager.GetIndexByName("pumpkin_side");
    }
    public override int GetLeftIndexByData(NBTChunk chunk, int data)
    {
        if (data == 1)
            return TextureArrayManager.GetIndexByName("pumpkin_face_off");
        else
            return TextureArrayManager.GetIndexByName("pumpkin_side");
    }
    public override int GetRightIndexByData(NBTChunk chunk, int data)
    {
        if (data == 3)
            return TextureArrayManager.GetIndexByName("pumpkin_face_off");
        else
            return TextureArrayManager.GetIndexByName("pumpkin_side");
    }

    public override float hardness => 1;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data) { return "pumpkin_face_off"; }
}
