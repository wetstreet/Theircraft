using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGrassBlock : NBTBlock
{
    public override string name { get { return "Grass Block"; } }
    public override string id { get { return "minecraft:grass"; } }

    public override string GetIconPathByData(short data) { return "GrassBlock"; }

    public override string GetDropItemByData(byte data) { return "minecraft:dirt"; }

    public override string topName { get { return "grass_top"; } }
    public override string bottomName { get { return "dirt"; } }
    public override string frontName { get { return "grass_side"; } }
    public override string backName { get { return "grass_side"; } }
    public override string leftName { get { return "grass_side"; } }
    public override string rightName { get { return "grass_side"; } }

    public override float hardness { get { return 0.6f; } }

    public override Color GetTopTintColorByData(NBTChunk chunk, byte data) { return TintManager.tintColor; }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }
}
