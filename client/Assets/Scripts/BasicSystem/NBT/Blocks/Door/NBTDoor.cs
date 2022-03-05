using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTDoor : NBTBlock
{
    public override string GetIconPathByData(short data) { return "door_wood"; }

    public override byte GetDropItemData(byte data) { return 0; }

    public override string GetBreakEffectTexture(byte data) { return "door_wood_lower"; }

    public override string pathPrefix => "GUI/items/";

    public virtual string upperName => "door_wood_upper";
    public virtual string lowerName => "door_wood_lower";

    public override bool isTransparent => true;

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
    protected static Vector2[] uv_side_flip = new Vector2[4] { new Vector2(0.1875f, 0), new Vector2(0.1875f, 1), Vector2.up, Vector2.zero };
    protected static Vector2[] uv_top = new Vector2[4] { Vector2.zero, new Vector2(0, 0.1875f), new Vector2(1, 0.1875f), Vector2.right };
    protected static Vector2[] uv_flip = new Vector2[4] { Vector2.right, Vector2.one, Vector2.up, Vector2.zero };

    protected static Vector2[] uv_upper_zero = new Vector2[4];
    protected static Vector2[] uv_upper_side = new Vector2[4];
    protected static Vector2[] uv_upper_side_flip = new Vector2[4];
    protected static Vector2[] uv_upper_top = new Vector2[4];
    protected static Vector2[] uv_upper_flip = new Vector2[4];
    protected static Vector2[] uv_lower_zero = new Vector2[4];
    protected static Vector2[] uv_lower_side = new Vector2[4];
    protected static Vector2[] uv_lower_side_flip = new Vector2[4];
    protected static Vector2[] uv_lower_top = new Vector2[4];
    protected static Vector2[] uv_lower_flip = new Vector2[4];

    public override void AfterTextureInit()
    {
        Rect rect_upper = TextureArrayManager.GetRectByName(upperName);
        Rect rect_lower = TextureArrayManager.GetRectByName(lowerName);
        for (int i = 0; i < 4; i++)
        {
            uv_upper_zero[i] = new Vector2(rect_upper.xMin + uv_zero[i].x * rect_upper.width,
                                           rect_upper.yMin + uv_zero[i].y * rect_upper.height);
            uv_upper_side[i] = new Vector2(rect_upper.xMin + uv_side[i].x * rect_upper.width,
                                           rect_upper.yMin + uv_side[i].y * rect_upper.height);
            uv_upper_side_flip[i] = new Vector2(rect_upper.xMin + uv_side_flip[i].x * rect_upper.width,
                                                rect_upper.yMin + uv_side_flip[i].y * rect_upper.height);
            uv_upper_top[i] = new Vector2(rect_upper.xMin + uv_top[i].x * rect_upper.width,
                                            rect_upper.yMin + uv_top[i].y * rect_upper.height);
            uv_upper_flip[i] = new Vector2(rect_upper.xMin + uv_flip[i].x * rect_upper.width,
                                            rect_upper.yMin + uv_flip[i].y * rect_upper.height);
            uv_lower_zero[i] = new Vector2(rect_lower.xMin + uv_zero[i].x * rect_lower.width,
                                           rect_lower.yMin + uv_zero[i].y * rect_lower.height);
            uv_lower_side[i] = new Vector2(rect_lower.xMin + uv_side[i].x * rect_lower.width,
                                           rect_lower.yMin + uv_side[i].y * rect_lower.height);
            uv_lower_side_flip[i] = new Vector2(rect_lower.xMin + uv_side_flip[i].x * rect_lower.width,
                                                rect_lower.yMin + uv_side_flip[i].y * rect_lower.height);
            uv_lower_top[i] = new Vector2(rect_lower.xMin + uv_top[i].x * rect_lower.width,
                                            rect_lower.yMin + uv_top[i].y * rect_lower.height);
            uv_lower_flip[i] = new Vector2(rect_lower.xMin + uv_flip[i].x * rect_lower.width,
                                            rect_lower.yMin + uv_flip[i].y * rect_lower.height);
        }
    }

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

    enum DoorDirection
    {
        East,
        South,
        West,
        North
    }

    public override bool canInteract => true;
    public override void OnRightClick()
    {
        bool isUpper = (WireFrameHelper.data & 0b1000) > 0;

        int direction = 0;
        bool isOpen = false;
        if (isUpper)
        {
            NBTHelper.GetBlockData(WireFrameHelper.pos.x, WireFrameHelper.pos.y - 1, WireFrameHelper.pos.z, out byte belowType, out byte belowData);
            NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(belowType);
            if (generator is NBTDoor)
            {
                isOpen = (belowData & 0b0100) > 0;
                direction = belowData & 0b0011;
            }

            byte newData = (byte)(belowData ^ 0b0100);
            NBTHelper.SetBlockData(WireFrameHelper.pos + Vector3Int.down, WireFrameHelper.type, newData);
        }
        else
        {
            isOpen = (WireFrameHelper.data & 0b0100) > 0;

            byte newData = (byte)(WireFrameHelper.data ^ 0b0100);
            NBTHelper.SetBlockData(WireFrameHelper.pos, WireFrameHelper.type, newData);
        }
        SoundManager.Play2DSound(isOpen ? "Player_Door_Close" : "Player_Door_Open");

        NBTChunk chunk = NBTHelper.GetChunk(WireFrameHelper.pos);
        chunk.RebuildMesh(UpdateFlags.Collidable);
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
        float skyLight, blockLight;
        if (ca.isItemMesh)
        {
            chunk.GetLights(ca.worldPos.x, ca.worldPos.y, ca.worldPos.z, out skyLight, out blockLight);
        }
        else
        {
            chunk.GetLights(ca.pos.x, ca.pos.y, ca.pos.z, out skyLight, out blockLight);
        }

        FaceAttributes fa = new FaceAttributes();
        fa.color = Color.white;
        fa.skyLight = new float[] { skyLight, skyLight, skyLight, skyLight };
        fa.blockLight = new float[] { blockLight, blockLight, blockLight, blockLight };
        fa.normal = Vector3.zero;


        bool isUpper = (ca.blockData & 0b1000) > 0;

        int hinge;

        DoorDirection direction = 0;
        bool isOpen = false;

        if (isUpper)
        {
            chunk.GetBlockData(ca.pos.x, ca.pos.y - 1, ca.pos.z, out byte belowType, out byte belowData);
            NBTBlock generator = NBTGeneratorManager.GetMeshGenerator(belowType);
            if (generator is NBTDoor)
            {
                isOpen = (belowData & 0b0100) > 0;
                direction = (DoorDirection)(belowData & 0b0011);
            }
            hinge = (ca.blockData & 0b0001);
        }
        else
        {
            isOpen = (ca.blockData & 0b0100) > 0;
            direction = (DoorDirection)(ca.blockData & 0b0011);
            chunk.GetBlockData(ca.pos.x, ca.pos.y + 1, ca.pos.z, out byte aboveType, out byte aboveData);
            hinge = (aboveData & 0b0001);
        }

        if (ca.isBreakingMesh)
        {
            ca.pos = Vector3Int.zero;
        }

        try
        {
            if ((direction == DoorDirection.East && isOpen == false)
                || (direction == DoorDirection.South && isOpen && hinge == 1)
                || (direction == DoorDirection.North && isOpen && hinge == 0))
            {
                fa.uv = (isUpper ? uv_upper_top : uv_lower_top);
                fa.pos = topVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.pos = bottomVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.uv = hinge == 0 ? (isUpper ? uv_upper_flip : uv_lower_flip) : (isUpper ? uv_upper_zero : uv_lower_zero);
                fa.pos = leftVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.pos = rightVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.uv = hinge == 0 ? (isUpper ? uv_upper_side_flip : uv_lower_side_flip) : (isUpper ? uv_upper_side : uv_lower_side);
                fa.pos = frontVertices_east;
                AddFace(nbtMesh, fa, ca);
                fa.pos = backVertices_east;
                AddFace(nbtMesh, fa, ca);
            }
            else if ((direction == DoorDirection.South && isOpen == false)
                || (direction == DoorDirection.East && isOpen && hinge == 0)
                || (direction == DoorDirection.West && isOpen && hinge == 1))
            {
                fa.uv = hinge == 0 ? (isUpper ? uv_upper_flip : uv_lower_flip) : (isUpper ? uv_upper_zero : uv_lower_zero);
                fa.pos = frontVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.pos = backVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.uv = (isUpper ? uv_upper_top : uv_lower_top);
                fa.pos = topVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.pos = bottomVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.uv = hinge == 0 ? (isUpper ? uv_upper_side_flip : uv_lower_side_flip) : (isUpper ? uv_upper_side : uv_lower_side);
                fa.pos = leftVertices_south;
                AddFace(nbtMesh, fa, ca);
                fa.pos = rightVertices_south;
                AddFace(nbtMesh, fa, ca);
            }
            else if ((direction == DoorDirection.West && isOpen == false)
                || (direction == DoorDirection.South && isOpen && hinge == 0)
                || (direction == DoorDirection.North && isOpen && hinge == 1))
            {
                fa.uv = (isUpper ? uv_upper_top : uv_lower_top);
                fa.pos = topVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.pos = bottomVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.uv = hinge == 0 ? (isUpper ? uv_upper_flip : uv_lower_flip) : (isUpper ? uv_upper_zero : uv_lower_zero);
                fa.pos = leftVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.pos = rightVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.uv = hinge == 0 ? (isUpper ? uv_upper_side_flip : uv_lower_side_flip) : (isUpper ? uv_upper_side : uv_lower_side);
                fa.pos = frontVertices_west;
                AddFace(nbtMesh, fa, ca);
                fa.pos = backVertices_west;
                AddFace(nbtMesh, fa, ca);
            }
            else if ((direction == DoorDirection.North && isOpen == false)
                || (direction == DoorDirection.East && isOpen && hinge == 1)
                || (direction == DoorDirection.West && isOpen && hinge == 0))
            {
                fa.uv = hinge == 0 ? (isUpper ? uv_upper_flip : uv_lower_flip) : (isUpper ? uv_upper_zero : uv_lower_zero);
                fa.pos = frontVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.pos = backVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.uv = (isUpper ? uv_upper_top : uv_lower_top);
                fa.pos = topVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.pos = bottomVertices_north;
                AddFace(nbtMesh, fa, ca);
                fa.uv = hinge == 0 ? (isUpper ? uv_upper_side_flip : uv_lower_side_flip) : (isUpper ? uv_upper_side : uv_lower_side);
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

    public override Mesh GetBreakingEffectMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos.Set(pos.x - chunk.x * 16, pos.y, pos.z - chunk.z * 16);
        ca.worldPos = pos;
        ca.blockData = blockData;
        ca.isBreakingMesh = true;

        NBTMesh nbtMesh = new NBTMesh(256);

        FillMesh(chunk, ca, nbtMesh);

        nbtMesh.Refresh();
        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        CubeAttributes ca = chunk.ca;
        ca.worldPos = pos;
        ca.blockData = blockData;
        ca.isItemMesh = true;

        NBTMesh nbtMesh = new NBTMesh(256);

        ca.pos = Vector3Int.up;
        ca.blockData = 0b1000;
        FillMesh(chunk, ca, nbtMesh);
        ca.pos = Vector3Int.zero;
        ca.blockData = 0b0000;
        FillMesh(chunk, ca, nbtMesh);

        nbtMesh.Refresh();
        nbtMesh.Dispose();

        return nbtMesh.mesh;
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        CubeAttributes ca = chunk.ca;
        ca.pos = pos;
        ca.blockData = blockData;

        FillMesh(chunk, ca, nbtGO.nbtMesh);
    }

    public override void OnAddBlock(RaycastHit hit)
    {
        if (hit.normal != Vector3.up)
        {
            return;
        }

        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        byte upperType = NBTHelper.GetBlockByte(pos + Vector3Int.up);
        byte lowerType = NBTHelper.GetBlockByte(pos);

        if (upperType != 0 || lowerType != 0)
        {
            return;
        }

        byte type = NBTGeneratorManager.id2type[id];
        byte upperData = 0b1000;
        byte lowerData = 0b0000;

        Vector3 playerPos = PlayerController.instance.position;
        Vector2 dir = (new Vector2(playerPos.x, playerPos.z) - new Vector2(pos.x, pos.z)).normalized;

        Vector3 diff = WireFrameHelper.hitPos - pos;

        if (dir.x > 0)
        {
            if (dir.y > 0)
            {
                if (dir.y > dir.x)
                {
                    // positive z
                    lowerData = 3;
                    if (diff.x > 0) upperData |= 0b0001;
                }
                else
                {
                    // positive x
                    lowerData = 2;
                    if (diff.z < 0) upperData |= 0b0001;
                }
            }
            else
            {
                if (-dir.y > dir.x)
                {
                    // negative z
                    lowerData = 1;
                    if (diff.x < 0) upperData |= 0b0001;
                }
                else
                {
                    // positive x
                    lowerData = 2;
                    if (diff.z < 0) upperData |= 0b0001;
                }
            }
        }
        else
        {
            if (dir.y > 0)
            {
                if (dir.y > -dir.x)
                {
                    // positive z
                    lowerData = 3;
                    if (diff.x > 0) upperData |= 0b0001;
                }
                else
                {
                    // negative x
                    lowerData = 0;
                    if (diff.z > 0) upperData |= 0b0001;
                }
            }
            else
            {
                if (-dir.y > -dir.x)
                {
                    // negative z
                    lowerData = 1;
                    if (diff.x < 0) upperData |= 0b0001;
                }
                else
                {
                    // negative x
                    lowerData = 0;
                    if (diff.z > 0) upperData |= 0b0001;
                }
            }
        }

        NBTHelper.SetBlockData(pos + Vector3Int.up, type, upperData);
        NBTHelper.SetBlockData(pos, type, lowerData);
    }

    public override void RenderWireframe(byte blockData)
    {
        bool isUpper = (blockData & 0b1000) > 0;

        DoorDirection direction = 0;
        bool isOpen = false;
        int hinge;

        if (isUpper)
        {
            NBTHelper.GetBlockData(WireFrameHelper.pos.x, WireFrameHelper.pos.y - 1, WireFrameHelper.pos.z, out byte belowType, out byte belowData);
            if (belowType == 64)
            {
                isOpen = (belowData & 0b0100) > 0;
                direction = (DoorDirection)(belowData & 0b0011);
            }
            hinge = (blockData & 0b0001);
        }
        else
        {
            isOpen = (blockData & 0b0100) > 0;
            direction = (DoorDirection)(blockData & 0b0011);
            NBTHelper.GetBlockData(WireFrameHelper.pos.x, WireFrameHelper.pos.y + 1, WireFrameHelper.pos.z, out byte aboveType, out byte aboveData);
            hinge = (aboveData & 0b0001);
        }

        float top, bottom, left, right, front, back;
        top = 0.501f;
        bottom = -0.501f;
        if ((direction == DoorDirection.East && isOpen == false)
            || (direction == DoorDirection.South && isOpen && hinge == 1)
            || (direction == DoorDirection.North && isOpen && hinge == 0))
        {
            left = -0.501f;
            right = -0.3115f;
            front = 0.501f;
            back = -0.501f;
        }
        else if ((direction == DoorDirection.South && isOpen == false)
            || (direction == DoorDirection.East && isOpen && hinge == 0)
            || (direction == DoorDirection.West && isOpen && hinge == 1))
        {
            left = -0.501f;
            right = 0.501f;
            front = -0.3115f;
            back = -0.501f;
        }
        else if ((direction == DoorDirection.West && isOpen == false)
            || (direction == DoorDirection.South && isOpen && hinge == 0)
            || (direction == DoorDirection.North && isOpen && hinge == 1))
        {
            left = 0.3115f;
            right = 0.501f;
            front = 0.501f;
            back = -0.501f;
        }
        else if ((direction == DoorDirection.North && isOpen == false)
            || (direction == DoorDirection.East && isOpen && hinge == 1)
            || (direction == DoorDirection.West && isOpen && hinge == 0))
        {
            left = -0.501f;
            right = 0.501f;
            front = 0.3115f;
            back = 0.501f;
        }
        else
        {
            left = -0.501f;
            right = 0.501f;
            front = 0.501f;
            back = -0.501f;
        }

        RenderWireframeByVertex(top, bottom, left, right, front, back);
    }
}