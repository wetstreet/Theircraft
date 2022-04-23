using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NBTObject
{
    public virtual string name { get; }

    public virtual string id { get; }

    public virtual byte maxStackCount { get { return 64; } }

    public virtual short burningTime { get { return -1; } }

    public virtual short cookTimeTotal { get { return 200; } }

    public virtual string smeltResult { get { return null; } }

    public virtual string GetNameByData(short data) { return name; }

    public virtual string pathPrefix { get { return "GUI/icon/"; } }
    public bool useBlockOnUI { get { return pathPrefix == "GUI/icon/"; } }

    public virtual string GetIconPathByData(short data = 0) { return GetNameByData(data).Trim(); }

    static Material commonItemMat;

    public virtual int attackDamage { get { return 1; } }

    protected Dictionary<byte, Material> itemMaterialDict = new Dictionary<byte, Material>();


    protected static Vector3 itemSize_half = new Vector3(0.5f, 0.5f, 0.5f);
    public virtual Vector3 itemSize { get { return Vector3.one; } }

    public virtual Material GetItemMaterial(byte data)
    {
        if (commonItemMat == null)
        {
            commonItemMat = new Material(Shader.Find("Custom/BlockShader"));
            commonItemMat.SetTexture("_MainTex", TextureArrayManager.atlas);
        }
        return commonItemMat;
    }

    protected Dictionary<byte, Mesh> itemMeshDict = new Dictionary<byte, Mesh>();

    protected virtual string itemMeshPath { get { return null; } }

    public virtual Mesh GetPrefabMesh(NBTChunk chunk, byte data)
    {
        if (!string.IsNullOrEmpty(itemMeshPath))
        {
            return Resources.Load<Mesh>("Meshes/items/" + itemMeshPath + "/" + itemMeshPath);
        }
        return null;
    }

    protected void CopyFromMesh(Mesh mesh, Vector3Int pos, List<Vector3> vertices, List<Vector2> uv, List<int> triangles)
    {
        int length = vertices.Count;
        foreach (Vector3 vertex in mesh.vertices)
        {
            vertices.Add(vertex + pos);
        }
        uv.AddRange(mesh.uv);

        foreach (int index in mesh.triangles)
        {
            triangles.Add(index + length);
        }
    }

    public virtual Mesh GetItemMesh(NBTChunk chunk, Vector3Int pos, byte data)
    {
        if (!itemMeshDict.ContainsKey(data))
        {
            Mesh oldMesh = GetPrefabMesh(chunk, data);

            Mesh mesh = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();

            foreach (Vector3 vertex in oldMesh.vertices)
            {
                vertices.Add(vertex);
            }
            uv.AddRange(oldMesh.uv);

            foreach (int index in oldMesh.triangles)
            {
                triangles.Add(index);
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = triangles.ToArray();

            itemMeshDict.Add(data, mesh);
        }

        return itemMeshDict[data];
    }
}
