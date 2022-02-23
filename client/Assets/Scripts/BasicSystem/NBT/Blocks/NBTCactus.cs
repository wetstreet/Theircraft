using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTCactus : NBTBlock
{
    public override string name => "Cactus";
    public override string id => "minecraft:cactus";

    public override bool isTransparent => true;

    public override float hardness => 0.4f;

    public override SoundMaterial soundMaterial => SoundMaterial.Grass;

    public override string topName => "cactus_top";
    public override string bottomName => "cactus_bottom";
    public override string frontName => "cactus_side";
    public override string backName => "cactus_side";
    public override string leftName => "cactus_side";
    public override string rightName => "cactus_side";

    protected static Vector3 nearBottomLeft_1 = new Vector3(-0.5f, -0.5f, -0.4375f);
    protected static Vector3 nearBottomRight_1 = new Vector3(0.5f, -0.5f, -0.4375f);
    protected static Vector3 nearTopLeft_1 = new Vector3(-0.5f, 0.5f, -0.4375f);
    protected static Vector3 nearTopRight_1 = new Vector3(0.5f, 0.5f, -0.4375f);
    protected static Vector3 farBottomLeft_1 = new Vector3(-0.5f, -0.5f, 0.4375f);
    protected static Vector3 farBottomRight_1 = new Vector3(0.5f, -0.5f, 0.4375f);
    protected static Vector3 farTopLeft_1 = new Vector3(-0.5f, 0.5f, 0.4375f);
    protected static Vector3 farTopRight_1 = new Vector3(0.5f, 0.5f, 0.4375f);

    protected static Vector3 nearBottomLeft_2 = new Vector3(-0.4375f, -0.5f, -0.5f);
    protected static Vector3 nearBottomRight_2 = new Vector3(0.4375f, -0.5f, -0.5f);
    protected static Vector3 nearTopLeft_2 = new Vector3(-0.4375f, 0.5f, -0.5f);
    protected static Vector3 nearTopRight_2 = new Vector3(0.4375f, 0.5f, -0.5f);
    protected static Vector3 farBottomLeft_2 = new Vector3(-0.4375f, -0.5f, 0.5f);
    protected static Vector3 farBottomRight_2 = new Vector3(0.4375f, -0.5f, 0.5f);
    protected static Vector3 farTopLeft_2 = new Vector3(-0.4375f, 0.5f, 0.5f);
    protected static Vector3 farTopRight_2 = new Vector3(0.4375f, 0.5f, 0.5f);

    protected static Vector3[] frontVertices = new Vector3[] { nearBottomLeft_1, nearTopLeft_1, nearTopRight_1, nearBottomRight_1 };
    protected static Vector3[] backVertices = new Vector3[] { farBottomRight_1, farTopRight_1, farTopLeft_1, farBottomLeft_1 };
    protected static Vector3[] topVertices = new Vector3[] { farTopRight, nearTopRight, nearTopLeft, farTopLeft };
    protected static Vector3[] bottomVertices = new Vector3[] { nearBottomRight, farBottomRight, farBottomLeft, nearBottomLeft };
    protected static Vector3[] leftVertices = new Vector3[] { farBottomLeft_2, farTopLeft_2, nearTopLeft_2, nearBottomLeft_2 };
    protected static Vector3[] rightVertices = new Vector3[] { nearBottomRight_2, nearTopRight_2, farTopRight_2, farBottomRight_2 };

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int localPosition, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = localPosition;
        ca.worldPos = new Vector3Int(localPosition.x + chunk.x * 16, localPosition.y, localPosition.z + chunk.z * 16);
        ca.blockData = blockData;

        InitBlockAttributes(chunk, ref ca);

        chunk.GetLights(localPosition.x, localPosition.y, localPosition.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };

        fa.uv = uv_zero;

        fa.pos = frontVertices;
        fa.normal = Vector3.forward;
        fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
        fa.color = GetFrontTintColorByData(chunk, localPosition, ca.blockData);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = backVertices;
        fa.normal = Vector3.back;
        fa.faceIndex = GetBackIndexByData(chunk, ca.blockData);
        fa.color = GetBackTintColorByData(chunk, localPosition, ca.blockData);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = topVertices;
        fa.normal = Vector3.up;
        fa.faceIndex = GetTopIndexByData(chunk, ca.blockData);
        fa.color = GetTopTintColorByData(chunk, localPosition, ca.blockData);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = bottomVertices;
        fa.normal = Vector3.down;
        fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
        fa.color = GetBottomTintColorByData(chunk, localPosition, ca.blockData);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = leftVertices;
        fa.normal = Vector3.left;
        fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
        fa.color = GetLeftTintColorByData(chunk, localPosition, ca.blockData);
        AddFace(nbtGO.nbtMesh, fa, ca);

        fa.pos = rightVertices;
        fa.normal = Vector3.right;
        fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
        fa.color = GetRightTintColorByData(chunk, localPosition, ca.blockData);
        AddFace(nbtGO.nbtMesh, fa, ca);
    }

    public override Mesh GetItemMesh(byte data = 0)
    {
        CubeAttributes ca = new CubeAttributes();

        NBTMesh nbtMesh = new NBTMesh(256);

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = skylight_default;
        fa.blockLight = blocklight_default;

        try
        {
            fa.pos = frontVertices;
            fa.normal = Vector3.forward;
            fa.faceIndex = GetFrontIndexByData(null, data);
            fa.uv = GetFrontRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetFrontTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            fa.faceIndex = GetBackIndexByData(null, data);
            fa.uv = GetBackRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetBackTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            fa.faceIndex = GetTopIndexByData(null, data);
            fa.uv = GetTopRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetTopTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            fa.faceIndex = GetBottomIndexByData(null, data);
            fa.uv = GetBottomRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetBottomTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            fa.faceIndex = GetLeftIndexByData(null, data);
            fa.uv = GetLeftRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetLeftTintColorByData(data);
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            fa.faceIndex = GetRightIndexByData(null, data);
            fa.uv = GetRightRotationByData(data) == Rotation.Right ? uv_right : uv_zero;
            fa.color = GetRightTintColorByData(data);
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

    // for break effect & drop item (will be affected by light)
    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        CubeAttributes ca = new CubeAttributes();
        //ca.pos = pos;
        ca.blockData = blockData;

        NBTMesh nbtMesh = new NBTMesh(256);

        chunk.GetLights(pos.x - chunk.x * 16, pos.y, pos.z - chunk.z * 16, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };

        fa.uv = uv_zero;

        try
        {
            fa.pos = frontVertices;
            fa.normal = Vector3.forward;
            fa.faceIndex = GetFrontIndexByData(chunk, ca.blockData);
            fa.color = GetFrontTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = backVertices;
            fa.normal = Vector3.back;
            fa.faceIndex = GetBackIndexByData(chunk, ca.blockData);
            fa.color = GetBackTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = topVertices;
            fa.normal = Vector3.up;
            fa.faceIndex = GetTopIndexByData(chunk, ca.blockData);
            fa.color = GetTopTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = bottomVertices;
            fa.normal = Vector3.down;
            fa.faceIndex = GetBottomIndexByData(chunk, ca.blockData);
            fa.color = GetBottomTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = leftVertices;
            fa.normal = Vector3.left;
            fa.faceIndex = GetLeftIndexByData(chunk, ca.blockData);
            fa.color = GetLeftTintColorByData(chunk, pos, ca.blockData);
            AddFace(nbtMesh, fa, ca);

            fa.pos = rightVertices;
            fa.normal = Vector3.right;
            fa.faceIndex = GetRightIndexByData(chunk, ca.blockData);
            fa.color = GetRightTintColorByData(chunk, pos, ca.blockData);
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
