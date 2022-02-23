using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGrassBlock : NBTBlock
{
    public override string name => "Grass Block";
    public override string id => "minecraft:grass";

    public override string GetDropItemByData(byte data) { return "minecraft:dirt"; }

    public override string topName => "grass_top";
    public override string bottomName => "dirt";
    public override string frontName => "grass_side";
    public override string backName => "grass_side";
    public override string leftName => "grass_side";
    public override string rightName => "grass_side";

    public override float hardness => 0.6f;

    public override BlockMaterial blockMaterial => BlockMaterial.Ground;

    public override SoundMaterial soundMaterial => SoundMaterial.Grass;

    public override Color GetTopTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return TintManager.tintColor; }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }


    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        InitBlockAttributes(chunk, ref ca);

        bool topIsSnow = chunk.GetBlockByte(pos.x, pos.y + 1, pos.z) == 78;

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.uv = TextureArrayManager.GetUVByName("grass_side_snowed");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.uv = TextureArrayManager.GetUVByName("grass_side_snowed");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.uv = TextureArrayManager.GetUVByName("grass_side_snowed");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.uv = TextureArrayManager.GetUVByName("grass_side_snowed");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.uv = TextureArrayManager.GetUVByName("snow");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
    }

    public override Mesh GetItemMesh(byte data = 0)
    {
        CubeAttributes ca = new CubeAttributes();

        NBTMesh nbtMesh = new NBTMesh(256);

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = skylight_default;
        fa.blockLight = blocklight_default;
        fa.color = Color.white;
        fa.uv = uv_zero;

        try
        {
            fa.pos = frontVertices;
            fa.normal = Vector3.forward;
            fa.uv = TextureArrayManager.GetUVByName(frontName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            fa.uv = TextureArrayManager.GetUVByName(backName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            fa.uv = TextureArrayManager.GetUVByName(topName);
            fa.color = TintManager.tintColor;
            AddFace(nbtMesh, fa, ca);
            fa.color = Color.white;

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            fa.uv = TextureArrayManager.GetUVByName(bottomName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            fa.uv = TextureArrayManager.GetUVByName(leftName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            fa.uv = TextureArrayManager.GetUVByName(rightName);
            AddFace(nbtMesh, fa, ca);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetItemMesh error,generator=" + GetType() + ",message=\n" + e.Message);
        }

        nbtMesh.Refresh();

        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }
}
