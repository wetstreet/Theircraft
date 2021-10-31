using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSpruceDoor : NBTBlock
{
    public override string name { get { return "Spruce Door"; } }
    public override string id { get { return "minecraft:spruce_door"; } }

    public override string GetDropItemByData(byte data) { return "minecraft:dirt"; }

    public override bool isTransparent => true;

    public override string topName { get { return "grass_path_top"; } }
    public override string bottomName { get { return "dirt"; } }
    public override string frontName { get { return "grass_path_side"; } }
    public override string backName { get { return "grass_path_side"; } }
    public override string leftName { get { return "grass_path_side"; } }
    public override string rightName { get { return "grass_path_side"; } }

    public override float hardness { get { return 0.6f; } }

    public override Color GetTopTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }


    protected static Vector2[] uv_side = new Vector2[4] { Vector2.zero, new Vector2(0, 0.9375f), new Vector2(1, 0.9375f), Vector2.right };


    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {

    }
}
