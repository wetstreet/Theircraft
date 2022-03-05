using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWoodenPressurePlate : NBTBlock
{
    public override string name => "Wooden Pressure Plate";
    public override string id => "minecraft:wooden_pressure_plate";

    public override string GetDropItemByData(byte data) { return "minecraft:wooden_pressure_plate"; }

    public override string allName => "planks_oak";

    public override bool isTransparent => true;

    public override float hardness => 0.5f;

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;

    public override string GetBreakEffectTexture(byte data) { return "planks_oak"; }

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


    protected static Vector2[] uv_side = new Vector2[4];
    protected static Vector2[] _uv_side = new Vector2[4] { Vector2.zero, new Vector2(0, 0.0625f), new Vector2(1, 0.0625f), Vector2.right };

    public override void AfterTextureInit()
    {
        Rect rect = TextureArrayManager.GetRectByName(allName);
        for (int i = 0; i < 4; i++)
        {
            uv_side[i] = new Vector2(rect.xMin + _uv_side[i].x * rect.width, rect.yMin + _uv_side[i].y * rect.height);
        }
    }


    public override void AddCube(NBTChunk chunk, byte data, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.worldPos.Set(pos.x + chunk.x * 16, pos.y, pos.z + chunk.z * 16);
        ca.blockData = data;

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = skylight_default;
        fa.blockLight = blocklight_default;

        fa.pos = frontVertices_1;
        fa.normal = Vector3.forward;
        fa.uv = uv_side;
        fa.color = GetFrontTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = backVertices_1;
        fa.normal = Vector3.back;
        fa.uv = uv_side;
        fa.color = GetBackTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = topVertices_1;
        fa.normal = Vector3.up;
        fa.uv = TextureArrayManager.GetUVByName(allName);
        fa.color = GetTopTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = bottomVertices_1;
        fa.normal = Vector3.down;
        fa.uv = TextureArrayManager.GetUVByName(allName);
        fa.color = GetBottomTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = leftVertices_1;
        fa.normal = Vector3.left;
        fa.uv = uv_side;
        fa.color = GetLeftTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = rightVertices_1;
        fa.normal = Vector3.right;
        fa.uv = uv_side;
        fa.color = GetRightTintColorByData(data);
        AddFace(nbtGO.nbtMesh, fa, ca);
    }
}
