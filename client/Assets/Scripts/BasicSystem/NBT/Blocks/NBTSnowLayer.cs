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
        CubeAttributes ca = InitCubeAttributes(chunk, blockData, pos);

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z - 1);
            AddFrontFace(nbtGO.nbtMesh, ca, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x + 1, pos.y, pos.z);
            AddRightFace(nbtGO.nbtMesh, ca, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x - 1, pos.y, pos.z);
            AddLeftFace(nbtGO.nbtMesh, ca, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z + 1);
            AddBackFace(nbtGO.nbtMesh, ca, skyLight);
        }

        float selfSkyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z);
        AddTopFace(nbtGO.nbtMesh, ca, selfSkyLight);

        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y - 1, pos.z);
            AddBottomFace(nbtGO.nbtMesh, ca, skyLight);
        }
    }

    static Vector2 leftMid = new Vector2(0, 0.125f);
    static Vector2 rightMid = new Vector2(1, 0.125f);

    Vector2[] uv_full = new Vector2[4] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
    Vector2[] uv_bot = new Vector2[4] { Vector2.zero, leftMid, rightMid, Vector2.right };

    protected void AddFace(NBTMesh mesh, Vector3Int pos,
        Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4,
        Vector2[] uv, int faceIndex, Color color, float skyLight, Vector3 normal)
    {
        ushort startIndex = mesh.vertexCount;

        SetVertex(mesh, pos1 + pos, faceIndex, uv[0], skyLight, color, normal);
        SetVertex(mesh, pos2 + pos, faceIndex, uv[1], skyLight, color, normal);
        SetVertex(mesh, pos3 + pos, faceIndex, uv[2], skyLight, color, normal);
        SetVertex(mesh, pos4 + pos, faceIndex, uv[3], skyLight, color, normal);

        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 1);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = startIndex;
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 2);
        mesh.triangleArray[mesh.triangleCount++] = (ushort)(startIndex + 3);
    }

    protected override void AddFrontFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetFrontRotationByData(ca.blockData);
        AddFace(mesh, ca.pos, nearBottomLeft, nearMiddleLeft, nearMiddleRight, nearBottomRight, uv_bot, ca.frontIndex, ca.frontColor, skyLight, Vector3.forward);
    }

    protected override void AddBackFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetBackRotationByData(ca.blockData);
        AddFace(mesh, ca.pos, farBottomRight, farMiddleRight, farMiddleLeft, farBottomLeft, uv_bot, ca.backIndex, ca.backColor, skyLight, Vector3.back);
    }

    protected override void AddTopFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetTopRotationByData(ca.blockData);
        AddFace(mesh, ca.pos, farMiddleRight, nearMiddleRight, nearMiddleLeft, farMiddleLeft, uv_full, ca.topIndex, ca.topColor, skyLight, Vector3.up);
    }

    protected override void AddBottomFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetBottomRotationByData(ca.blockData);
        AddFace(mesh, ca.pos, nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft, uv_full, ca.bottomIndex, ca.bottomColor, skyLight, Vector3.down);
    }

    protected override void AddLeftFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetLeftRotationByData(ca.blockData);
        AddFace(mesh, ca.pos, farBottomLeft, farMiddleLeft, nearMiddleLeft, nearBottomLeft, uv_bot, ca.leftIndex, ca.leftColor, skyLight, Vector3.left);
    }

    protected override void AddRightFace(NBTMesh mesh, CubeAttributes ca, float skyLight)
    {
        rotation = GetRightRotationByData(ca.blockData);
        AddFace(mesh, ca.pos, nearBottomRight, nearMiddleRight, farMiddleRight, farBottomRight, uv_bot, ca.rightIndex, ca.rightColor, skyLight, Vector3.right);
    }
}
