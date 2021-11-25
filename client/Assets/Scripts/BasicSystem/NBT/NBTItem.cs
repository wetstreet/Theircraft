using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NBTItem : NBTObject
{
    public override string pathPrefix { get { return "GUI/items/"; } }

    public virtual int durability { get { return -1; } }

    public virtual float toolSpeed { get { return 1; } }

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

    public virtual bool IsMatch(BlockMaterial blockMaterial)
    {
        return blockMaterial == BlockMaterial.Ground;
    }
}
