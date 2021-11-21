using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTOakDoor : NBTBlock
{
    public override string name { get { return "Oak Door"; } }
    public override string id { get { return "minecraft:wooden_door"; } }

    public override string GetIconPathByData(short data) { return "door_wood"; }

    public override void Init()
    {
        UsedTextures = new string[] { "door_wood_lower", "door_wood_upper" };
    }

    public override string GetBreakEffectTexture(byte data) { return "door_wood_lower"; }

    public override bool isTransparent { get { return true; } }

    public override BlockMaterial blockMaterial => BlockMaterial.Wood;
    public override SoundMaterial soundMaterial => SoundMaterial.Wood;
    public override float hardness => 3f;

    protected static Vector3 nearBottomLeft_1 = new Vector3(-0.5f, -0.5f, 0.3125f);
    protected static Vector3 nearBottomRight_1 = new Vector3(0.5f, -0.5f, 0.3125f);
    protected static Vector3 nearTopLeft_1 = new Vector3(-0.5f, 0.5f, 0.3125f);
    protected static Vector3 nearTopRight_1 = new Vector3(0.5f, 0.5f, 0.3125f);
    protected static Vector3 farBottomLeft_1 = new Vector3(-0.5f, -0.5f, -0.3125f);
    protected static Vector3 farBottomRight_1 = new Vector3(0.5f, -0.5f, -0.3125f);
    protected static Vector3 farTopLeft_1 = new Vector3(-0.5f, 0.5f, -0.3125f);
    protected static Vector3 farTopRight_1 = new Vector3(0.5f, 0.5f, -0.3125f);

    protected static Vector3 nearBottomLeft_2 = new Vector3(0.3125f, -0.5f, -0.5f);
    protected static Vector3 nearBottomRight_2 = new Vector3(-0.3125f, -0.5f, -0.5f);
    protected static Vector3 nearTopLeft_2 = new Vector3(0.3125f, 0.5f, -0.5f);
    protected static Vector3 nearTopRight_2 = new Vector3(-0.3125f, 0.5f, -0.5f);
    protected static Vector3 farBottomLeft_2 = new Vector3(0.3125f, -0.5f, 0.5f);
    protected static Vector3 farBottomRight_2 = new Vector3(-0.3125f, -0.5f, 0.5f);
    protected static Vector3 farTopLeft_2 = new Vector3(0.3125f, 0.5f, 0.5f);
    protected static Vector3 farTopRight_2 = new Vector3(-0.3125f, 0.5f, 0.5f);

    protected static Vector3[] frontVertices_east = new Vector3[] { nearBottomLeft, nearTopLeft, nearTopRight_2, nearBottomRight_2 };
    protected static Vector3[] backVertices_east = new Vector3[] { farBottomRight_2, farTopRight_2, farTopLeft, farBottomLeft };
    protected static Vector3[] topVertices_east = new Vector3[] { farTopLeft, farTopRight_2, nearTopRight_2, nearTopLeft };
    protected static Vector3[] bottomVertices_east = new Vector3[] { nearBottomRight_2, farBottomRight_2, farBottomLeft, nearBottomLeft };
    protected static Vector3[] leftVertices_east = new Vector3[] { farBottomLeft, farTopLeft, nearTopLeft, nearBottomLeft };
    protected static Vector3[] rightVertices_east = new Vector3[] { nearBottomRight_2, nearTopRight_2, farTopRight_2, farBottomRight_2 };

    protected static Vector3[] frontVertices_south = new Vector3[] { nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight };
    protected static Vector3[] backVertices_south = new Vector3[] { farBottomRight_1, farTopRight_1, farTopLeft_1, farBottomLeft_1 };
    protected static Vector3[] topVertices_south = new Vector3[] { farTopRight_1, nearTopRight, nearTopLeft, farTopLeft_1 };
    protected static Vector3[] bottomVertices_south = new Vector3[] { nearBottomRight, farBottomRight_1, farBottomLeft_1, nearBottomLeft };
    protected static Vector3[] leftVertices_south = new Vector3[] { farBottomLeft_1, farTopLeft_1, nearTopLeft, nearBottomLeft };
    protected static Vector3[] rightVertices_south = new Vector3[] { nearBottomRight, nearTopRight, farTopRight_1, farBottomRight_1 };

    protected static Vector3[] frontVertices_west = new Vector3[] { nearBottomLeft_2, nearTopLeft_2, nearTopRight, nearBottomRight };
    protected static Vector3[] backVertices_west = new Vector3[] { farBottomRight, farTopRight, farTopLeft_2, farBottomLeft_2 };
    protected static Vector3[] topVertices_west = new Vector3[] { nearTopRight, nearTopLeft_2, farTopLeft_2, farTopRight };
    protected static Vector3[] bottomVertices_west = new Vector3[] { nearBottomRight, farBottomRight, farBottomLeft_2, nearBottomLeft_2 };
    protected static Vector3[] leftVertices_west = new Vector3[] { farBottomLeft_2, farTopLeft_2, nearTopLeft_2, nearBottomLeft_2 };
    protected static Vector3[] rightVertices_west = new Vector3[] { nearBottomRight, nearTopRight, farTopRight, farBottomRight };

    protected static Vector3[] frontVertices_north = new Vector3[] { nearBottomLeft_1, nearTopLeft_1, nearTopRight_1, nearBottomRight_1 };
    protected static Vector3[] backVertices_north = new Vector3[] { farBottomRight, farTopRight, farTopLeft, farBottomLeft };
    protected static Vector3[] topVertices_north = new Vector3[] { farTopRight, nearTopRight_1, nearTopLeft_1, farTopLeft };
    protected static Vector3[] bottomVertices_north = new Vector3[] { nearBottomRight_1, farBottomRight, farBottomLeft, nearBottomLeft_1 };
    protected static Vector3[] leftVertices_north = new Vector3[] { farBottomLeft, farTopLeft, nearTopLeft_1, nearBottomLeft_1 };
    protected static Vector3[] rightVertices_north = new Vector3[] { nearBottomRight_1, nearTopRight_1, farTopRight, farBottomRight };

    protected static Vector2[] uv_side = new Vector2[4] { Vector2.zero, Vector2.up, new Vector2(0.1875f, 1), new Vector2(0.1875f, 0) };
    protected static Vector2[] uv_top = new Vector2[4] { Vector2.zero, new Vector2(0, 0.1875f), new Vector2(1, 0.1875f), Vector2.right };

    public override void OnDestroyBlock(Vector3Int globalPos, byte blockData)
    {
        bool isUpper = (blockData & 0b1000) > 0;
        if (isUpper)
        {
            NBTHelper.SetBlockByteNoUpdate(globalPos - Vector3Int.up, 0);
        }
        else
        {
            NBTHelper.SetBlockByteNoUpdate(globalPos + Vector3Int.up, 0);
        }
    }

    // upper door
    // 0x1: 0 if hinge is on the left (the default), 1 if on the right
    // 0x2: 0 if unpowered, 1 if powered
    // 0x4: unused
    // 0x8: Always 1 for the upper part of a door.

    // lower door
    // 0x1,0x2: Two bits storing a value from 0 to 3 specifying the direction the door is facing:
    // 0: Facing east
    // 1: Facing south
    // 2: Facing west
    // 3: Facing north
    // 0x4: 0 if the entire door is closed, 1 if open.
    // 0x8: Always 0 for the lower part of a door.
    void FillMesh(NBTChunk chunk, CubeAttributes ca, NBTMesh nbtMesh)
    {
        chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.color = Color.white;
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;

        bool isUpper = (ca.blockData & 0b1000) > 0;

        int direction = 0;
        bool isOpen = false;

        if (isUpper)
        {
            chunk.GetBlockData(ca.pos.x, ca.pos.y - 1, ca.pos.z, out byte belowType, out byte belowData);
            if (belowType == 64)
            {
                isOpen = (belowData & 0b0100) > 0;
                direction = belowData & 0b0011;
            }
        }
        else
        {
            isOpen = (ca.blockData & 0b0100) > 0;
            direction = ca.blockData & 0b0011;
        }

        if (ca.isBreakingMesh)
        {
            ca.pos = Vector3Int.zero;
        }

        try
        {
            fa.faceIndex = TextureArrayManager.GetIndexByName(isUpper ? "door_wood_upper" : "door_wood_lower");

            if (direction == 0)
            {
                fa.uv = uv_top;
                fa.pos = topVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.pos = bottomVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.uv = uv_zero;
                fa.pos = leftVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.pos = rightVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.uv = uv_side;
                fa.pos = frontVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.pos = backVertices_east;
                AddFace(nbtMesh, fa, ca);
            }
            else if (direction == 1)
            {
                fa.uv = uv_zero;
                fa.pos = frontVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.pos = backVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.uv = uv_top;
                fa.pos = topVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.pos = bottomVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.uv = uv_side;
                fa.pos = leftVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.pos = rightVertices_south;
                AddFace(nbtMesh, fa, ca);
            }
            else if (direction == 2)
            {
                fa.uv = uv_top;
                fa.pos = topVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.pos = bottomVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.uv = uv_zero;
                fa.pos = leftVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.pos = rightVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.uv = uv_side;
                fa.pos = frontVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.pos = backVertices_west;
                AddFace(nbtMesh, fa, ca);
            }
            else
            {
                fa.uv = uv_zero;
                fa.pos = frontVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.pos = backVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.uv = uv_top;
                fa.pos = topVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.pos = bottomVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.uv = uv_side;
                fa.pos = leftVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.pos = rightVertices_north;
                AddFace(nbtMesh, fa, ca);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString() + "\n" + "pos=" + ca.pos + ",data=" + ca.blockData);
        }
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = new Vector3Int(pos.x - chunk.x * 16, pos.y, pos.z - chunk.z * 16);
        ca.worldPos = pos;
        ca.blockData = blockData;
        ca.isBreakingMesh = true;

        NBTMesh nbtMesh = new NBTMesh(256);

        FillMesh(chunk, ca, nbtMesh);


        nbtMesh.Refresh();

        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.blockData = blockData;

        FillMesh(chunk, ca, nbtGO.nbtMesh);
    }
}