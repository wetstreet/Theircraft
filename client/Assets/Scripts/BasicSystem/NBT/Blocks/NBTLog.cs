using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTLog : NBTBlock
{
    public override string name { get { return "Log"; } }
    public override string id { get { return "minecraft:log"; } }
    
    public override string GetIconPathByData(short data)
    {
        if (data == 0) return "OakLog";
        else if (data == 1) return "SpruceLog";
        else if (data == 2) return "BirchLog";
        else if (data == 3) return "JungleLog";

        throw new System.Exception("no icon, data=" + data);
    }

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Oak Wood";
            case 1:
                return "Spruce Wood";
            case 2:
                return "Birch Wood";
            case 3:
                return "Jungle Wood";
        }
        throw new System.Exception("no name, data=" + data);
    }


    public override byte GetDropItemData(byte data) { return (byte)(data % 4); }

    public override float hardness { get { return 2f; } }

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

    public override void Init()
    {
        UsedTextures = new string[] { "log_oak_top", "log_oak", "log_spruce_top", "log_spruce", "log_birch_top", "log_birch", "log_jungle_top", "log_jungle" };
    }

    public override int GetTopIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 1:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 2:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 3:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
            case 4:
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 5:
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 6:
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 7:
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle");
        }
        return TextureArrayManager.GetIndexByName("log_oak_top");
    }

    public override int GetBottomIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 1:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 2:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 3:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
            case 4:
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 5:
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 6:
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 7:
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle");
        }
        return TextureArrayManager.GetIndexByName("log_oak_top");
    }
    public override int GetFrontIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 4:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 1:
            case 5:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 2:
            case 6:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 3:
            case 7:
                return TextureArrayManager.GetIndexByName("log_jungle");
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
        }
        return TextureArrayManager.GetIndexByName("log_oak");
    }

    public override int GetBackIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 4:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 1:
            case 5:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 2:
            case 6:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 3:
            case 7:
                return TextureArrayManager.GetIndexByName("log_jungle");
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
        }
        return TextureArrayManager.GetIndexByName("log_oak");
    }
    public override int GetLeftIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 1:
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 2:
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 3:
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle");
            case 4:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 5:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 6:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 7:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
        }
        return TextureArrayManager.GetIndexByName("log_oak");
    }

    public override int GetRightIndexByData(NBTChunk chunk, int data)
    {
        switch (data)
        {
            case 0:
            case 8:
                return TextureArrayManager.GetIndexByName("log_oak");
            case 1:
            case 9:
                return TextureArrayManager.GetIndexByName("log_spruce");
            case 2:
            case 10:
                return TextureArrayManager.GetIndexByName("log_birch");
            case 3:
            case 11:
                return TextureArrayManager.GetIndexByName("log_jungle");
            case 4:
                return TextureArrayManager.GetIndexByName("log_oak_top");
            case 5:
                return TextureArrayManager.GetIndexByName("log_spruce_top");
            case 6:
                return TextureArrayManager.GetIndexByName("log_birch_top");
            case 7:
                return TextureArrayManager.GetIndexByName("log_jungle_top");
        }
        return TextureArrayManager.GetIndexByName("log_oak");
    }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetBreakEffectTexture(byte data)
    {
        string texture = "";
        switch (data % 4)
        {
            case 0:
                texture = "log_oak";
                break;
            case 1:
                texture = "log_spruce";
                break;
            case 2:
                texture = "log_birch";
                break;
            case 3:
                texture = "log_jungle";
                break;
        }
        return texture;
    }
}
