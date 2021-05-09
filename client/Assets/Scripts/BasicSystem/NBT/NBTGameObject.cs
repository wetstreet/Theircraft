using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static MeshGenerator;

public class NBTGameObject : MonoBehaviour
{
    public NBTMesh nbtMesh;

    bool isCollidable;

    NBTChunk chunk;

    public static NBTGameObject Create(string name, NBTChunk chunk, int layer, bool isCollidable = true)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = chunk.transform;
        go.AddComponent<MeshFilter>();
        go.layer = layer;
        if (isCollidable)
        {
            go.AddComponent<MeshCollider>();
        }

        string matPath = "Materials/block";
        if (layer == 12)
        {
            matPath = "Materials/plant";
        }
        go.AddComponent<MeshRenderer>().material = Resources.Load<Material>(matPath);
        go.AddComponent<NavMeshSourceTag>();
        NBTGameObject nbtGO = go.AddComponent<NBTGameObject>();
        nbtGO.nbtMesh.mesh.name = name;
        nbtGO.isCollidable = isCollidable;
        nbtGO.chunk = chunk;
        return nbtGO;
    }

    public void Clear()
    {
        nbtMesh.Clear();
    }

    private void Awake()
    {
        nbtMesh = new NBTMesh(65536, chunk);
    }

    private void OnDestroy()
    {
        nbtMesh.Dispose();
    }

    public void Refresh()
    {
        nbtMesh.Refresh();

        GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_Array", TextureArrayManager.GetArray());
        GetComponent<MeshFilter>().sharedMesh = nbtMesh.mesh;
        if (isCollidable)
        {
            GetComponent<MeshCollider>().sharedMesh = nbtMesh.mesh;
        }
    }

}
