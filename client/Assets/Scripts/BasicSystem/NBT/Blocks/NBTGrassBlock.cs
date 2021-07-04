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

    public override void Init()
    {
        UsedTextures = new string[] { "grass_top", "grass_side", "dirt", "snow", "grass_side_snowed" };
    }

    public override string topName { get { return "grass_top"; } }
    public override string bottomName { get { return "dirt"; } }
    public override string frontName { get { return "grass_side"; } }
    public override string backName { get { return "grass_side"; } }
    public override string leftName { get { return "grass_side"; } }
    public override string rightName { get { return "grass_side"; } }

    public override float hardness { get { return 0.6f; } }

    public override Color GetTopTintColorByData(NBTChunk chunk, Vector3Int pos, byte data) { return TintManager.tintColor; }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override string GetBreakEffectTexture(byte data) { return "dirt"; }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        bool topIsSnow = chunk.GetBlockByte(pos.x, pos.y + 1, pos.z) == 78;
        if (topIsSnow)
        {
            topIndex = TextureArrayManager.GetIndexByName("snow");
            frontIndex = TextureArrayManager.GetIndexByName("grass_side_snowed");
            backIndex = TextureArrayManager.GetIndexByName("grass_side_snowed");
            leftIndex = TextureArrayManager.GetIndexByName("grass_side_snowed");
            rightIndex = TextureArrayManager.GetIndexByName("grass_side_snowed");
        }
        else
        {
            topIndex = TextureArrayManager.GetIndexByName("grass_top");
            frontIndex = TextureArrayManager.GetIndexByName("grass_side");
            backIndex = TextureArrayManager.GetIndexByName("grass_side");
            leftIndex = TextureArrayManager.GetIndexByName("grass_side");
            rightIndex = TextureArrayManager.GetIndexByName("grass_side");

        }
        topColor = GetTopTintColorByData(chunk, pos, blockData);
        bottomIndex = TextureArrayManager.GetIndexByName("dirt");

        UnityEngine.Profiling.Profiler.BeginSample("AddFaces");

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z - 1);
            AddFrontFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x + 1, pos.y, pos.z);
            AddRightFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x - 1, pos.y, pos.z);
            AddLeftFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y, pos.z + 1);
            AddBackFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y + 1, pos.z);
            AddTopFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            float skyLight = chunk.GetSkyLight(pos.x, pos.y - 1, pos.z);
            AddBottomFace(nbtGO.nbtMesh, pos, blockData, skyLight);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }
}
