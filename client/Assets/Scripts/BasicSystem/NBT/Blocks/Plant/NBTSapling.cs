using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTSapling : NBTPlant
{
    public override string name => "Sapling";
    public override string id => "minecraft:sapling";

    public override Material GetItemMaterial(byte data)
    {
        byte index = (byte)(data % 4);
        if (!itemMaterialDict.ContainsKey(index))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>("GUI/icon/" + GetIconPathByData(index));
            mat.mainTexture = tex;
            itemMaterialDict.Add(index, mat);
        }
        return itemMaterialDict[index];
    }

    public override string GetIconPathByData(short data)
    {
        switch (data % 4)
        {
            case 0:
                return "sapling_oak";
            case 1:
                return "sapling_spruce";
            case 2:
                return "sapling_birch";
            case 3:
                return "sapling_jungle";
        }
        return null;
    }

    public override bool hasDropItem => true;

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        switch (data % 4)
        {
            case 0:
                return "sapling_oak";
            case 1:
                return "sapling_spruce";
            case 2:
                return "sapling_birch";
            case 3:
                return "sapling_jungle";
        }
        throw new System.Exception();
    }

    public override string GetBreakEffectTexture(byte data)
    {
        return GetTexName(null, Vector3Int.zero, data);
    }
}
