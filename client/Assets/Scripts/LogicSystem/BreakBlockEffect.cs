﻿using protocol.cs_theircraft;
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

        int chunkX = Mathf.FloorToInt(pos.x / 16f);
        int chunkZ = Mathf.FloorToInt(pos.z / 16f);
        NBTChunk chunk = NBTHelper.GetChunk(chunkX, chunkZ);

        BreakBlockEffect effect = go.AddComponent<BreakBlockEffect>();
        NBTBlock block = NBTGeneratorManager.GetMeshGenerator(type);
        effect.texturePath = block.GetBreakEffectTexture(chunk, data);
        effect.tintColor = block.GetFrontTintColorByData(chunk, data);
    }

    public string texturePath;
    public Color tintColor;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.SetTexture("_Array", TextureArrayManager.GetArray());
        GetComponent<Renderer>().material.SetInt("_Slice", TextureArrayManager.GetIndexByName(texturePath));
        GetComponent<Renderer>().material.SetColor("_Color", tintColor);
    }
}
