using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWheat : NBTPlant
{
    public override string name { get { return "Wheat"; } }
    public override string id { get { return "minecraft:wheat"; } }

    public override string pathPrefix { get { return "GUI/items/"; } }

    public override string GetIconPathByData(short data) { return "wheat"; }

    public override void Init()
    {
        UsedTextures = new string[] { "wheat_stage_7" };
    }

    public override int GetPlantIndexByData(int data)
    {
        return TextureArrayManager.GetIndexByName("wheat_stage_7");
    }

    protected override string itemMeshPath { get { return "wheat"; } }

    public override string GetBreakEffectTexture(byte data) { return "wheat"; }
}
