using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSugarCane : NBTPlant
{
    public override string name => "Sugar Cane";
    public override string id => "minecraft:reeds";

    public override string GetTexName(NBTChunk chunk, Vector3Int pos, int data)
    {
        return "reeds";
    }

    public override string pathPrefix => "GUI/items/";

    public override string GetIconPathByData(short data = 0)
    {
        return "reeds";
    }
    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(data))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(data));
            mat.mainTexture = tex;
            itemMaterialDict.Add(data, mat);
        }
        return itemMaterialDict[data];
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte data)
    {
        if (!itemMeshDict.ContainsKey(data))
        {
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(data));
            Mesh mesh = ItemMeshGenerator.instance.Generate(tex);
            itemMeshDict.Add(data, mesh);
        }
        return itemMeshDict[data];
    }

    public override string GetBreakEffectTexture(byte data) { return "reeds"; }
}