using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class BlockIconHelper
{
    public static Sprite GetIcon(CSBlockType type)
    {
        return Resources.Load<Sprite>("GUI/icon/" + type.ToString());
    }

    public static Sprite GetIcon(string id, short data)
    {
        if (NBTGeneratorManager.id2generator.ContainsKey(id))
        {
            string path = NBTGeneratorManager.id2generator[id].GetIconPathByData(data);
            return Resources.Load<Sprite>("GUI/icon/" + path);
        }
        Debug.Log("no icon, id=" + id + ",data=" + data);
        return null;
    }
}
