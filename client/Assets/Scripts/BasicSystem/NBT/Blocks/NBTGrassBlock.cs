using protocol.cs_theircraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTGrassBlock : NBTBlock
{
    public override string name { get { return "Grass Block"; } }
    public override string id { get { return "minecraft:grass"; } }

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
        CubeAttributes ca = new CubeAttributes()
        {
            pos = pos,
            blockData = blockData,
        };

        GetAOBlocks(chunk, ref ca);

        bool topIsSnow = chunk.GetBlockByte(pos.x, pos.y + 1, pos.z) == 78;

        UnityEngine.Profiling.Profiler.BeginSample("AddFaces");

        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z - 1))
        {
            FaceAttributes fa = GetFrontFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.faceIndex = TextureArrayManager.GetIndexByName("grass_side_snowed");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x + 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetRightFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.faceIndex = TextureArrayManager.GetIndexByName("grass_side_snowed");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x - 1, pos.y, pos.z))
        {
            FaceAttributes fa = GetLeftFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.faceIndex = TextureArrayManager.GetIndexByName("grass_side_snowed");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y, pos.z + 1))
        {
            FaceAttributes fa = GetBackFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.faceIndex = TextureArrayManager.GetIndexByName("grass_side_snowed");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y + 1, pos.z))
        {
            FaceAttributes fa = GetTopFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            if (topIsSnow)
                fa.faceIndex = TextureArrayManager.GetIndexByName("snow");
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        if (!chunk.HasOpaqueBlock(pos.x, pos.y - 1, pos.z))
        {
            FaceAttributes fa = GetBottomFaceAttributes(chunk, nbtGO.nbtMesh, ca);
            AddFace(nbtGO.nbtMesh, fa, ca);
        }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    public override Mesh GetItemMesh(byte data = 0)
    {
        CubeAttributes ca = new CubeAttributes();

        NBTMesh nbtMesh = new NBTMesh(256);

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = 1;
        fa.blockLight = 1;
        fa.color = Color.white;
        fa.uv = uv_zero;

        try
        {
            fa.pos = frontVertices;
            fa.normal = Vector3.forward;
            fa.faceIndex = TextureArrayManager.GetIndexByName(frontName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            fa.faceIndex = TextureArrayManager.GetIndexByName(backName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            fa.faceIndex = TextureArrayManager.GetIndexByName(topName);
            fa.color = TintManager.tintColor;
            AddFace(nbtMesh, fa, ca);
            fa.color = Color.white;

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            fa.faceIndex = TextureArrayManager.GetIndexByName(bottomName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            fa.faceIndex = TextureArrayManager.GetIndexByName(leftName);
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            fa.faceIndex = TextureArrayManager.GetIndexByName(rightName);
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
