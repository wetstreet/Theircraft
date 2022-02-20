using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLeaves : NBTBlock
{
    public override string name => "Leaves";
    public override string id => "minecraft:leaves";

    public override float hardness => 0.2f;

    public override BlockMaterial blockMaterial => BlockMaterial.Leaves;
    public override SoundMaterial soundMaterial => SoundMaterial.Grass;

    public override bool isTransparent => true;

    public override bool willReduceLight => true;

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

    string GetTexName(int data)
    {
        switch (data % 4)
        {
            case 0:
                return "leaves_oak";
            case 1:
                return "leaves_spruce";
            case 2:
                return "leaves_birch";
            case 3:
                return "leaves_jungle";
        }
        throw new System.Exception();
    }

    public override string GetTopTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetBottomTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetFrontTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetBackTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetLeftTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetRightTexName(NBTChunk chunk, int data) { return GetTexName(data); }

    public override string GetBreakEffectTexture(byte data) { return GetTexName(data); }
}
