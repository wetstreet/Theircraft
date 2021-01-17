using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class NBTSapling : NBTPlant
{
    public override string name { get { return "Sapling"; } }
    public override string id { get { return "minecraft:sapling"; } }

    public override void Init()
    {
        UsedTextures = new string[] { "sapling_oak", "sapling_spruce", "sapling_birch", "sapling_jungle" };
    }

    public override string GetIconPathByData(short data)
    {
        switch (data % 4)
        {
            case 0:
                return "sapling_oak";
            case 1:
                return "sapling_spruce";
            case 2:
                return "sapling_birch";
            case 3:
                return "sapling_jungle";
        }
        return null;
    }

    public override bool hasDropItem { get { return true; } }

    public override Mesh GetItemMesh(NBTChunk chunk, byte data)
    {
        if (!itemMeshDict.ContainsKey(data))
        {
            Mesh oldMesh = null;
            string path;
            switch (data % 4)
            {
                case 0:
                    path = "oak_sapling";
                    oldMesh = Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
                    break;
                case 1:
                    path = "spruce_sapling";
                    oldMesh = Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
                    break;
                case 2:
                    path = "birch_sapling";
                    oldMesh = Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
                    break;
                case 3:
                    path = "jungle_sapling";
                    oldMesh = Resources.Load<Mesh>("Meshes/items/" + path + "/" + path);
                    break;
            }
            
            Mesh mesh = new Mesh();

            pos = Vector3Int.zero;
            blockData = data;

            List<Vertex> vertexList = new List<Vertex>();
            List<int> triangles = new List<int>();
            
            int index = GetPlantIndexByData(data);
            for (int i = 0; i < oldMesh.vertices.Length; i++)
            {
                vertexList.Add(new Vertex { pos = ToVector4(oldMesh.vertices[i], index), texcoord = oldMesh.uv[i], color = Color.white });
            }
            foreach (int i in oldMesh.triangles)
            {
                triangles.Add(i);
            }
            
            var vertexCount = vertexList.Count;

            mesh.SetVertexBufferParams(vertexCount, new[] {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 4),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
            });

            var verts = new NativeArray<Vertex>(vertexCount, Allocator.Temp);

            verts.CopyFrom(vertexList.ToArray());

            mesh.SetVertexBufferData(verts, 0, 0, vertexCount);
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.RecalculateBounds();

            itemMeshDict.Add(data, mesh);
        }

        return itemMeshDict[data];
    }

    public override int GetPlantIndexByData(int data)
    {
        switch (data % 4)
        {
            case 0:
                return TextureArrayManager.GetIndexByName("sapling_oak");
            case 1:
                return TextureArrayManager.GetIndexByName("sapling_spruce");
            case 2:
                return TextureArrayManager.GetIndexByName("sapling_birch");
            case 3:
                return TextureArrayManager.GetIndexByName("sapling_jungle");
        }
        throw new System.Exception("no index");
    }

    public override string GetBreakEffectTexture(byte data)
    {
        switch (data % 4)
        {
            case 0:
                return "sapling_oak";
            case 1:
                return "sapling_spruce";
            case 2:
                return "sapling_birch";
            case 3:
                return "sapling_jungle";
        }
        Debug.Log("no break effect texture, data=" + data);
        return "sapling_oak";
    }
}
