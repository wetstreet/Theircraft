using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenPressurePlate : NBTBlock
{
    public override string name { get { return "Wooden Pressure Plate"; } }
    public override string id { get { return "minecraft:wooden_pressure_plate"; } }

    public override string GetDropItemByData(byte data) { return "minecraft:wooden_pressure_plate"; }

    public override string topName { get { return "planks_oak"; } }
    public override string bottomName { get { return "planks_oak"; } }
    public override string frontName { get { return "planks_oak"; } }
    public override string backName { get { return "planks_oak"; } }
    public override string leftName { get { return "planks_oak"; } }
    public override string rightName { get { return "planks_oak"; } }

    public override bool isTransparent => true;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Stone; } }

    public override string GetBreakEffectTexture(byte data) { return "lapis_ore"; }

    protected static Vector3 nearBottomLeft_1 = new Vector3(-0.4375f, -0.5f, -0.4375f);
    protected static Vector3 nearBottomRight_1 = new Vector3(0.4375f, -0.5f, -0.4375f);
    protected static Vector3 nearTopLeft_1 = new Vector3(-0.4375f, -0.4375f, -0.4375f);
    protected static Vector3 nearTopRight_1 = new Vector3(0.4375f, -0.4375f, -0.4375f);
    protected static Vector3 farBottomLeft_1 = new Vector3(-0.4375f, -0.5f, 0.4375f);
    protected static Vector3 farBottomRight_1 = new Vector3(0.4375f, -0.5f, 0.4375f);
    protected static Vector3 farTopLeft_1 = new Vector3(-0.4375f, -0.4375f, 0.4375f);
    protected static Vector3 farTopRight_1 = new Vector3(0.4375f, -0.4375f, 0.4375f);

    protected static Vector3[] frontVertices_1 = new Vector3[] { nearBottomLeft_1, nearTopLeft_1, nearTopRight_1, nearBottomRight_1 };
    protected static Vector3[] backVertices_1 = new Vector3[] { farBottomRight_1, farTopRight_1, farTopLeft_1, farBottomLeft_1 };
    protected static Vector3[] topVertices_1 = new Vector3[] { farTopRight_1, nearTopRight_1, nearTopLeft_1, farTopLeft_1 };
    protected static Vector3[] bottomVertices_1 = new Vector3[] { nearBottomRight_1, farBottomRight_1, farBottomLeft_1, nearBottomLeft_1 };
    protected static Vector3[] leftVertices_1 = new Vector3[] { farBottomLeft_1, farTopLeft_1, nearTopLeft_1, nearBottomLeft_1 };
    protected static Vector3[] rightVertices_1 = new Vector3[] { nearBottomRight_1, nearTopRight_1, farTopRight_1, farBottomRight_1 };

    protected static Vector2[] uv_side = new Vector2[4] { Vector2.zero, new Vector2(0, 0.0625f), new Vector2(1, 0.0625f), Vector2.right };



    public override void AddCube(NBTChunk chunk, byte data, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.worldPos = new Vector3Int(pos.x + chunk.x * 16, pos.y, pos.z + chunk.z * 16);
        ca.blockData = data;

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = skylight_default;
        fa.blockLight = blocklight_default;

        fa.pos = frontVertices_1;
        fa.normal = Vector3.forward;
        fa.faceIndex = GetFrontIndexByData(null, data);
        fa.uv = uv_side;
        fa.color = GetFrontTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = backVertices_1;
        fa.normal = Vector3.back;
        fa.faceIndex = GetBackIndexByData(null, data);
        fa.uv = uv_side;
        fa.color = GetBackTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = topVertices_1;
        fa.normal = Vector3.up;
        fa.faceIndex = GetTopIndexByData(null, data);
        fa.uv = GetTopRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
        fa.color = GetTopTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = bottomVertices_1;
        fa.normal = Vector3.down;
        fa.faceIndex = GetBottomIndexByData(null, data);
        fa.uv = GetBottomRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
        fa.color = GetBottomTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = leftVertices_1;
        fa.normal = Vector3.left;
        fa.faceIndex = GetLeftIndexByData(null, data);
        fa.uv = uv_side;
        fa.color = GetLeftTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = rightVertices_1;
        fa.normal = Vector3.right;
        fa.faceIndex = GetRightIndexByData(null, data);
        fa.uv = uv_side;
        fa.color = GetRightTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);
    }
}
