using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTPumpkin : NBTBlock
{
    public override string name => "Pumpkin";
    public override string id => "minecraft:pumpkin";

    public override string topName => "pumpkin_top";
    public override string bottomName => "pumpkin_top";
    public override string frontName => "pumpkin_face_off";
    public override string backName => "pumpkin_side";
    public override string leftName => "pumpkin_side";
    public override string rightName => "pumpkin_side";

    public override float hardness => 1;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

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

    public override string GetBreakEffectTexture(byte data) { return "pumpkin_face_off"; }

    // 0 = south
    // 1 = west
    // 2 = north
    // 3 = east
    public override void OnAddBlock(RaycastHit hit)
    {
        if (hit.normal == Vector3.down)
        {
            return;
        }

        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        byte type = NBTGeneratorManager.id2type[id];
        byte data = CalcBlockDirection(pos, 0, 1, 2, 3);
        NBTHelper.SetBlockData(pos, type, data);
    }
}
