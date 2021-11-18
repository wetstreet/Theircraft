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

    protected static Vector3[] frontVertices_south = new Vector3[] { nearBottomLeft, nearTopLeft, nearTopRight, nearBottomRight };
    protected static Vector3[] backVertices_south = new Vector3[] { farBottomRight_1, farTopRight_1, farTopLeft_1, farBottomLeft_1 };
    protected static Vector3[] topVertices_south = new Vector3[] { farTopRight_1, nearTopRight, nearTopLeft, farTopLeft_1 };
    protected static Vector3[] bottomVertices_south = new Vector3[] { nearBottomRight, farBottomRight_1, farBottomLeft_1, nearBottomLeft };
    protected static Vector3[] leftVertices_south = new Vector3[] { farBottomLeft_1, farTopLeft_1, nearTopLeft, nearBottomLeft };
    protected static Vector3[] rightVertices_south = new Vector3[] { nearBottomRight, nearTopRight, farTopRight_1, farBottomRight_1 };

    protected static Vector3[] frontVertices_north = new Vector3[] { nearBottomLeft_1, nearTopLeft_1, nearTopRight_1, nearBottomRight_1 };
    protected static Vector3[] backVertices_north = new Vector3[] { farBottomRight, farTopRight, farTopLeft, farBottomLeft };
    protected static Vector3[] topVertices_north = new Vector3[] { farTopRight, nearTopRight_1, nearTopLeft_1, farTopLeft };
    protected static Vector3[] bottomVertices_north = new Vector3[] { nearBottomRight_1, farBottomRight, farBottomLeft, nearBottomLeft_1 };
    protected static Vector3[] leftVertices_north = new Vector3[] { farBottomLeft, farTopLeft, nearTopLeft_1, nearBottomLeft_1 };
    protected static Vector3[] rightVertices_north = new Vector3[] { nearBottomRight_1, nearTopRight_1, farTopRight, farBottomRight };

    protected static Vector2[] uv_side = new Vector2[4] { new Vector2(0.8125f, 0), new Vector2(0.8125f, 1), Vector2.one, Vector2.right };

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

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = new CubeAttributes();
        ca.pos = pos;
        ca.blockData = blockData;

        chunk.GetLights(pos.x, pos.y, pos.z, out float skyLight, out float blockLight);

        FaceAttributes fa = new FaceAttributes();
        fa.color = Color.white;
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;
        fa.uv = uv_zero;

        bool isUpper = (blockData & 0b1000) > 0;

        int direction = 0;
        bool isOpen = false;

        if (isUpper)
        {
            chunk.GetBlockData(pos.x, pos.y - 1, pos.z, out byte belowType, out byte belowData);
            if (belowType == 64)
            {
                isOpen = (belowData & 0b0100) > 0;
                direction = belowData & 0b0011;
            }
        }
        else
        {
            isOpen = (blockData & 0b0100) > 0;
            direction = blockData & 0b0011;
        }


        try
        {
            fa.faceIndex = TextureArrayManager.GetIndexByName(isUpper ? "door_wood_upper" : "door_wood_lower");

            if (direction == 1)
                fa.pos = frontVertices_south;
            else
                fa.pos = frontVertices_north;
            AddFace(nbtGO.nbtMesh, fa, ca);

            if (direction == 1)
                fa.pos = backVertices_south;
            else
                fa.pos = backVertices_north;
            AddFace(nbtGO.nbtMesh, fa, ca);

            if (direction == 1)
                fa.pos = topVertices_south;
            else
                fa.pos = topVertices_north;
            AddFace(nbtGO.nbtMesh, fa, ca);

            if (direction == 1)
                fa.pos = bottomVertices_south;
            else
                fa.pos = bottomVertices_north;
            AddFace(nbtGO.nbtMesh, fa, ca);

            fa.uv = uv_side;

            if (direction == 1)
                fa.pos = leftVertices_south;
            else
                fa.pos = leftVertices_north;
            AddFace(nbtGO.nbtMesh, fa, ca);

            if (direction == 1)
                fa.pos = rightVertices_south;
            else
                fa.pos = rightVertices_north;
            AddFace(nbtGO.nbtMesh, fa, ca);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString() + "\n" + "pos=" + pos + ",data=" + blockData);
        }
    }
}