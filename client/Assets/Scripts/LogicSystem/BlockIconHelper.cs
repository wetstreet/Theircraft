using System.Collections.Generic;
using UnityEngine;

public class BlockIconHelper
{
    public static Texture2D GetIcon(string id, short data)
    {
        NBTObject generator = NBTGeneratorManager.GetObjectGenerator(id);
        string path = "";
        if (generator != null)
        {
            path = generator.GetIconPathByData(data);
            Texture2D icon = Resources.Load<Texture2D>(generator.pathPrefix + path);
            if (icon == null)
            {
                Debug.Log("no icon, generator=" + generator + ", id=" + id + ",data=" + data + ",path=" + path);
            }
            return icon;
        }
        Debug.Log("no icon, generator=" +generator+", id=" + id + ",data=" + data + ",path="+ path);
        return null;
    }
}
