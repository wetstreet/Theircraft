using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTFence : NBTBlock
{
    public virtual string fenceName { get { return "planks_oak"; } }
    public override string name { get { return "Oak Fence"; } }
    public override string id { get { return "minecraft:fence"; } }

    public override float hardness => 2;

    Mesh mesh;
    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;
    public override void Init()
    {
        mesh = Resources.Load<Mesh>("Meshes/blocks/fence/wall_0000");
        vertices = mesh.vertices;
        uvs = mesh.uv;
        triangles = mesh.triangles;
    }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Wood; } }

    public override string GetIconPathByData(short data) { return "BirchWoodStairs"; }

    public override string GetBreakEffectTexture(NBTChunk chunk, byte data) { return "planks_birch"; }

    public override bool isTransparent => true;

    public override void AddCube(NBTChunk chunk, byte blockData, Vector3Int pos, NBTGameObject nbtGO)
    {
        int faceIndex = TextureArrayManager.GetIndexByName(fenceName);

        NBTMesh nbtMesh = nbtGO.nbtMesh;
        ushort startIndex = nbtMesh.vertexCount;
        for (int i = 0; i < vertices.Length; i++)
        {
            SetVertex(nbtMesh, vertices[i] + pos, faceIndex, uvs[i], 1, Color.white, Vector3.zero);
        }
        foreach (int index in triangles)
        {
            nbtMesh.triangleArray[nbtMesh.triangleCount++] = (ushort)(startIndex + index);
        }
    }

    public override Mesh GetItemMesh(NBTChunk chunk, byte blockData)
    {
        return mesh;
    }

    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(0))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>("GUI/block/" + fenceName);
            mat.mainTexture = tex;

            itemMaterialDict.Add(0, mat);
        }
        return itemMaterialDict[0];
    }
}
