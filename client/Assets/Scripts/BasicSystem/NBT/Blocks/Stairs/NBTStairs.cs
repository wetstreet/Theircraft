using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStairs : NBTBlock
{
    public virtual string stairsName { get { return null; } }

    public override bool isTransparent => true;

    // east 0 -x
    // west 1 +x
    // south 2 -z
    // north 3 +z
    MeshData[] meshes = new MeshData[16];
    public override void Init()
    {
        meshes[0] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x").ToMeshData();
        meshes[1] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x").ToMeshData();
        meshes[2] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-z").ToMeshData();
        meshes[3] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+z").ToMeshData();
        meshes[4] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-y-x").ToMeshData();
        meshes[5] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-y+x").ToMeshData();
        meshes[6] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-y-z").ToMeshData();
        meshes[7] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_-y+z").ToMeshData();
        meshes[8] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x+z_inner").ToMeshData();
        meshes[9] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x+z_outer").ToMeshData();
        meshes[10] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x-z_inner").ToMeshData();
        meshes[11] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x-z_outer").ToMeshData();
        meshes[12] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x+z_inner").ToMeshData();
        meshes[13] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x+z_outer").ToMeshData();
        meshes[14] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x-z_inner").ToMeshData();
        meshes[15] = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x-z_outer").ToMeshData();
    }

    public override void AfterTextureInit()
    {
        Rect rect = TextureArrayManager.GetRectByName(stairsName);
        foreach (var mesh in meshes)
        {
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                Vector2 uv = new Vector2(rect.xMin + mesh.uv[i].x * rect.width, rect.yMin + mesh.uv[i].y * rect.height);
                mesh.uv[i] = uv;
            }
        }
    }

    MeshData GetMesh(NBTChunk chunk, Vector3Int localPosition, byte blockData)
    {
        MeshData mesh = meshes[blockData];

        if (blockData == 0)
        {
            chunk.GetBlockData(localPosition.x - 1, localPosition.y, localPosition.z, out byte leftType, out byte leftData);
            chunk.GetBlockData(localPosition.x + 1, localPosition.y, localPosition.z, out byte rightType, out byte rightData);
            if (rightType == 53)
            {
                if (rightData == 2)
                {
                    // +y-x-z_outer
                    mesh = meshes[11];
                }
                else if (rightData == 3)
                {
                    // +y-x+z_outer
                    mesh = meshes[9];
                }
            }
            else if (leftType == 53)
            {
                if (leftData == 2)
                {
                    // +y-x-z_inner
                    mesh = meshes[10];
                }
                else if (leftData == 3)
                {
                    // +y-x+z_inner
                    mesh = meshes[8];
                }
            }
        }
        else if (blockData == 1)
        {
            chunk.GetBlockData(localPosition.x - 1, localPosition.y, localPosition.z, out byte leftType, out byte leftData);
            chunk.GetBlockData(localPosition.x + 1, localPosition.y, localPosition.z, out byte rightType, out byte rightData);
            if (leftType == 53)
            {
                if (leftData == 2)
                {
                    // +y+x-z_outer
                    mesh = meshes[15];
                }
                else if (leftData == 3)
                {
                    // +y+x+z_outer
                    mesh = meshes[13];
                }
            }
            else if (rightType == 53)
            {
                if (rightData == 2)
                {
                    // +y+x-z_inner
                    mesh = meshes[14];
                }
                else if (rightData == 3)
                {
                    // +y+x+z_inner
                    mesh = meshes[12];
                }
            }
        }
        else if (blockData == 2)
        {
            chunk.GetBlockData(localPosition.x, localPosition.y, localPosition.z + 1, out byte backType, out byte backData);
            chunk.GetBlockData(localPosition.x, localPosition.y, localPosition.z - 1, out byte frontType, out byte frontData);
            if (backType == 53)
            {
                if (backData == 0)
                {
                    // +y-x-z_outer
                    mesh = meshes[11];
                }
                else if (backData == 1)
                {
                    // +y+x-z_outer
                    mesh = meshes[15];
                }
            }
            else if (frontType == 53)
            {
                if (frontData == 0)
                {
                    // +y-x-z_inner
                    mesh = meshes[10];
                }
                else if (frontData == 1)
                {
                    // +y+x-z_inner
                    mesh = meshes[14];
                }
            }
        }
        else if (blockData == 3)
        {
            chunk.GetBlockData(localPosition.x, localPosition.y, localPosition.z + 1, out byte backType, out byte backData);
            chunk.GetBlockData(localPosition.x, localPosition.y, localPosition.z - 1, out byte frontType, out byte frontData);
            if (frontType == 53)
            {
                if (frontData == 0)
                {
                    // +y-x+z_outer
                    mesh = meshes[9];
                }
                else if (frontData == 1)
                {
                    // +y+x+z_outer
                    mesh = meshes[13];
                }
            }
            else if (backType == 53)
            {
                if (backData == 0)
                {
                    // +y-x+z_inner
                    mesh = meshes[8];
                }
                else if (backData == 1)
                {
                    // +y+x+z_inner
                    mesh = meshes[12];
                }
            }
        }
        return mesh;
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        Vector3Int localPosition = pos - new Vector3Int(chunk.x, 0, chunk.z) * 16;
        return GetMesh(chunk, localPosition, blockData).mesh;
    }

    public override byte GetDropItemData(byte data) { return 0; }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int localPosition, NBTGameObject nbtGO)
    {
        MeshData mesh = GetMesh(chunk, localPosition, blockData);

        chunk.GetLights(localPosition.x, localPosition.y + 1, localPosition.z, out float skyLight, out float blockLight);

        NBTMesh nbtMesh = nbtGO.nbtMesh;
        int startIndex = nbtMesh.vertexCount;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            SetVertex(nbtMesh, mesh.vertices[i] + localPosition, mesh.uv[i], skyLight, blockLight, Color.white, Vector3.zero);
        }
        foreach (int index in mesh.triangles)
        {
            nbtMesh.triangleArray[nbtMesh.triangleCount++] = startIndex + index;
        }
    }

    public override Mesh GetItemMesh(byte data = 0)
    {
        return meshes[0].mesh;
    }

    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(0))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>("GUI/block/" + stairsName);
            mat.mainTexture = tex;

            itemMaterialDict.Add(0, mat);
        }
        return itemMaterialDict[0];
    }

    public override void OnAddBlock(RaycastHit hit)
    {
        Vector3Int pos = WireFrameHelper.pos + Vector3Int.RoundToInt(hit.normal);

        byte type = NBTGeneratorManager.id2type[id];
        byte data = 0;

        Vector3 playerPos = PlayerController.instance.position;
        Vector2 dir = (new Vector2(playerPos.x, playerPos.z) - new Vector2(pos.x, pos.z)).normalized;
        if (dir.x > 0)
        {
            if (dir.y > 0)
            {
                if (dir.y > dir.x)
                {
                    // positive z
                    data = 3;
                }
                else
                {
                    // positive x
                    data = 1;
                }
            }
            else
            {
                if (-dir.y > dir.x)
                {
                    // negative z
                    data = 2;
                }
                else
                {
                    // positive x
                    data = 1;
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
                    data = 3;
                }
                else
                {
                    // negative x
                    data = 0;
                }
            }
            else
            {
                if (-dir.y > -dir.x)
                {
                    // negative z
                    data = 2;
                }
                else
                {
                    // negative x
                    data = 0;
                }
            }
        }

        if (hit.point.y - pos.y > 0)
        {
            data += 4;
        }

        NBTHelper.SetBlockData(pos, type, data);
    }
}
