using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTYellowFlower : NBTPlant
{
    public override string name => "Yellow Flower";
    public override string id => "minecraft:yellow_flower";

    protected override int size => 4;
    protected override int height => 8;

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        if (data == 0)
        {
            return "flower_dandelion";
        }
        else if (data == 3)
        {
            return "flower_houstonia";
        }
        else if (data == 8)
        {
            return "flower_oxeye_daisy";
        }
        throw new System.Exception("no texture");
    }

    public override string GetBreakEffectTexture(byte data)
    {
        return GetTexName(null, Vector3Int.zero, data);
    }

    public override string GetIconPathByData(short data)
    {
        if (data == 0)
        {
            return "flower_dandelion";
        }
        else if (data == 3)
        {
            return "flower_houstonia";
        }
        else if (data == 8)
        {
            return "flower_oxeye_daisy";
        }
        throw new System.Exception("no icon");
    }

    public override string GetNameByData(short data)
    {
        switch (data)
        {
            case 0:
                return "Dandelion";
            case 3:
                return "Houstonia";
            case 8:
                return "Oxeye Daisy";
        }
        throw new System.Exception("no name, data=" + data);
    }

    public override void RenderWireframe(byte blockData)
    {
        float top = 0f;
        float bottom = -0.501f;
        float left = -0.1875f;
        float right = 0.1875f;
        float front = 0.1875f;
        float back = -0.1875f;

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }
}