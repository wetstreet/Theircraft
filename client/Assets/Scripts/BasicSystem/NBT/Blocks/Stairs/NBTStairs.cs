using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTStairs : NBTBlock
{
    public virtual string stairsName { get { return null; } }

    public override bool isTransparent => true;

    Mesh GetMesh(byte blockData)
    {
        Mesh mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-x");
        switch (blockData)
        {
            case 1:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+x");
                break;
            case 2:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y-z");
                break;
            case 3:
                mesh = Resources.Load<Mesh>("Meshes/blocks/stair/stair_+y+z");
                break;
        }
        return mesh;
    }

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        Mesh mesh = GetMesh(blockData);

        int faceIndex = TextureArrayManager.GetIndexByName(stairsName);

        chunk.GetLights(pos.x, pos.y + 1, pos.z, out float skyLight, out float blockLight);

        NBTMesh nbtMesh = nbtGO.nbtMesh;
        ushort startIndex = nbtMesh.vertexCount;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            SetVertex(nbtMesh, mesh.vertices[i] + pos, faceIndex, mesh.uv[i], skyLight, blockLight, 1, Color.white, Vector3.zero);
        }
        foreach (int index in mesh.triangles)
        {
            nbtMesh.triangleArray[nbtMesh.triangleCount++] = (ushort)(startIndex + index);
        }
    }

    public override Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte blockData)
    {
        return GetMesh(blockData);
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
}
