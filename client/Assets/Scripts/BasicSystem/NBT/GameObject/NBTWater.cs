using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBTWater : NBTGameObject
{
    public override int meshSize => 8192;

    public static NBTGameObject Create(NBTChunk chunk)
    {
        GameObject go = new GameObject("Water");
        go.transform.parent = chunk.transform;
        go.AddComponent<MeshFilter>();
        go.layer = LayerMask.NameToLayer("Water");

        Material mat = Resources.Load<Material>("Materials/block/water_still");
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        go.AddComponent<MeshCollider>();

        NBTGameObject nbtGO = go.AddComponent<NBTWater>();
        nbtGO.mat = mat;
        nbtGO.nbtMesh.mesh.name = "Water";
        return nbtGO;
    }
}
