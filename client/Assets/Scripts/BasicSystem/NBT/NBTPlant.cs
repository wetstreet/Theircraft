using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTPlant : NBTBlock
{
    protected int plantIndex;

    public virtual int GetPlantIndexByData(NBTChunk chunk, int data) { return GetPlantIndexByData(data); }

    public virtual int GetPlantIndexByData(int data) { return 0; }

    public override float breakNeedTime { get { return 0; } }

    public override SoundMaterial soundMaterial { get { return SoundMaterial.Grass; } }

    public override bool isTransparent { get { return true; } }

    public override bool isCollidable { get { return false; } }

    public override bool hasDropItem { get { return false; } }

    public override void AddCube(NBTChunk chunk, byte blockData, byte skyLight, Vector3Int pos, NBTGameObject nbtGO)
    {
        this.pos = pos;
        vertices = nbtGO.vertexList;
        triangles = nbtGO.triangles;

        plantIndex = GetPlantIndexByData(chunk, blockData);
        tintColor = GetTintColorByData(chunk, blockData);

        try
        {
            AddDiagonalFace();
            AddAntiDiagonalFace();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString() + "\n" + "pos=" + pos + ",data=" + blockData);
        }
    }

    protected Color tintColor;

    protected virtual Color GetTintColorByData(NBTChunk chunk, byte data) { return Color.white; }

    public override Color GetFrontTintColorByData(NBTChunk chunk, byte data) { return GetTintColorByData(chunk, data); }

    void AddDiagonalFace()
    {
        AddFace(farBottomLeft, farTopLeft, nearTopRight, nearBottomRight, plantIndex, tintColor);
    }

    void AddAntiDiagonalFace()
    {
        AddFace(nearBottomLeft, nearTopLeft, farTopRight, farBottomRight, plantIndex, tintColor);
    }
    
    public override string pathPrefix { get { return "GUI/block/"; } }

    public override Material GetItemMaterial(byte data)
    {
        if (!itemMaterialDict.ContainsKey(data))
        {
            Material mat = new Material(Shader.Find("Custom/BlockShader"));
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(data));
            mat.mainTexture = tex;
            itemMaterialDict.Add(data, mat);
        }
        return itemMaterialDict[data];
    }

    public override Mesh GetItemMesh(NBTChunk chunk, byte data)
    {
        byte index = (byte)(data % 4);
        if (!itemMeshDict.ContainsKey(index))
        {
            Texture2D tex = Resources.Load<Texture2D>(pathPrefix + GetIconPathByData(index));
            Mesh mesh = ItemMeshGenerator.instance.Generate(tex);
            itemMeshDict.Add(index, mesh);
        }
        return itemMeshDict[index];
    }

    //public override Mesh GetItemMesh(NBTChunk chunk, byte data)
    //{
    //    if (!itemMeshDict.ContainsKey(data))
    //    {
    //        Mesh oldMesh = GetPrefabMesh(chunk, data);

    //        Mesh mesh = new Mesh();

    //        pos = Vector3Int.zero;
    //        blockData = data;

    //        List<Vertex> vertexList = new List<Vertex>();
    //        List<int> triangles = new List<int>();

    //        int index = GetPlantIndexByData(data);

    //        Color color = GetTintColorByData(chunk, data);

    //        for (int i = 0; i < oldMesh.vertices.Length; i++)
    //        {
    //            vertexList.Add(new Vertex { pos = ToVector4(oldMesh.vertices[i], index), texcoord = oldMesh.uv[i], color = color });
    //        }
    //        foreach (int i in oldMesh.triangles)
    //        {
    //            triangles.Add(i);
    //        }

    //        var vertexCount = vertexList.Count;

    //        mesh.SetVertexBufferParams(vertexCount, new[] {
    //            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4),
    //            new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
    //            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
    //        });

    //        var verts = new NativeArray<Vertex>(vertexCount, Allocator.Temp);

    //        verts.CopyFrom(vertexList.ToArray());

    //        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);
    //        mesh.SetTriangles(triangles.ToArray(), 0);
    //        mesh.RecalculateBounds();

    //        itemMeshDict.Add(data, mesh);
    //    }

    //    return itemMeshDict[data];
    //}
}
