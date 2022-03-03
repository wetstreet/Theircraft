using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGrassPath : NBTBlock
{
    public override string name => "Grass Path";
    public override string id => "minecraft:grass_path";

    public override string GetDropItemByData(byte data) { return "minecraft:dirt"; }

    public override bool isTransparent => true;

    public override string topName => "grass_path_top";
    public override string bottomName => "dirt";
    public override string frontName => "grass_path_side";
    public override string backName => "grass_path_side";
    public override string leftName => "grass_path_side";
    public override string rightName => "grass_path_side";

    public override float hardness => 0.65f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;
    public override SoundMaterial soundMaterial => SoundMaterial.Grass;

    public override Color GetTopTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return Color.white; }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }

    protected static Vector3 nearTopLeft = new Vector3(-0.5f, 0.4375f, -0.5f);
    protected static Vector3 nearTopRight = new Vector3(0.5f, 0.4375f, -0.5f);
    protected static Vector3 farTopLeft = new Vector3(-0.5f, 0.4375f, 0.5f);
    protected static Vector3 farTopRight = new Vector3(0.5f, 0.4375f, 0.5f);

    protected static Vector3[] topVertices = new Vector3[] { farTopRight, nearTopRight, nearTopLeft, farTopLeft };


    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        InitBlockAttributes(chunk, ref ca);

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
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            fa.pos = topVertices;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
    }

    public override void RenderWireframe(byte blockData)
    {
        float top = 0.4385f;
        float bottom = -0.501f;
        float left = -0.501f;
        float right = 0.501f;
        float front = 0.501f;
        float back = -0.501f;

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }
}