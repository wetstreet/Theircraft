using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlockEffect : MonoBehaviour
{
    public static void Create(byte type, byte data, Vector3 pos)
    {
        GameObject prefab = Resources.Load("Prefabs/BreakBlockEffect") as GameObject;
        GameObject go = Instantiate(prefab);
        go.transform.localPosition = pos;
        Destroy(go, 1);

        BreakBlockEffect effect = go.AddComponent<BreakBlockEffect>();
        NBTBlock block = NBTGeneratorManager.GetMeshGenerator(type);
        effect.texturePath = block.GetBreakEffectTexture(data);
    }

    public string texturePath;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.SetTexture("_Array", TextureArrayManager.GetArray());
        GetComponent<Renderer>().material.SetInt("_Slice", TextureArrayManager.GetIndexByName(texturePath));
    }
}
