using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLog2 : NBTBlock
{
    public override string name { get { return "Log (Acacia/Dark Oak)"; } }
    public override string id { get { return "minecraft:log2"; } }
    
    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Acacia Wood";
            case 1:
                return "Dark Oak Wood";
        }
        throw new System.Exception("no name, data=" + data);
    }


    public override byte GetDropItemData(byte data) { return (byte)(data % 4); }

    public override float hardness { get { return 2f; } }

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;

    protected override Rotation GetTopRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Right;
        }
        else
        {
            return Rotation.Zero;
        }
    }
    protected override Rotation GetBottomRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Right;
        }
        else
        {
            return Rotation.Zero;
        }
    }
    protected override Rotation GetFrontRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Right;
        }
        else
        {
            return Rotation.Zero;
        }
    }
    protected override Rotation GetBackRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Right;
        }
        else
        {
            return Rotation.Zero;
        }
    }
    protected override Rotation GetLeftRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Zero;
        }
        else
        {
            return Rotation.Right;
        }
    }
    protected override Rotation GetRightRotationByData(byte data)
    {
        if (data < 4)
        {
            return Rotation.Zero;
        }
        else if (data < 8)
        {
            return Rotation.Zero;
        }
        else
        {
            return Rotation.Right;
        }
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("log_acacia_top");
            case 1:
                return TextureArrayManager.GetIndexByName("log_big_oak_top");
            case 4:
            case 8:
                return TextureArrayManager.GetIndexByName("log_acacia");
            case 5:
            case 9:
                return TextureArrayManager.GetIndexByName("log_big_oak");
        }
        return TextureArrayManager.GetIndexByName("log_acacia_top");
    }

    public override int GetBottomIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("log_acacia_top");
            case 1:
                return TextureArrayManager.GetIndexByName("log_big_oak_top");
            case 4:
            case 8:
                return TextureArrayManager.GetIndexByName("log_acacia");
            case 5:
            case 9:
                return TextureArrayManager.GetIndexByName("log_big_oak");
        }
        return TextureArrayManager.GetIndexByName("log_acacia_top");
    }
    public override int GetFrontIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 4:
                return TextureArrayManager.GetIndexByName("log_acacia");
            case 1:
            case 5:
                return TextureArrayManager.GetIndexByName("log_big_oak");
            case 8:
                return TextureArrayManager.GetIndexByName("log_acacia_top");
            case 9:
                return TextureArrayManager.GetIndexByName("log_big_oak_top");
        }
        return TextureArrayManager.GetIndexByName("log_acacia");
    }

    public override int GetBackIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 4:
                return TextureArrayManager.GetIndexByName("log_acacia");
            case 1:
            case 5:
                return TextureArrayManager.GetIndexByName("log_big_oak");
            case 8:
                return TextureArrayManager.GetIndexByName("log_acacia_top");
            case 9:
                return TextureArrayManager.GetIndexByName("log_big_oak_top");
        }
        return TextureArrayManager.GetIndexByName("log_acacia");
    }
    public override int GetLeftIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 8:
                return TextureArrayManager.GetIndexByName("log_acacia");
            case 1:
            case 9:
                return TextureArrayManager.GetIndexByName("log_big_oak");
            case 4:
                return TextureArrayManager.GetIndexByName("log_acacia_top");
            case 5:
                return TextureArrayManager.GetIndexByName("log_big_oak_top");
        }
        return TextureArrayManager.GetIndexByName("log_acacia");
    }

    public override int GetRightIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 8:
                return TextureArrayManager.GetIndexByName("log_acacia");
            case 1:
            case 9:
                return TextureArrayManager.GetIndexByName("log_big_oak");
            case 4:
                return TextureArrayManager.GetIndexByName("log_acacia_top");
            case 5:
                return TextureArrayManager.GetIndexByName("log_big_oak_top");
        }
        return TextureArrayManager.GetIndexByName("log_acacia");
    }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data)
    {
        string texture = "";
        switch (data % 4)
        {
            case 0:
                texture = "log_acacia";
                break;
            case 1:
                texture = "log_big_oak";
                break;
        }
        return texture;
    }
}
