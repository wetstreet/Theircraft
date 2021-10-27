using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTSnowLayer : NBTBlock
{
    public override string name { get { return "Snow Layer"; } }
    public override string id { get { return "minecraft:snow_layer"; } }

    public override string GetIconPathByData(short data) { return "QuartzSlab"; }

    public override bool hasDropItem => false;

    public override bool isTransparent => true;
    public override bool isCollidable => false;

    public override string topName { get { return "snow"; } }
    public override string bottomName { get { return "snow"; } }
    public override string frontName { get { return "snow"; } }
    public override string backName { get { return "snow"; } }
    public override string leftName { get { return "snow"; } }
    public override string rightName { get { return "snow"; } }

    public override float hardness => 0.1f;

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Snow; } }

    public override string GetBreakEffectTexture(byte data) { return "snow"; }

    public override float topOffset => -0.374f;


    protected static Vector3 nearMiddleLeft = new Vector3(-0.5f, -0.375f, -0.5f);
    protected static Vector3 farMiddleLeft = new Vector3(-0.5f, -0.375f, 0.5f);
    protected static Vector3 nearMiddleRight = new Vector3(0.5f, -0.375f, -0.5f);
    protected static Vector3 farMiddleRight = new Vector3(0.5f, -0.375f, 0.5f);

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.blockData = blockData;

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }

        AddFace(nbtGO.nbtMesh, GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca), ca);

        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
    }

    static Vector2 leftMid = new Vector2(0, 0.125f);
    static Vector2 rightMid = new Vector2(1, 0.125f);

    static Vector2[] uv_full = new Vector2[4] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    static Vector2[] uv_bot = new Vector2[4] { Vector2.zero, leftMid, rightMid, Vector2.right };

    static Vector3[] frontVertices_snow = new Vector3[] { nearBottomLeft, nearMiddleLeft, nearMiddleRight, nearBottomRight };
    static Vector3[] backVertices_snow = new Vector3[] { farBottomRight, farMiddleRight, farMiddleLeft, farBottomLeft };
    static Vector3[] topVertices_snow = new Vector3[] { farMiddleRight, nearMiddleRight, nearMiddleLeft, farMiddleLeft };
    static Vector3[] bottomVertices_snow = new Vector3[] { nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft };
    static Vector3[] leftVertices_snow = new Vector3[] { farBottomLeft, farMiddleLeft, nearMiddleLeft, nearBottomLeft };
    static Vector3[] rightVertices_snow = new Vector3[] { nearBottomRight, nearMiddleRight, farMiddleRight, farBottomRight };


    protected override FaceAttributes GetFrontFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z - 1, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.pos = frontVertices_snow;
        fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
        fa.color = GetFrontTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.forward;
        fa.uv = uv_bot;

        return fa;
    }
    protected override FaceAttributes GetBackFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z + 1, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.pos = backVertices_snow;
        fa.faceIndex = GetBackIndexByData(chunk, ca.blockData);
        fa.color = GetBackTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.back;
        fa.uv = uv_bot;

        return fa;
    }
    protected override FaceAttributes GetTopFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y + 1, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.pos = topVertices_snow;
        fa.faceIndex = GetTopIndexByData(chunk, ca.blockData);
        fa.color = GetTopTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.up;
        fa.uv = uv_full;

        return fa;
    }
    protected override FaceAttributes GetBottomFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y - 1, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa;
        fa.pos = bottomVertices_snow;
        fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
        fa.color = GetBottomTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.down;
        fa.uv = uv_full;

        return fa;
    }
    protected override FaceAttributes GetLeftFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x - 1, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa;
        fa.pos = leftVertices_snow;
        fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
        fa.color = GetLeftTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.left;
        fa.uv = uv_bot;

        return fa;
    }
    protected override FaceAttributes GetRightFaceAttributes(NBTChunk chunk, NBTMesh mesh, CubeAttributes ca)
    {
        chunk.GetLights(ca.pos.x + 1, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa;
        fa.pos = rightVertices_snow;
        fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
        fa.color = GetRightTintColorByData(chunk, ca.pos, ca.blockData);
        fa.skyLight = skyLight;
        fa.blockLight = blockLight;
        fa.normal = Vector3.right;
        fa.uv = uv_bot;

        return fa;
    }
}
