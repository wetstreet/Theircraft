﻿using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlockEffect : MonoBehaviour
{
    static Dictionary<CSBlockType, string> type2breakEffect = new Dictionary<CSBlockType, string>
    {
        {CSBlockType.Grass, "dirt" },
        {CSBlockType.Dirt, "dirt" },
        {CSBlockType.Tnt, "tnt_side" },
        {CSBlockType.Brick, "brick" },
        {CSBlockType.Furnace, "furnace_front_off" },
        {CSBlockType.HayBlock, "hay_block_side" },
    };

    public static void Create(CSBlockType type,Vector3Int pos)
    {
        GameObject prefab = Resources.Load("Prefabs/BreakBlockEffect") as GameObject;
        GameObject effect = Instantiate(prefab);
        effect.transform.localPosition = pos;
        Destroy(effect, 1);
        
        if (type2breakEffect.ContainsKey(type))
        {
            string name = type2breakEffect[type];
            Texture tex = Resources.Load<Texture>("GUI/PlainBlock/" + name);
            effect.GetComponent<Renderer>().material.mainTexture = tex;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
