using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLeaves2 : NBTBlock
{
    public override string name => "Leaves (Acacia/Dark Oak)";
    public override string id => "minecraft:leaves2";

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

    string[] texNames = new string[] { "leaves_acacia", "leaves_big_oak" };
    string GetTexName(int data)
    {
        return texNames[data % 4];
    }

    public override string GetTopTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetBottomTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetFrontTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetBackTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetLeftTexName(NBTChunk chunk, int data) { return GetTexName(data); }
    public override string GetRightTexName(NBTChunk chunk, int data) { return GetTexName(data); }

    public override string GetBreakEffectTexture(byte data) { return GetTexName(data); }
}
