using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTRedFlower : NBTPlant
{
    public override string name { get { return "Red Flower"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "flower_rose", "flower_houstonia", "flower_tulip_red", "flower_tulip_orange", "flower_tulip_white", "flower_oxeye_daisy" };
    }

    public override int GetPlantIndexByData(int data)
    {
        if (data == 0)
        {
            return TextureArrayManager.GetIndexByName("flower_rose");
        }
        else if (data == 3)
        {
            return TextureArrayManager.GetIndexByName("flower_houstonia");
        }
        else if (data == 4)
        {
            return TextureArrayManager.GetIndexByName("flower_tulip_red");
        }
        else if (data == 5)
        {
            return TextureArrayManager.GetIndexByName("flower_tulip_orange");
        }
        else if (data == 6)
        {
            return TextureArrayManager.GetIndexByName("flower_houstonia");
        }
        else if (data == 8)
        {
            return TextureArrayManager.GetIndexByName("flower_oxeye_daisy");
        }
        throw new System.Exception("no index");
    }

    public override string GetBreakEffectTexture(byte data)
    {
        if (data == 0)
        {
            return "flower_rose";
        }
        else if (data == 3)
        {
            return "flower_houstonia";
        }
        else if (data == 4)
        {
            return "flower_tulip_red";
        }
        else if (data == 5)
        {
            return "flower_tulip_orange";
        }
        else if (data == 6)
        {
            return "flower_houstonia";
        }
        else if (data == 8)
        {
            return "flower_oxeye_daisy";
        }
        Debug.Log("red flower no break effect texture, data=" + data);
        return "flower_rose";
    }

    public override Mesh GetItemMesh(NBTChunk chunk, byte data)
    {
        if (data == 0)
        {
            string path = "poppy";
            return Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
        }
        throw new System.Exception("no texture");
    }
}