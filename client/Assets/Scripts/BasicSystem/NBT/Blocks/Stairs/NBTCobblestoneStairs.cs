using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCobblestoneStairs : NBTStairs
{
    public override string name { get { return "Cobblestone Stairs"; } }
    public override string id { get { return "minecraft:stone_stairs"; } }

    public override string stairsName { get { return "cobblestone"; } }

    public override float hardness => 2;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetIconPathByData(short data) { return "CobblestoneStairs"; }

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "cobblestone"; }
}
