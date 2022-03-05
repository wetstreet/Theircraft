﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLog2 : NBTBlock
{
    public override string name => "Log (Acacia/Dark Oak)";
    public override string id => "minecraft:log2";
    
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

    public override float hardness => 2f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

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
    string[] woodNames = new string[] { "log_acacia", "log_big_oak" };

    string GetNameByData(int data, LogAxis axis)
    {
        int wood_type = data & 0b0011;
        int dir = data & 0b1100;
        string name = woodNames[wood_type];
        if ((axis == LogAxis.X && dir == 1) ||
            (axis == LogAxis.Y && dir == 0) ||
            (axis == LogAxis.Z && dir == 2))
        {
            name += "_top";
        }
        return name;
    }

    public override string GetLeftTexName(NBTChunk chunk, int data) { return GetNameByData(data, LogAxis.X); }
    public override string GetRightTexName(NBTChunk chunk, int data) { return GetNameByData(data, LogAxis.X); }
    public override string GetTopTexName(NBTChunk chunk, int data) { return GetNameByData(data, LogAxis.Y); }
    public override string GetBottomTexName(NBTChunk chunk, int data) { return GetNameByData(data, LogAxis.Y); }
    public override string GetFrontTexName(NBTChunk chunk, int data) { return GetNameByData(data, LogAxis.Z); }
    public override string GetBackTexName(NBTChunk chunk, int data) { return GetNameByData(data, LogAxis.Z); }

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

    public override void OnAddBlock(RaycastHit hit)
    {
        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        byte type = NBTGeneratorManager.id2type[id];
        byte data = (byte)InventorySystem.items[ItemSelectPanel.curIndex].damage;

        if (hit.normal == Vector3.up || hit.normal == Vector3.down)
        {
            data |= 0b0000;
        }
        else if (hit.normal == Vector3.left || hit.normal == Vector3.right)
        {
            data |= 0b0100;
        }
        else if (hit.normal == Vector3.back || hit.normal == Vector3.forward)
        {
            data |= 0b1000;
        }
        NBTHelper.SetBlockData(pos, type, data);
    }
}
