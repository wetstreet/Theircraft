using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NBTItem : NBTObject
{

    public override string pathPrefix { get { return "GUI/items/"; } }


    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(data))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>("GUI/items/" + GetIconPathByData(data));
            mat.mainTexture = tex;
            itemMaterialDict.Add(data, mat);
        }
        return itemMaterialDict[data];
    }

    public override Mesh GetItemMesh(NBTChunk chunk, byte data)
    {
        if (!itemMeshDict.ContainsKey(data))
        {
            Texture2D tex = Resources.Load<Texture2D>("GUI/items/" + GetIconPathByData(data));
            Mesh mesh = ItemMeshGenerator.instance.Generate(tex);
            itemMeshDict.Add(data, mesh);
        }
        return itemMeshDict[data];
    }
}
